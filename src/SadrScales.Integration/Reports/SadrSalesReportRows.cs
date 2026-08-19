using System;
using SadrScales.Integration.Sales;

namespace SadrScales.Integration.Reports
{
    /// <summary>One daily sales-report aggregate.</summary>
    public sealed class SadrDailySalesReportRow
    {
        /// <summary>Gets the Gregorian sale date represented by this aggregate.</summary>
        public DateTime SaleDate { get; internal set; }
        /// <summary>Gets the aggregate totals.</summary>
        public SadrSalesSummary Summary { get; internal set; } = new SadrSalesSummary();
    }

    /// <summary>One per-scale sales-report aggregate.</summary>
    public sealed class SadrScaleSalesReportRow
    {
        /// <summary>Gets the registered Scale ID / DeviceNo.</summary>
        public int ScaleId { get; internal set; }
        /// <summary>Gets the latest/non-empty Identify value represented by the source aggregate.</summary>
        public string Identify { get; internal set; } = string.Empty;
        /// <summary>Gets the aggregate totals.</summary>
        public SadrSalesSummary Summary { get; internal set; } = new SadrSalesSummary();
    }

    /// <summary>One per-item sales-report aggregate.</summary>
    public sealed class SadrItemSalesReportRow
    {
        /// <summary>Gets the PLU number.</summary>
        public int Plu { get; internal set; }
        /// <summary>Gets the item name represented by the source aggregate.</summary>
        public string PluName { get; internal set; } = string.Empty;
        /// <summary>Gets the aggregate totals.</summary>
        public SadrSalesSummary Summary { get; internal set; } = new SadrSalesSummary();
    }
}
