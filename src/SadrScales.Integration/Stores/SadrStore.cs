namespace SadrScales.Integration.Stores
{
    /// <summary>
    /// Public store/branch model from the Sadr Scales 5.2.1 SQL catalog.
    /// </summary>
    public sealed class SadrStore
    {
        /// <summary>Gets or sets the stable public store code.</summary>
        public string StoreCode { get; set; } = "0";

        /// <summary>Gets or sets the store display name.</summary>
        public string? StoreName { get; set; }

        /// <summary>Gets or sets optional store descriptions.</summary>
        public string? Descriptions { get; set; }
    }
}
