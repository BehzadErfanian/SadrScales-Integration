using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SadrScales.Integration.Sales
{
    /// <summary>
    /// Shared SQL filter construction so Sales.Query and Reports use exactly the same 5.2.1 semantics.
    /// </summary>
    internal static class SadrSalesQuerySql
    {
        internal const int MinimumPageSize = 50;
        internal const int MaximumPageSize = 2000;

        internal static SadrSalesQueryFilter Normalize(SadrSalesQueryFilter? filter)
        {
            SadrSalesQueryFilter normalized = filter == null
                ? new SadrSalesQueryFilter()
                : filter.Clone();

            normalized.PageNumber = Math.Max(1, normalized.PageNumber);
            normalized.PageSize = Math.Max(MinimumPageSize, Math.Min(MaximumPageSize, normalized.PageSize));
            normalized.Identify = string.IsNullOrWhiteSpace(normalized.Identify)
                ? null
                : normalized.Identify.Trim();

            if (normalized.StartDateInclusive.HasValue &&
                normalized.EndDateExclusive.HasValue &&
                normalized.EndDateExclusive.Value <= normalized.StartDateInclusive.Value)
            {
                normalized.EndDateExclusive = normalized.StartDateInclusive.Value.Date.AddDays(1);
            }

            return normalized;
        }

        internal static string BuildWhereClause(SqlCommand command, SadrSalesQueryFilter filter)
        {
            var conditions = new List<string>();

            if (filter.StartDateInclusive.HasValue)
            {
                conditions.Add("[DateTime] >= @StartDate");
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = filter.StartDateInclusive.Value;
            }

            if (filter.EndDateExclusive.HasValue)
            {
                conditions.Add("[DateTime] < @EndDate");
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = filter.EndDateExclusive.Value;
            }

            if (!string.IsNullOrWhiteSpace(filter.Identify))
            {
                conditions.Add("Identify = @Identify");
                command.Parameters.Add("@Identify", SqlDbType.NVarChar, 50).Value = filter.Identify;
            }

            if (filter.Plu.HasValue)
            {
                conditions.Add("PLU = @Plu");
                command.Parameters.Add("@Plu", SqlDbType.Int).Value = filter.Plu.Value;
            }

            if (filter.ScaleId.HasValue)
            {
                conditions.Add("DeviceNo = @ScaleId");
                command.Parameters.Add("@ScaleId", SqlDbType.Int).Value = filter.ScaleId.Value;
            }

            if (filter.Fid.HasValue)
            {
                conditions.Add("FID = @Fid");
                command.Parameters.Add("@Fid", SqlDbType.Int).Value = filter.Fid.Value;
            }

            return conditions.Count == 0
                ? string.Empty
                : "WHERE " + string.Join(" AND ", conditions);
        }
    }
}
