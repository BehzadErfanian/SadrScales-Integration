using System.Collections.Generic;

namespace SadrScales.Integration.Sales
{
    /// <summary>
    /// One newest-first page of filtered sales rows plus totals for the complete filtered result.
    /// </summary>
    public sealed class SadrSalesPage
    {
        internal SadrSalesPage(
            IReadOnlyList<SadrSaleRow> rows,
            SadrSalesSummary summary,
            int pageNumber,
            int pageSize,
            int pageCount)
        {
            Rows = rows;
            Summary = summary;
            PageNumber = pageNumber;
            PageSize = pageSize;
            PageCount = pageCount;
        }

        /// <summary>Gets the rows on the requested page, ordered by DateTime DESC then ID DESC.</summary>
        public IReadOnlyList<SadrSaleRow> Rows { get; }

        /// <summary>Gets aggregate totals for the complete filter, not only this page.</summary>
        public SadrSalesSummary Summary { get; }

        /// <summary>Gets the normalized one-based page number.</summary>
        public int PageNumber { get; }

        /// <summary>Gets the normalized page size.</summary>
        public int PageSize { get; }

        /// <summary>Gets the total page count. Empty results report one page, matching 5.2.1 behavior.</summary>
        public int PageCount { get; }
    }
}
