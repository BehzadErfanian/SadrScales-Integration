namespace SadrScales.Integration.HotKeys
{
    /// <summary>
    /// User-managed HotKey entry in an item-group template.
    /// </summary>
    public sealed class SadrHotKey
    {
        /// <summary>Gets or sets the zero-based HotKey page.</summary>
        public int PageNo { get; set; }

        /// <summary>Gets or sets the positive key position within the page.</summary>
        public int KeyNo { get; set; }

        /// <summary>Gets or sets the positive catalog PLU assigned to this key.</summary>
        public int PluNo { get; set; }
    }
}
