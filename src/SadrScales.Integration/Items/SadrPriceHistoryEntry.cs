using System;

namespace SadrScales.Integration.Items
{
    /// <summary>
    /// Read-only price-history entry persisted by Sadr Scales.
    /// </summary>
    public sealed class SadrPriceHistoryEntry
    {
        /// <summary>Gets or sets the database history identity.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the PLU identity.</summary>
        public int PluNo { get; set; }

        /// <summary>Gets or sets the item index barcode captured with the change.</summary>
        public string? IndexBarcode { get; set; }

        /// <summary>Gets or sets the item name captured with the change.</summary>
        public string? PluName { get; set; }

        /// <summary>Gets or sets the previous unit price.</summary>
        public int LastPrice { get; set; }

        /// <summary>Gets or sets the new unit price.</summary>
        public int NewPrice { get; set; }

        /// <summary>Gets or sets the recorded change time.</summary>
        public DateTime DateTime { get; set; }

        /// <summary>Gets or sets the recorded user/source text when available.</summary>
        public string? User { get; set; }
    }
}
