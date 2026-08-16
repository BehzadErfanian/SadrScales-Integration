namespace SadrScales.Integration.Items
{
    /// <summary>
    /// Public Contract v1 model for <c>dbo.SADR_ItemClass</c>.
    /// </summary>
    public sealed class SadrItemGroup
    {
        /// <summary>Gets or sets the required group code.</summary>
        public string ItemClassCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional group name.</summary>
        public string? ItemClassName { get; set; }

        /// <summary>Gets or sets the optional group description.</summary>
        public string? Descriptions { get; set; }
    }
}
