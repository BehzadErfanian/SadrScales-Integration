namespace SadrScales.Integration.Assignments
{
    /// <summary>
    /// Public per-scale item mapping used when a registered scale has its own PLU/item-code layout.
    /// </summary>
    /// <remarks>
    /// <para><see cref="PluNo"/> identifies the catalog item.</para>
    /// <para><see cref="ItemCode"/> is the scale-specific positive item code.</para>
    /// <para><see cref="PageNo"/> and <see cref="KeyNo"/> are either both null or both populated.</para>
    /// </remarks>
    public sealed class SadrScaleItemMap
    {
        /// <summary>Gets or sets the owning Scale ID. Zero is accepted when supplying an item to ReplaceAsync.</summary>
        public int ScaleId { get; set; }

        /// <summary>Gets or sets the existing positive PLU number.</summary>
        public int PluNo { get; set; }

        /// <summary>Gets or sets the positive scale-specific item code.</summary>
        public int ItemCode { get; set; }

        /// <summary>Gets or sets the optional zero-based HotKey page.</summary>
        public int? PageNo { get; set; }

        /// <summary>Gets or sets the optional one-based HotKey position within the page.</summary>
        public int? KeyNo { get; set; }
    }
}
