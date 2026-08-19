using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration.Internal;

namespace SadrScales.Integration.Assignments
{
    /// <summary>
    /// Canonical multi-group assignment operations for registered scales.
    /// </summary>
    public sealed class SadrScaleAssignmentClient
    {
        #region Dependencies

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly SadrScalesClientOptions _options;

        #endregion

        #region Construction

        internal SadrScaleAssignmentClient(SqlConnectionFactory connectionFactory, SadrScalesClientOptions options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
        }

        #endregion

        #region Read API

        /// <summary>
        /// Reads the canonical item-group assignments for one scale.
        /// </summary>
        public Task<IReadOnlyList<string>> GetGroupsAsync(
            int scaleId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateScaleId(scaleId);

            return _connectionFactory.ExecuteReadAsync<IReadOnlyList<string>>(
                async (connection, token) =>
                {
                    var result = new List<string>();
                    const string sql = @"
SELECT ItemClassCode
FROM dbo.SADR_ScaleItemClass
WHERE ScaleID = @ScaleID
ORDER BY ItemClassCode ASC;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;

                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                result.Add(reader.GetString(0));
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
        /// Atomically replaces all canonical item-group assignments for one scale.
        /// </summary>
        /// <remarks>
        /// At least one group is required, matching Sadr Scales 5.2.1 behavior. A real change resets the
        /// internal item-send watermark so the next eligible AutoSend cycle can re-evaluate the assigned catalog.
        /// </remarks>
        public async Task<SadrReplaceResult> ReplaceGroupsAsync(
            int scaleId,
            IEnumerable<string> itemClassCodes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateScaleId(scaleId);
            var requested = NormalizeCodes(itemClassCodes);

            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    if (!await ScaleExistsAsync(connection, transaction, scaleId, cancellationToken).ConfigureAwait(false))
                    {
                        transaction.Rollback();
                        return SadrReplaceResult.NotFound;
                    }

                    await EnsureGroupsExistAsync(connection, transaction, requested, cancellationToken).ConfigureAwait(false);
                    var current = await ReadGroupsAsync(connection, transaction, scaleId, cancellationToken).ConfigureAwait(false);

                    if (current.SequenceEqual(requested, StringComparer.OrdinalIgnoreCase))
                    {
                        transaction.Commit();
                        return SadrReplaceResult.Unchanged;
                    }

                    using (var delete = new SqlCommand(
                        "DELETE FROM dbo.SADR_ScaleItemClass WHERE ScaleID = @ScaleID;",
                        connection,
                        transaction))
                    {
                        delete.CommandTimeout = _options.CommandTimeoutSeconds;
                        delete.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;
                        await delete.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }

                    const string insertSql = @"
INSERT INTO dbo.SADR_ScaleItemClass(ScaleID, ItemClassCode)
VALUES(@ScaleID, @ItemClassCode);";

                    foreach (string code in requested)
                    {
                        using (var insert = new SqlCommand(insertSql, connection, transaction))
                        {
                            insert.CommandTimeout = _options.CommandTimeoutSeconds;
                            insert.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;
                            insert.Parameters.Add("@ItemClassCode", SqlDbType.VarChar, 50).Value = code;
                            await insert.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }

                    using (var reset = new SqlCommand(
                        "UPDATE dbo.SADR_Scale SET LastSendItem = 0 WHERE ScaleID = @ScaleID;",
                        connection,
                        transaction))
                    {
                        reset.CommandTimeout = _options.CommandTimeoutSeconds;
                        reset.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;
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

        private async Task<List<string>> ReadGroupsAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            int scaleId,
            CancellationToken cancellationToken)
        {
            var result = new List<string>();
            const string sql = @"
SELECT ItemClassCode
FROM dbo.SADR_ScaleItemClass WITH (UPDLOCK, HOLDLOCK)
WHERE ScaleID = @ScaleID
ORDER BY ItemClassCode ASC;";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;

                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        result.Add(reader.GetString(0));
                    }
                }
            }

            return result;
        }

        private async Task EnsureGroupsExistAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            IReadOnlyList<string> requested,
            CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT COUNT(*)
FROM dbo.SADR_ItemClass
WHERE ItemClassCode = @ItemClassCode;";

            foreach (string code in requested)
            {
                using (var command = new SqlCommand(sql, connection, transaction))
                {
                    command.CommandTimeout = _options.CommandTimeoutSeconds;
                    command.Parameters.Add("@ItemClassCode", SqlDbType.VarChar, 50).Value = code;
                    int count = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
                    if (count != 1)
                    {
                        throw new ArgumentException("Item group '" + code + "' does not exist.", nameof(requested));
                    }
                }
            }
        }

        private async Task<bool> ScaleExistsAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            int scaleId,
            CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT COUNT(*)
FROM dbo.SADR_Scale WITH (UPDLOCK, HOLDLOCK)
WHERE ScaleID = @ScaleID;";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;
                return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false)) == 1;
            }
        }

        #endregion

        #region Validation

        private static List<string> NormalizeCodes(IEnumerable<string> itemClassCodes)
        {
            if (itemClassCodes == null)
            {
                throw new ArgumentNullException(nameof(itemClassCodes));
            }

            var result = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string raw in itemClassCodes)
            {
                string code = (raw ?? string.Empty).Trim();
                if (code.Length == 0)
                {
                    continue;
                }

                if (code.Length > 50)
                {
                    throw new ArgumentException("ItemClassCode exceeds 50 characters.", nameof(itemClassCodes));
                }

                if (seen.Add(code))
                {
                    result.Add(code);
                }
            }

            if (result.Count == 0)
            {
                throw new ArgumentException("At least one valid item group is required.", nameof(itemClassCodes));
            }

            result.Sort(StringComparer.OrdinalIgnoreCase);
            return result;
        }

        private static void ValidateScaleId(int scaleId)
        {
            if (scaleId < 1 || scaleId > 99)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleId), "Scale ID must be between 1 and 99.");
            }
        }

        private static void TryRollback(SqlTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // Preserve the original exception.
            }
        }

        #endregion
    }
}
