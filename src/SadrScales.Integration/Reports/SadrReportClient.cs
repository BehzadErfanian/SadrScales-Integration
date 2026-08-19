using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration.Internal;
using SadrScales.Integration.Sales;

namespace SadrScales.Integration.Reports
{
    /// <summary>
    /// Typed read-only sales reports matching the Daily / Scale / Item report semantics in Sadr Scales 5.2.1.
    /// </summary>
    public sealed class SadrReportClient
    {
        /// <summary>Maximum number of rows returned by the per-item report, matching Sadr Scales 5.2.1.</summary>
        public const int MaximumItemReportRows = 5000;

        #region Dependencies

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly SadrScalesClientOptions _options;

        #endregion

        #region Construction

        internal SadrReportClient(SqlConnectionFactory connectionFactory, SadrScalesClientOptions options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
        }

        #endregion

        #region Daily Report

        /// <summary>
        /// Aggregates the filtered sales rows by Gregorian sale date, newest date first.
        /// </summary>
        public Task<IReadOnlyList<SadrDailySalesReportRow>> GetDailyAsync(
            SadrSalesQueryFilter? filter = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SadrSalesQueryFilter normalized = SadrSalesQuerySql.Normalize(filter);

            return _connectionFactory.ExecuteReadAsync<IReadOnlyList<SadrDailySalesReportRow>>(
                async (connection, token) =>
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        string whereClause = SadrSalesQuerySql.BuildWhereClause(command, normalized);
                        command.CommandText = @"
SELECT
    CONVERT(date, [DateTime]) AS SaleDate,
    COUNT_BIG(*) AS RecordCount,
    COUNT(DISTINCT CONVERT(varchar(20), DeviceNo) + ':' + CONVERT(varchar(20), FID)) AS InvoiceCount,
    ISNULL(SUM(CONVERT(decimal(38, 3), TotalPrice)), 0) AS TotalPrice,
    ISNULL(SUM(CASE [Unit]
                  WHEN 0 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 1 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 3 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalWeight,
    ISNULL(SUM(CASE [Unit]
                  WHEN 2 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalQuantity
FROM dbo.SADR_Logs
" + whereClause + @"
GROUP BY CONVERT(date, [DateTime])
ORDER BY SaleDate DESC;";

                        var rows = new List<SadrDailySalesReportRow>();
                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                rows.Add(new SadrDailySalesReportRow
                                {
                                    SaleDate = Convert.ToDateTime(reader["SaleDate"]),
                                    Summary = ReadSummary(reader)
                                });
                            }
                        }

                        return rows;
                    }
                },
                cancellationToken);
        }

        #endregion

        #region Scale Report

        /// <summary>
        /// Aggregates the filtered sales rows by Scale ID / DeviceNo, highest total price first.
        /// </summary>
        public Task<IReadOnlyList<SadrScaleSalesReportRow>> GetByScaleAsync(
            SadrSalesQueryFilter? filter = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SadrSalesQueryFilter normalized = SadrSalesQuerySql.Normalize(filter);

            return _connectionFactory.ExecuteReadAsync<IReadOnlyList<SadrScaleSalesReportRow>>(
                async (connection, token) =>
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        string whereClause = SadrSalesQuerySql.BuildWhereClause(command, normalized);
                        command.CommandText = @"
SELECT
    DeviceNo,
    MAX(ISNULL(Identify, N'')) AS Identify,
    COUNT_BIG(*) AS RecordCount,
    COUNT(DISTINCT CONVERT(varchar(20), DeviceNo) + ':' + CONVERT(varchar(20), FID)) AS InvoiceCount,
    ISNULL(SUM(CONVERT(decimal(38, 3), TotalPrice)), 0) AS TotalPrice,
    ISNULL(SUM(CASE [Unit]
                  WHEN 0 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 1 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 3 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalWeight,
    ISNULL(SUM(CASE [Unit]
                  WHEN 2 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalQuantity
FROM dbo.SADR_Logs
" + whereClause + @"
GROUP BY DeviceNo
ORDER BY TotalPrice DESC, DeviceNo;";

                        var rows = new List<SadrScaleSalesReportRow>();
                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                rows.Add(new SadrScaleSalesReportRow
                                {
                                    ScaleId = Convert.ToInt32(reader["DeviceNo"]),
                                    Identify = Convert.ToString(reader["Identify"]) ?? string.Empty,
                                    Summary = ReadSummary(reader)
                                });
                            }
                        }

                        return rows;
                    }
                },
                cancellationToken);
        }

        #endregion

        #region Item Report

        /// <summary>
        /// Aggregates the filtered sales rows by PLU, highest total price first, capped at 5000 rows.
        /// </summary>
        public Task<IReadOnlyList<SadrItemSalesReportRow>> GetByItemAsync(
            SadrSalesQueryFilter? filter = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SadrSalesQueryFilter normalized = SadrSalesQuerySql.Normalize(filter);

            return _connectionFactory.ExecuteReadAsync<IReadOnlyList<SadrItemSalesReportRow>>(
                async (connection, token) =>
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        string whereClause = SadrSalesQuerySql.BuildWhereClause(command, normalized);
                        command.CommandText = @"
SELECT TOP (5000)
    PLU,
    MAX(ISNULL(PLUName, N'')) AS PLUName,
    COUNT_BIG(*) AS RecordCount,
    COUNT(DISTINCT CONVERT(varchar(20), DeviceNo) + ':' + CONVERT(varchar(20), FID)) AS InvoiceCount,
    ISNULL(SUM(CONVERT(decimal(38, 3), TotalPrice)), 0) AS TotalPrice,
    ISNULL(SUM(CASE [Unit]
                  WHEN 0 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 1 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 3 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalWeight,
    ISNULL(SUM(CASE [Unit]
                  WHEN 2 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalQuantity
FROM dbo.SADR_Logs
" + whereClause + @"
GROUP BY PLU
ORDER BY TotalPrice DESC, PLU;";

                        var rows = new List<SadrItemSalesReportRow>();
                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                rows.Add(new SadrItemSalesReportRow
                                {
                                    Plu = Convert.ToInt32(reader["PLU"]),
                                    PluName = Convert.ToString(reader["PLUName"]) ?? string.Empty,
                                    Summary = ReadSummary(reader)
                                });
                            }
                        }

                        return rows;
                    }
                },
                cancellationToken);
        }

        #endregion

        #region Mapping

        private static SadrSalesSummary ReadSummary(SqlDataReader reader)
        {
            return new SadrSalesSummary
            {
                RecordCount = Convert.ToInt64(reader["RecordCount"]),
                InvoiceCount = Convert.ToInt64(reader["InvoiceCount"]),
                TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),
                TotalWeight = Convert.ToDecimal(reader["TotalWeight"]),
                TotalQuantity = Convert.ToDecimal(reader["TotalQuantity"])
            };
        }

        #endregion
    }
}
