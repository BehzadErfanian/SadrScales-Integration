namespace SadrScales.Integration.Scales
{
    /// <summary>
    /// Coarse SQL-visible connection status persisted by Sadr Scales 5.2.1.
    /// </summary>
    public enum SadrScaleStatus
    {
        /// <summary>
        /// The database does not currently expose a recognized Online/Offline value.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Sadr Scales last persisted the scale as offline.
        /// </summary>
        Offline = 1,

        /// <summary>
        /// Sadr Scales last persisted the scale as online.
        /// </summary>
        Online = 2
    }
}
