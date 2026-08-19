namespace SadrScales.Integration.Scales
{
    /// <summary>
    /// Public, SQL-readable scale registration metadata from Sadr Scales 5.2.1.
    /// </summary>
    public sealed class SadrScale
    {
        #region Identity

        /// <summary>
        /// Gets or sets the scale number/ID. Supported Sadr Scales values are 1 through 99.
        /// </summary>
        public int ScaleId { get; set; }

        /// <summary>
        /// Gets or sets the registered scale IP address.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the registered TCP port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the registered scale model/category, for example LSG or PLUS.
        /// </summary>
        public string? Model { get; set; }

        #endregion

        #region Display And Assignment

        /// <summary>
        /// Gets or sets the user-visible registered scale name.
        /// </summary>
        public string? DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the store code currently persisted for the scale.
        /// </summary>
        public string? StoreCode { get; set; }

        /// <summary>
        /// Gets or sets the store name currently persisted for compatibility/display purposes.
        /// </summary>
        public string? StoreName { get; set; }

        /// <summary>
        /// Gets or sets the legacy primary item-group code stored on the scale row.
        /// Canonical multi-group assignment is exposed separately in a later Vendor-Ready slice.
        /// </summary>
        public string? PrimaryItemGroupCode { get; set; }

        #endregion

        #region Runtime Configuration

        /// <summary>
        /// Gets or sets whether the scale is enabled in Sadr Scales.
        /// </summary>
        public bool Used { get; set; }

        /// <summary>
        /// Gets or sets whether automatic item sending is enabled.
        /// </summary>
        public bool AutoSendItems { get; set; }

        /// <summary>
        /// Gets or sets whether automatic invoice retrieval is enabled.
        /// </summary>
        public bool AutoGetInvoice { get; set; }

        /// <summary>
        /// Gets or sets the coarse Online/Offline status persisted by Sadr Scales.
        /// </summary>
        public SadrScaleStatus Status { get; set; }

        #endregion

        #region Device Metadata

        /// <summary>
        /// Gets or sets the MAC address when available.
        /// </summary>
        public string? Mac { get; set; }

        /// <summary>
        /// Gets or sets the device-reported/software version when available.
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Gets or sets the configured number of hot keys per page.
        /// </summary>
        public int HotKeyCountPerPage { get; set; }

        /// <summary>
        /// Gets or sets the configured hot-key page count.
        /// </summary>
        public int HotKeyPageCount { get; set; }

        #endregion
    }
}
