using System;
using System.Collections.Generic;
using SadrScales.Integration.Assignments;
using SadrScales.Integration.HotKeys;
using SadrScales.Integration.Items;
using SadrScales.Integration.Stores;

namespace SadrScales.Integration.DemoLab
{
    /// <summary>
    /// Deterministic synthetic dataset used only by the Developer Sample and acceptance tests.
    /// </summary>
    public sealed class SadrDemoScenario
    {
        internal SadrDemoScenario(int seed, DateTime referenceDate)
        {
            Seed = seed;
            ReferenceDate = referenceDate;
        }

        /// <summary>Gets the seed that fully determines this scenario.</summary>
        public int Seed { get; }

        /// <summary>Gets the deterministic reference date derived from the seed.</summary>
        public DateTime ReferenceDate { get; }

        /// <summary>Gets generated demo stores.</summary>
        public List<SadrStore> Stores { get; } = new List<SadrStore>();

        /// <summary>Gets generated demo item groups.</summary>
        public List<SadrItemGroup> Groups { get; } = new List<SadrItemGroup>();

        /// <summary>Gets generated demo items.</summary>
        public List<SadrItem> Items { get; } = new List<SadrItem>();

        /// <summary>Gets generated synthetic registered-scale rows.</summary>
        public List<SadrDemoScale> Scales { get; } = new List<SadrDemoScale>();

        /// <summary>Gets canonical per-scale group assignments.</summary>
        public Dictionary<int, List<string>> ScaleAssignments { get; } =
            new Dictionary<int, List<string>>();

        /// <summary>Gets per-scale item mappings.</summary>
        public Dictionary<int, List<SadrScaleItemMap>> ScaleMappings { get; } =
            new Dictionary<int, List<SadrScaleItemMap>>();

        /// <summary>Gets user-managed group HotKey templates.</summary>
        public Dictionary<string, List<SadrHotKey>> GroupHotKeys { get; } =
            new Dictionary<string, List<SadrHotKey>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Gets synthetic invoices/sales used to exercise Invoice, Sales Query and Reports.</summary>
        public List<SadrDemoInvoice> Invoices { get; } = new List<SadrDemoInvoice>();
    }

    /// <summary>Options for reproducible Demo Data generation.</summary>
    public sealed class SadrDemoScenarioOptions
    {
        /// <summary>Gets or sets the deterministic seed.</summary>
        public int Seed { get; set; } = 12345;

        /// <summary>Gets or sets the number of generated stores.</summary>
        public int StoreCount { get; set; } = 3;

        /// <summary>Gets or sets the number of generated item groups.</summary>
        public int GroupCount { get; set; } = 5;

        /// <summary>Gets or sets the number of generated items.</summary>
        public int ItemCount { get; set; } = 100;

        /// <summary>Gets or sets the number of synthetic registered scales.</summary>
        public int ScaleCount { get; set; } = 10;

        /// <summary>Gets or sets the number of synthetic invoices/sales.</summary>
        public int InvoiceCount { get; set; } = 20;
    }

    /// <summary>
    /// Demo-only scale definition. Scale creation is not a production 5.2.1 Integration API.
    /// </summary>
    public sealed class SadrDemoScale
    {
        public int ScaleId { get; set; }
        public string Model { get; set; } = "LSG";
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; } = 5000;
        public string Mac { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string StoreCode { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string PrimaryItemGroupCode { get; set; } = string.Empty;
        public int HotKeyCountPerPage { get; set; } = 40;
        public int HotKeyPageCount { get; set; } = 3;
    }

    /// <summary>One synthetic structured invoice plus its sales rows.</summary>
    public sealed class SadrDemoInvoice
    {
        public int ScaleId { get; set; }
        public int Fid { get; set; }
        public DateTime SaleDateTime { get; set; }
        public string TotalBarcode { get; set; } = string.Empty;
        public string ItemBarcode { get; set; } = string.Empty;
        public bool IsAcknowledged { get; set; }
        public List<SadrDemoInvoiceLine> Lines { get; } = new List<SadrDemoInvoiceLine>();
    }

    /// <summary>One synthetic line shared by structured invoice and SADR_Logs demo data.</summary>
    public sealed class SadrDemoInvoiceLine
    {
        public int SubId { get; set; }
        public int PluNo { get; set; }
        public string PluName { get; set; } = string.Empty;
        public int Unit { get; set; }
        public decimal Amount { get; set; }
        public int UnitPrice { get; set; }
        public int TotalPrice { get; set; }
    }
}
