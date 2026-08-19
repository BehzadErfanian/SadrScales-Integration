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
    /// Per-scale PLU/item-code mapping operations.
    /// </summary>
    /// <remarks>
    /// Mapping writes are full atomic replacements, matching Sadr Scales 5.2.1 behavior.
    /// A real mapping change resets both internal Item and HotKey AutoSend watermarks for the affected scale.
    /// </remarks>
    public sealed class SadrScaleMappingClient
    {
        #region Dependencies

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly SadrScalesClientOptions _options;

        #endregion

        #region Construction

        internal SadrScaleMappingClient(SqlConnectionFactory connectionFactory, SadrScalesClientOptions options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
        }

        #endregion

        #region Read API

        /// <summary>
        /// Reads the complete per-scale item mapping ordered by PLU number.
        /// </summary>
        public Task<IReadOnlyList<SadrScaleItemMap>> GetAsync(
            int scaleId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateScaleId(scaleId, nameof(scaleId));

            return _connectionFactory.ExecuteReadAsync<IReadOnlyList<SadrScaleItemMap>>(
                async (connection, token) =>
                {
                    var result = new List<SadrScaleItemMap>();
                    const string sql = @"
SELECT ScaleID, PluNo, ItemCode, PageNo, KeyNo
FROM dbo.SADR_ScaleItemMap
WHERE ScaleID = @ScaleID
ORDER BY PluNo ASC;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;

                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                result.Add(Map(reader));
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
        /// Atomically replaces the complete per-scale item mapping.
        /// </summary>
        /// <remarks>
        /// An empty collection is valid and clears the user mapping. Duplicate PLUs, duplicate ItemCodes,
        /// duplicate HotKey positions, partial Page/Key pairs and positions outside the scale's configured
        /// HotKey layout are rejected before the old mapping is removed.
        /// </remarks>
        public async Task<SadrReplaceResult> ReplaceAsync(
            int scaleId,
            IEnumerable<SadrScaleItemMap> mappings,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateScaleId(scaleId, nameof(scaleId));
            var requested = NormalizeMappings(scaleId, mappings);

            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ScaleLayout layout = await ReadScaleLayoutAsync(
                        connection,
                        transaction,
                        scaleId,
                        cancellationToken).ConfigureAwait(false);

                    if (layout == null)
                    {
                        transaction.Rollback();
                        return SadrReplaceResult.NotFound;
                    }

                    ValidateAgainstLayout(requested, layout);
                    await EnsureItemsExistAsync(connection, transaction, requested, cancellationToken).ConfigureAwait(false);

                    var current = await ReadMappingsAsync(
                        connection,
                        transaction,
                        scaleId,
                        cancellationToken).ConfigureAwait(false);

                    if (MappingsEqual(current, requested))
                    {
                        transaction.Commit();
                        return SadrReplaceResult.Unchanged;
                    }

                    await ReplaceMappingsInternalAsync(
                        connection,
                        transaction,
                        scaleId,
                        requested,
                        cancellationToken).ConfigureAwait(false);

                    await ResetSendStateAsync(connection, transaction, scaleId, cancellationToken).ConfigureAwait(false);

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

        /// <summary>
        /// Atomically copies the complete mapping from one registered scale to another.
        /// </summary>
        /// <remarks>
        /// The source mapping must fit the destination scale's HotKey layout. The destination is not modified
        /// when validation fails. A real change resets Item and HotKey AutoSend watermarks on the destination.
        /// </remarks>
        public async Task<SadrReplaceResult> CopyAsync(
            int sourceScaleId,
            int destinationScaleId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateScaleId(sourceScaleId, nameof(sourceScaleId));
            ValidateScaleId(destinationScaleId, nameof(destinationScaleId));

            if (sourceScaleId == destinationScaleId)
            {
                throw new ArgumentException("Source and destination scales must be different.", nameof(destinationScaleId));
            }

            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ScaleLayout sourceLayout = await ReadScaleLayoutAsync(
                        connection,
                        transaction,
                        sourceScaleId,
                        cancellationToken).ConfigureAwait(false);
                    ScaleLayout destinationLayout = await ReadScaleLayoutAsync(
                        connection,
                        transaction,
                        destinationScaleId,
                        cancellationToken).ConfigureAwait(false);

                    if (sourceLayout == null || destinationLayout == null)
                    {
                        transaction.Rollback();
                        return SadrReplaceResult.NotFound;
                    }

                    var source = await ReadMappingsAsync(
                        connection,
                        transaction,
                        sourceScaleId,
                        cancellationToken).ConfigureAwait(false);
                    var destination = await ReadMappingsAsync(
                        connection,
                        transaction,
                        destinationScaleId,
                        cancellationToken).ConfigureAwait(false);

                    var copied = source
                        .Select(item => new SadrScaleItemMap
                        {
                            ScaleId = destinationScaleId,
                            PluNo = item.PluNo,
                            ItemCode = item.ItemCode,
                            PageNo = item.PageNo,
                            KeyNo = item.KeyNo
                        })
                        .ToList();

                    ValidateAgainstLayout(copied, destinationLayout);

                    if (MappingsEqual(destination, copied))
                    {
                        transaction.Commit();
                        return SadrReplaceResult.Unchanged;
                    }

                    await ReplaceMappingsInternalAsync(
                        connection,
                        transaction,
                        destinationScaleId,
                        copied,
                        cancellationToken).ConfigureAwait(false);
                    await ResetSendStateAsync(
                        connection,
                        transaction,
                        destinationScaleId,
                        cancellationToken).ConfigureAwait(false);

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

        private async Task<ScaleLayout> ReadScaleLayoutAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            int scaleId,
            CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT HotKeyCountPerPage, HotKeyPageCount
FROM dbo.SADR_Scale WITH (UPDLOCK, HOLDLOCK)
WHERE ScaleID = @ScaleID;";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;

                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        return null;
                    }

                    return new ScaleLayout
                    {
                        HotKeyCountPerPage = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0)),
                        HotKeyPageCount = reader.IsDBNull(1) ? 0 : Convert.ToInt32(reader.GetValue(1))
                    };
                }
            }
        }

        private async Task<List<SadrScaleItemMap>> ReadMappingsAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            int scaleId,
            CancellationToken cancellationToken)
        {
            var result = new List<SadrScaleItemMap>();
            const string sql = @"
SELECT ScaleID, PluNo, ItemCode, PageNo, KeyNo
FROM dbo.SADR_ScaleItemMap WITH (UPDLOCK, HOLDLOCK)
WHERE ScaleID = @ScaleID
ORDER BY PluNo ASC;";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;

                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        result.Add(Map(reader));
                    }
                }
            }

            return result;
        }

        private async Task EnsureItemsExistAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            IReadOnlyList<SadrScaleItemMap> mappings,
            CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT COUNT(*)
FROM dbo.SADR_Item
WHERE PluNo = @PluNo;";

            foreach (SadrScaleItemMap item in mappings)
            {
                using (var command = new SqlCommand(sql, connection, transaction))
                {
                    command.CommandTimeout = _options.CommandTimeoutSeconds;
                    command.Parameters.Add("@PluNo", SqlDbType.Int).Value = item.PluNo;
                    int count = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
                    if (count != 1)
                    {
                        throw new ArgumentException("PLU " + item.PluNo + " does not exist in the catalog.", nameof(mappings));
                    }
                }
            }
        }

        private async Task ReplaceMappingsInternalAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            int scaleId,
            IReadOnlyList<SadrScaleItemMap> mappings,
            CancellationToken cancellationToken)
        {
            using (var delete = new SqlCommand(
                "DELETE FROM dbo.SADR_ScaleItemMap WHERE ScaleID = @ScaleID;",
                connection,
                transaction))
            {
                delete.CommandTimeout = _options.CommandTimeoutSeconds;
                delete.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;
                await delete.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            const string insertSql = @"
INSERT INTO dbo.SADR_ScaleItemMap(ScaleID, PluNo, ItemCode, PageNo, KeyNo)
VALUES(@ScaleID, @PluNo, @ItemCode, @PageNo, @KeyNo);";

            foreach (SadrScaleItemMap item in mappings)
            {
                using (var insert = new SqlCommand(insertSql, connection, transaction))
                {
                    insert.CommandTimeout = _options.CommandTimeoutSeconds;
                    insert.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;
                    insert.Parameters.Add("@PluNo", SqlDbType.Int).Value = item.PluNo;
                    insert.Parameters.Add("@ItemCode", SqlDbType.Int).Value = item.ItemCode;
                    insert.Parameters.Add("@PageNo", SqlDbType.Int).Value = item.PageNo.HasValue
                        ? (object)item.PageNo.Value
                        : DBNull.Value;
                    insert.Parameters.Add("@KeyNo", SqlDbType.Int).Value = item.KeyNo.HasValue
                        ? (object)item.KeyNo.Value
                        : DBNull.Value;
                    await insert.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task ResetSendStateAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            int scaleId,
            CancellationToken cancellationToken)
        {
            const string sql = @"
UPDATE dbo.SADR_Scale
SET LastSendItem = 0,
    LastSendKey = 0
WHERE ScaleID = @ScaleID;";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private static SadrScaleItemMap Map(SqlDataReader reader)
        {
            return new SadrScaleItemMap
            {
                ScaleId = reader.GetInt32(0),
                PluNo = reader.GetInt32(1),
                ItemCode = reader.GetInt32(2),
                PageNo = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                KeyNo = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4)
            };
        }

        #endregion

        #region Validation

        private static List<SadrScaleItemMap> NormalizeMappings(
            int scaleId,
            IEnumerable<SadrScaleItemMap> mappings)
        {
            if (mappings == null)
            {
                throw new ArgumentNullException(nameof(mappings));
            }

            var result = new List<SadrScaleItemMap>();
            var pluNumbers = new HashSet<int>();
            var itemCodes = new HashSet<int>();
            var positions = new HashSet<string>(StringComparer.Ordinal);

            foreach (SadrScaleItemMap source in mappings)
            {
                if (source == null)
                {
                    throw new ArgumentException("Mapping entries cannot be null.", nameof(mappings));
                }

                if (source.ScaleId != 0 && source.ScaleId != scaleId)
                {
                    throw new ArgumentException("A mapping entry belongs to a different Scale ID.", nameof(mappings));
                }

                if (source.PluNo <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(mappings), "PLU numbers must be positive.");
                }

                if (source.ItemCode <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(mappings), "Scale item codes must be positive.");
                }

                bool hasPage = source.PageNo.HasValue;
                bool hasKey = source.KeyNo.HasValue;
                if (hasPage != hasKey)
                {
                    throw new ArgumentException("PageNo and KeyNo must either both be null or both be populated.", nameof(mappings));
                }

                if (!pluNumbers.Add(source.PluNo))
                {
                    throw new ArgumentException("Duplicate PLU number in scale mapping: " + source.PluNo + ".", nameof(mappings));
                }

                if (!itemCodes.Add(source.ItemCode))
                {
                    throw new ArgumentException("Duplicate scale ItemCode in scale mapping: " + source.ItemCode + ".", nameof(mappings));
                }

                if (hasPage)
                {
                    string position = source.PageNo.Value + ":" + source.KeyNo.Value;
                    if (!positions.Add(position))
                    {
                        throw new ArgumentException("Duplicate HotKey position in scale mapping: " + position + ".", nameof(mappings));
                    }
                }

                result.Add(new SadrScaleItemMap
                {
                    ScaleId = scaleId,
                    PluNo = source.PluNo,
                    ItemCode = source.ItemCode,
                    PageNo = source.PageNo,
                    KeyNo = source.KeyNo
                });
            }

            result.Sort((left, right) => left.PluNo.CompareTo(right.PluNo));
            return result;
        }

        private static void ValidateAgainstLayout(
            IReadOnlyList<SadrScaleItemMap> mappings,
            ScaleLayout layout)
        {
            foreach (SadrScaleItemMap item in mappings)
            {
                if (!item.PageNo.HasValue)
                {
                    continue;
                }

                if (layout.HotKeyCountPerPage <= 0 || layout.HotKeyPageCount <= 0)
                {
                    throw new ArgumentException("The target scale does not expose a usable HotKey layout.", nameof(mappings));
                }

                if (item.PageNo.Value < 0 || item.PageNo.Value >= layout.HotKeyPageCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(mappings), "A HotKey page is outside the target scale layout.");
                }

                if (!item.KeyNo.HasValue || item.KeyNo.Value <= 0 || item.KeyNo.Value > layout.HotKeyCountPerPage)
                {
                    throw new ArgumentOutOfRangeException(nameof(mappings), "A HotKey position is outside the target scale layout.");
                }
            }
        }

        private static bool MappingsEqual(
            IReadOnlyList<SadrScaleItemMap> left,
            IReadOnlyList<SadrScaleItemMap> right)
        {
            if (left.Count != right.Count)
            {
                return false;
            }

            for (int index = 0; index < left.Count; index++)
            {
                SadrScaleItemMap a = left[index];
                SadrScaleItemMap b = right[index];
                if (a.PluNo != b.PluNo ||
                    a.ItemCode != b.ItemCode ||
                    a.PageNo != b.PageNo ||
                    a.KeyNo != b.KeyNo)
                {
                    return false;
                }
            }

            return true;
        }

        private static void ValidateScaleId(int scaleId, string parameterName)
        {
            if (scaleId < 1 || scaleId > 99)
            {
                throw new ArgumentOutOfRangeException(parameterName, "Scale ID must be between 1 and 99.");
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
                // Preserve the original operation exception.
            }
        }

        private sealed class ScaleLayout
        {
            public int HotKeyCountPerPage { get; set; }
            public int HotKeyPageCount { get; set; }
        }

        #endregion
    }
}
