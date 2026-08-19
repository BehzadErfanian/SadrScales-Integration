using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration.Internal;

namespace SadrScales.Integration.HotKeys
{
    /// <summary>
    /// User-managed item-group HotKey template operations.
    /// </summary>
    /// <remarks>
    /// Internal/system rows whose PLU is zero or negative are not part of the public template and are preserved.
    /// A real template change resets the HotKey AutoSend watermark for registered scales assigned to the group.
    /// </remarks>
    public sealed class SadrHotKeyClient
    {
        #region Dependencies

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly SadrScalesClientOptions _options;

        #endregion

        #region Construction

        internal SadrHotKeyClient(SqlConnectionFactory connectionFactory, SadrScalesClientOptions options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
        }

        #endregion

        #region Read API

        /// <summary>
        /// Reads user-managed HotKeys for one item group, ordered by page and key.
        /// </summary>
        public Task<IReadOnlyList<SadrHotKey>> GetGroupAsync(
            string itemClassCode,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string groupCode = ValidateGroupCode(itemClassCode);

            return _connectionFactory.ExecuteReadAsync<IReadOnlyList<SadrHotKey>>(
                async (connection, token) =>
                {
                    var result = new List<SadrHotKey>();
                    const string sql = @"
SELECT PageNo, KeyNo, PluNo
FROM dbo.SADR_KeyAssignment
WHERE ItemClassCode = @ItemClassCode
  AND PluNo > 0
ORDER BY PageNo ASC, KeyNo ASC;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        command.Parameters.Add("@ItemClassCode", SqlDbType.VarChar, 50).Value = groupCode;

                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                result.Add(new SadrHotKey
                                {
                                    PageNo = reader.GetInt32(0),
                                    KeyNo = reader.GetInt32(1),
                                    PluNo = reader.GetInt32(2)
                                });
                            }
                        }
                    }

                    return result;
                },
                cancellationToken);
        }

        #endregion

        #region Replace API

        /// <summary>
        /// Atomically replaces the user-managed HotKey template for one item group.
        /// </summary>
        /// <remarks>
        /// An empty collection clears only positive-PLU user HotKeys. Internal/system rows with zero or negative
        /// PLUs remain untouched. A real change resets <c>LastSendKey</c> for scales canonically assigned to the group.
        /// </remarks>
        public async Task<SadrReplaceResult> ReplaceGroupAsync(
            string itemClassCode,
            IEnumerable<SadrHotKey> hotKeys,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string groupCode = ValidateGroupCode(itemClassCode);
            List<SadrHotKey> requested = NormalizeHotKeys(hotKeys);

            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    if (!await GroupExistsAsync(connection, transaction, groupCode, cancellationToken).ConfigureAwait(false))
                    {
                        transaction.Rollback();
                        return SadrReplaceResult.NotFound;
                    }

                    await EnsureItemsExistAsync(connection, transaction, requested, cancellationToken).ConfigureAwait(false);
                    List<SadrHotKey> current = await ReadUserHotKeysAsync(
                        connection,
                        transaction,
                        groupCode,
                        cancellationToken).ConfigureAwait(false);

                    if (HotKeysEqual(current, requested))
                    {
                        transaction.Commit();
                        return SadrReplaceResult.Unchanged;
                    }

                    using (var delete = new SqlCommand(@"
DELETE FROM dbo.SADR_KeyAssignment
WHERE ItemClassCode = @ItemClassCode
  AND PluNo > 0;", connection, transaction))
                    {
                        delete.CommandTimeout = _options.CommandTimeoutSeconds;
                        delete.Parameters.Add("@ItemClassCode", SqlDbType.VarChar, 50).Value = groupCode;
                        await delete.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }

                    const string insertSql = @"
INSERT INTO dbo.SADR_KeyAssignment(ItemClassCode, PageNo, KeyNo, PluNo)
VALUES(@ItemClassCode, @PageNo, @KeyNo, @PluNo);";

                    foreach (SadrHotKey item in requested)
                    {
                        using (var insert = new SqlCommand(insertSql, connection, transaction))
                        {
                            insert.CommandTimeout = _options.CommandTimeoutSeconds;
                            insert.Parameters.Add("@ItemClassCode", SqlDbType.VarChar, 50).Value = groupCode;
                            insert.Parameters.Add("@PageNo", SqlDbType.Int).Value = item.PageNo;
                            insert.Parameters.Add("@KeyNo", SqlDbType.Int).Value = item.KeyNo;
                            insert.Parameters.Add("@PluNo", SqlDbType.Int).Value = item.PluNo;
                            await insert.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }

                    using (var reset = new SqlCommand(@"
UPDATE s
SET LastSendKey = 0
FROM dbo.SADR_Scale s
WHERE EXISTS
(
    SELECT 1
    FROM dbo.SADR_ScaleItemClass sic
    WHERE sic.ScaleID = s.ScaleID
      AND sic.ItemClassCode = @ItemClassCode
);", connection, transaction))
                    {
                        reset.CommandTimeout = _options.CommandTimeoutSeconds;
                        reset.Parameters.Add("@ItemClassCode", SqlDbType.VarChar, 50).Value = groupCode;
                        await reset.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }

                    transaction.Commit();
                    return SadrReplaceResult.Replaced;
                }
                catch
                {
                    TryRollback(transaction);
                    throw;
                }
            }
        }

        #endregion

        #region SQL Helpers

        private async Task<bool> GroupExistsAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            string groupCode,
            CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT COUNT(*)
FROM dbo.SADR_ItemClass WITH (UPDLOCK, HOLDLOCK)
WHERE ItemClassCode = @ItemClassCode;";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@ItemClassCode", SqlDbType.VarChar, 50).Value = groupCode;
                return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false)) == 1;
            }
        }

        private async Task<List<SadrHotKey>> ReadUserHotKeysAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            string groupCode,
            CancellationToken cancellationToken)
        {
            var result = new List<SadrHotKey>();
            const string sql = @"
SELECT PageNo, KeyNo, PluNo
FROM dbo.SADR_KeyAssignment WITH (UPDLOCK, HOLDLOCK)
WHERE ItemClassCode = @ItemClassCode
  AND PluNo > 0
ORDER BY PageNo ASC, KeyNo ASC;";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@ItemClassCode", SqlDbType.VarChar, 50).Value = groupCode;

                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        result.Add(new SadrHotKey
                        {
                            PageNo = reader.GetInt32(0),
                            KeyNo = reader.GetInt32(1),
                            PluNo = reader.GetInt32(2)
                        });
                    }
                }
            }

            return result;
        }

        private async Task EnsureItemsExistAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            IReadOnlyList<SadrHotKey> hotKeys,
            CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT COUNT(*)
FROM dbo.SADR_Item
WHERE PluNo = @PluNo;";

            foreach (SadrHotKey item in hotKeys)
            {
                using (var command = new SqlCommand(sql, connection, transaction))
                {
                    command.CommandTimeout = _options.CommandTimeoutSeconds;
                    command.Parameters.Add("@PluNo", SqlDbType.Int).Value = item.PluNo;
                    int count = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
                    if (count != 1)
                    {
                        throw new ArgumentException("PLU " + item.PluNo + " does not exist in the catalog.", nameof(hotKeys));
                    }
                }
            }
        }

        #endregion

        #region Validation

        private static List<SadrHotKey> NormalizeHotKeys(IEnumerable<SadrHotKey> hotKeys)
        {
            if (hotKeys == null)
            {
                throw new ArgumentNullException(nameof(hotKeys));
            }

            var result = new List<SadrHotKey>();
            var positions = new HashSet<string>(StringComparer.Ordinal);

            foreach (SadrHotKey source in hotKeys)
            {
                if (source == null)
                {
                    throw new ArgumentException("HotKey entries cannot be null.", nameof(hotKeys));
                }

                if (source.PageNo < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(hotKeys), "HotKey PageNo cannot be negative.");
                }

                if (source.KeyNo <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(hotKeys), "HotKey KeyNo must be positive.");
                }

                if (source.PluNo <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(hotKeys), "Public HotKey PLU numbers must be positive.");
                }

                string position = source.PageNo + ":" + source.KeyNo;
                if (!positions.Add(position))
                {
                    throw new ArgumentException("Duplicate HotKey position: " + position + ".", nameof(hotKeys));
                }

                result.Add(new SadrHotKey
                {
                    PageNo = source.PageNo,
                    KeyNo = source.KeyNo,
                    PluNo = source.PluNo
                });
            }

            result.Sort((left, right) =>
            {
                int pageComparison = left.PageNo.CompareTo(right.PageNo);
                return pageComparison != 0 ? pageComparison : left.KeyNo.CompareTo(right.KeyNo);
            });

            return result;
        }

        private static bool HotKeysEqual(IReadOnlyList<SadrHotKey> left, IReadOnlyList<SadrHotKey> right)
        {
            if (left.Count != right.Count)
            {
                return false;
            }

            for (int index = 0; index < left.Count; index++)
            {
                SadrHotKey a = left[index];
                SadrHotKey b = right[index];
                if (a.PageNo != b.PageNo || a.KeyNo != b.KeyNo || a.PluNo != b.PluNo)
                {
                    return false;
                }
            }

            return true;
        }

        private static string ValidateGroupCode(string itemClassCode)
        {
            string code = (itemClassCode ?? string.Empty).Trim();
            if (code.Length == 0)
            {
                throw new ArgumentException("ItemClassCode is required.", nameof(itemClassCode));
            }

            if (code.Length > 50)
            {
                throw new ArgumentException("ItemClassCode exceeds 50 characters.", nameof(itemClassCode));
            }

            return code;
        }

        private static void TryRollback(SqlTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // Preserve the original operation exception.
            }
        }

        #endregion
    }
}
