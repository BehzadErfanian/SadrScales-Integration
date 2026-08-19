using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SadrScales.Integration.Items
{
    public sealed partial class SadrItemClient
    {
        #region Read SQL

        private const string ReadItemColumns = @"
    ItemClassCode, PluNo, PluUnit, UnitPrice, PrintFormat, PluCost,
    BarFormat, BarFlags, ItemCode, IndexBarcode, Tare,
    ShelfDate, ShelfDatePrint, SaleDatePrint, SaleTimePrint,
    OnlyTare, TaxRate, PluName,
    Text1, Text2, Text3, Text4, Text5, Text6, Text7,
    DeleteFlag";

        #endregion

        #region Item Read API

        /// <summary>
        /// Reads one PLU by its public identity. Logically deleted rows are still returned so callers can inspect/recover them.
        /// </summary>
        public Task<SadrItem?> GetAsync(
            int pluNo,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidatePluNo(pluNo);

            return _connectionFactory.ExecuteReadAsync<SadrItem?>(
                async (connection, token) =>
                {
                    string sql = "SELECT " + ReadItemColumns + " FROM dbo.SADR_Item WHERE PluNo = @PluNo;";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        command.Parameters.Add("@PluNo", SqlDbType.Int).Value = pluNo;

                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            return await reader.ReadAsync(token).ConfigureAwait(false)
                                ? MapItem(reader)
                                : null;
                        }
                    }
                },
                cancellationToken);
        }

        /// <summary>
        /// Reads the catalog ordered by PLU number. By default logically deleted rows are excluded.
        /// </summary>
        public Task<IReadOnlyList<SadrItem>> GetAllAsync(
            bool includeDeleted = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _connectionFactory.ExecuteReadAsync<IReadOnlyList<SadrItem>>(
                async (connection, token) =>
                {
                    var result = new List<SadrItem>();
                    string sql = "SELECT " + ReadItemColumns + @"
FROM dbo.SADR_Item" +
                        (includeDeleted ? string.Empty : " WHERE ISNULL(DeleteFlag, 0) = 0") +
                        " ORDER BY PluNo ASC;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                result.Add(MapItem(reader));
                            }
                        }
                    }

                    return result;
                },
                cancellationToken);
        }

        #endregion

        #region Soft Delete API

        /// <summary>
        /// Logically deletes a PLU by setting DeleteFlag=1 rather than physically removing its row.
        /// </summary>
        /// <remarks>
        /// This matches the 5.2.1 item lifecycle used by scale-send queries: active reads use DeleteFlag=0,
        /// while the changed deleted row remains available for synchronization/recovery semantics.
        /// </remarks>
        public async Task<SadrItemSoftDeleteResult> SoftDeleteAsync(
            int pluNo,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidatePluNo(pluNo);

            const string sql = @"
IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SADR_Item WITH (UPDLOCK, HOLDLOCK)
    WHERE PluNo = @PluNo
)
BEGIN
    SELECT CAST(0 AS int);
END
ELSE IF EXISTS
(
    SELECT 1
    FROM dbo.SADR_Item WITH (UPDLOCK, HOLDLOCK)
    WHERE PluNo = @PluNo
      AND ISNULL(DeleteFlag, 0) <> 0
)
BEGIN
    SELECT CAST(2 AS int);
END
ELSE
BEGIN
    UPDATE dbo.SADR_Item
    SET DeleteFlag = 1
    WHERE PluNo = @PluNo;

    SELECT CAST(1 AS int);
END;";

            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@PluNo", SqlDbType.Int).Value = pluNo;

                try
                {
                    var scalar = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                    return (SadrItemSoftDeleteResult)Convert.ToInt32(scalar);
                }
                catch
                {
                    TryRollback(transaction);
                    throw;
                }
            }
        }

        #endregion

        #region Price History API

        /// <summary>
        /// Reads recent price-history entries for one PLU, newest first.
        /// </summary>
        public Task<IReadOnlyList<SadrPriceHistoryEntry>> GetPriceHistoryAsync(
            int pluNo,
            int maxRows = 100,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidatePluNo(pluNo);
            ValidateHistoryLimit(maxRows);
            return ReadPriceHistoryAsync(pluNo, maxRows, cancellationToken);
        }

        /// <summary>
        /// Reads recent price-history entries across all PLUs, newest first.
        /// </summary>
        public Task<IReadOnlyList<SadrPriceHistoryEntry>> GetRecentPriceHistoryAsync(
            int maxRows = 100,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateHistoryLimit(maxRows);
            return ReadPriceHistoryAsync(null, maxRows, cancellationToken);
        }

        private Task<IReadOnlyList<SadrPriceHistoryEntry>> ReadPriceHistoryAsync(
            int? pluNo,
            int maxRows,
            CancellationToken cancellationToken)
        {
            return _connectionFactory.ExecuteReadAsync<IReadOnlyList<SadrPriceHistoryEntry>>(
                async (connection, token) =>
                {
                    var result = new List<SadrPriceHistoryEntry>();
                    const string sql = @"
SELECT TOP (@MaxRows)
    ID, PluNo, IndexBarcode, PluName, LastPrice, NewPrice, [DateTime], [User]
FROM dbo.SADR_PriceLog
WHERE (@PluNo IS NULL OR PluNo = @PluNo)
ORDER BY [DateTime] DESC, ID DESC;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        command.Parameters.Add("@MaxRows", SqlDbType.Int).Value = maxRows;
                        command.Parameters.Add("@PluNo", SqlDbType.Int).Value =
                            pluNo.HasValue ? (object)pluNo.Value : DBNull.Value;

                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                result.Add(new SadrPriceHistoryEntry
                                {
                                    Id = reader.GetInt32(0),
                                    PluNo = reader.GetInt32(1),
                                    IndexBarcode = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    PluName = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    LastPrice = reader.GetInt32(4),
                                    NewPrice = reader.GetInt32(5),
                                    DateTime = reader.GetDateTime(6),
                                    User = reader.IsDBNull(7) ? null : reader.GetString(7)
                                });
                            }
                        }
                    }

                    return result;
                },
                cancellationToken);
        }

        #endregion

        #region Mapping

        private static SadrItem MapItem(SqlDataReader reader)
        {
            return new SadrItem
            {
                ItemClassCode = reader.GetString(0),
                PluNo = reader.GetInt32(1),
                PluUnit = GetInt32(reader, 2),
                UnitPrice = GetInt32(reader, 3),
                PrintFormat = GetInt32(reader, 4),
                PluCost = GetInt32(reader, 5),
                BarFormat = GetInt32(reader, 6),
                BarFlags = GetInt32(reader, 7),
                ItemCode = GetNullableString(reader, 8),
                IndexBarcode = GetNullableString(reader, 9),
                Tare = GetDouble(reader, 10),
                ShelfDate = GetInt32(reader, 11),
                ShelfDatePrint = GetInt32(reader, 12),
                SaleDatePrint = GetInt32(reader, 13),
                SaleTimePrint = GetInt32(reader, 14),
                OnlyTare = GetInt32(reader, 15),
                TaxRate = GetDouble(reader, 16),
                PluName = GetNullableString(reader, 17),
                Text1 = GetNullableString(reader, 18),
                Text2 = GetNullableString(reader, 19),
                Text3 = GetNullableString(reader, 20),
                Text4 = GetNullableString(reader, 21),
                Text5 = GetNullableString(reader, 22),
                Text6 = GetNullableString(reader, 23),
                Text7 = GetNullableString(reader, 24),
                DeleteFlag = GetInt32(reader, 25)
            };
        }

        private static string? GetNullableString(SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : Convert.ToString(reader.GetValue(ordinal));
        }

        private static int GetInt32(SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static double GetDouble(SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? 0d : Convert.ToDouble(reader.GetValue(ordinal));
        }

        #endregion

        #region Validation

        private static void ValidatePluNo(int pluNo)
        {
            if (pluNo == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pluNo), "PluNo must be non-zero.");
            }
        }

        private static void ValidateHistoryLimit(int maxRows)
        {
            if (maxRows < 1 || maxRows > 5000)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRows), "Price-history row limit must be between 1 and 5000.");
            }
        }

        #endregion
    }
}
