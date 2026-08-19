namespace SadrScales.Integration.Sales
{
    /// <summary>
    /// Aggregate totals for the complete filtered sales result, independent of the current page.
    /// </summary>
    public sealed class SadrSalesSummary
    {
        /// <summary>Gets the number of matching SADR_Logs rows.</summary>
        public long RecordCount { get; internal set; }

        /// <summary>Gets the number of distinct invoices, identified by Scale/DeviceNo + FID.</summary>
        public long InvoiceCount { get; internal set; }

        /// <summary>Gets the sum of matching row TotalPrice values.</summary>
        public decimal TotalPrice { get; internal set; }

        /// <summary>Gets the sum of Amount for unit codes 0, 1 and 3.</summary>
        public decimal TotalWeight { get; internal set; }

        /// <summary>Gets the sum of Amount for unit code 2.</summary>
        public decimal TotalQuantity { get; internal set; }
    }
}
