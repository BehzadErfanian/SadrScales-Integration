namespace SadrScales.Integration.Items
{
    /// <summary>
    /// Public Contract v1 model for a Sadr Scales PLU/item.
    /// </summary>
    public sealed class SadrItem
    {
        /// <summary>Gets or sets the existing item-group code.</summary>
        public string ItemClassCode { get; set; } = "0";

        /// <summary>Gets or sets the unique non-zero public PLU identity.</summary>
        public int PluNo { get; set; }

        /// <summary>Gets or sets the PLU unit code. Default is 3.</summary>
        public int PluUnit { get; set; } = 3;

        /// <summary>Gets or sets the unit price.</summary>
        public int UnitPrice { get; set; }

        /// <summary>Gets or sets the print format.</summary>
        public int PrintFormat { get; set; }

        /// <summary>Gets or sets the PLU cost.</summary>
        public int PluCost { get; set; }

        /// <summary>Gets or sets the barcode format.</summary>
        public int BarFormat { get; set; }

        /// <summary>Gets or sets barcode flags.</summary>
        public int BarFlags { get; set; }

        /// <summary>Gets or sets the optional item code.</summary>
        public string? ItemCode { get; set; } = "0";

        /// <summary>Gets or sets the optional index barcode.</summary>
        public string? IndexBarcode { get; set; } = "0";

        /// <summary>Gets or sets tare.</summary>
        public double Tare { get; set; }

        /// <summary>Gets or sets shelf-date value.</summary>
        public int ShelfDate { get; set; }

        /// <summary>Gets or sets shelf-date print flag.</summary>
        public int ShelfDatePrint { get; set; }

        /// <summary>Gets or sets sale-date print flag.</summary>
        public int SaleDatePrint { get; set; }

        /// <summary>Gets or sets sale-time print flag.</summary>
        public int SaleTimePrint { get; set; }

        /// <summary>Gets or sets only-tare flag.</summary>
        public int OnlyTare { get; set; }

        /// <summary>Gets or sets tax rate.</summary>
        public double TaxRate { get; set; }

        /// <summary>Gets or sets the optional PLU name.</summary>
        public string? PluName { get; set; }

        /// <summary>Gets or sets text line 1.</summary>
        public string? Text1 { get; set; }
        /// <summary>Gets or sets text line 2.</summary>
        public string? Text2 { get; set; }
        /// <summary>Gets or sets text line 3.</summary>
        public string? Text3 { get; set; }
        /// <summary>Gets or sets text line 4.</summary>
        public string? Text4 { get; set; }
        /// <summary>Gets or sets text line 5.</summary>
        public string? Text5 { get; set; }
        /// <summary>Gets or sets text line 6.</summary>
        public string? Text6 { get; set; }
        /// <summary>Gets or sets text line 7.</summary>
        public string? Text7 { get; set; }

        /// <summary>Gets or sets the logical delete flag.</summary>
        public int DeleteFlag { get; set; }
    }
}
