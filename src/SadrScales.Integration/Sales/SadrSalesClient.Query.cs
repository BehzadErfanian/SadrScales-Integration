using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SadrScales.Integration.Sales
{
    public sealed partial class SadrSalesClient
    {
        #region Vendor-Ready Query API

        /// <summary>
        /// Reads a newest-first filtered/paged sales result without changing the destination-owned feed cursor.
        /// </summary>
        /// <remarks>
        /// Summary values cover the complete filter, not only the current page. This API is for search/report UI;
        /// use <see cref="ReadAfterAsync(long,int,CancellationToken)"/> for incremental synchronization.
        /// </remarks>
        public Task<SadrSalesPage> QueryAsync(
            SadrSalesQueryFilter? filter = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SadrSalesQueryFilter normalized = SadrSalesQuerySql.Normalize(filter);

            return _connectionFactory.ExecuteReadAsync(
                async (connection, token) =>
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        string whereClause = SadrSalesQuerySql.BuildWhereClause(command, normalized);
                        int offset = (normalized.PageNumber - 1) * normalized.PageSize;

                        command.CommandText = @"
SELECT
    COUNT_BIG(*) AS RecordCount,
    COUNT_BIG(DISTINCT CONVERT(varchar(20), DeviceNo) + ':' + CONVERT(varchar(20), FID)) AS InvoiceCount,
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
" + whereClause + @";

SELECT
    ID, DeviceNo, Identify, [DateTime], FID, SID,
    Salesman, SubID, TotalPrice, PLU, Class, Dept,
    Amount, Unit, LogType, Tax,
    Text1, Text2, Text3, Text4,
    UnitPrice, CoFID, PLUName
FROM dbo.SADR_Logs
" + whereClause + @"
ORDER BY [DateTime] DESC, ID DESC
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;";

                        command.Parameters.Add("@Offset", SqlDbType.Int).Value = offset;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = normalized.PageSize;

                        var summary = new SadrSalesSummary();
                        var rows = new List<SadrSaleRow>(normalized.PageSize);

                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                summary.RecordCount = Convert.ToInt64(reader["RecordCount"]);
                                summary.InvoiceCount = Convert.ToInt64(reader["InvoiceCount"]);
                                summary.TotalPrice = Convert.ToDecimal(reader["TotalPrice"]);
                                summary.TotalWeight = Convert.ToDecimal(reader["TotalWeight"]);
                                summary.TotalQuantity = Convert.ToDecimal(reader["TotalQuantity"]);
                            }

                            if (await reader.NextResultAsync(token).ConfigureAwait(false))
                            {
                                while (await reader.ReadAsync(token).ConfigureAwait(false))
                                {
                                    rows.Add(Map(reader));
                                }
                            }
                        }

                        int pageCount = summary.RecordCount == 0
                            ? 1
                            : (int)Math.Ceiling(summary.RecordCount / (decimal)normalized.PageSize);

                        return new SadrSalesPage(
                            rows,
                            summary,
                            normalized.PageNumber,
                            normalized.PageSize,
                            pageCount);
                    }
                },
                cancellationToken);
        }

        #endregion
    }
}
