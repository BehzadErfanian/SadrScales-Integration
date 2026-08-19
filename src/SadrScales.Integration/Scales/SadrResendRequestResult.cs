namespace SadrScales.Integration.Scales
{
    /// <summary>
    /// Result of recording an AutoSend resend request in the Sadr Scales SQL state.
    /// </summary>
    public enum SadrResendRequestResult
    {
        /// <summary>
        /// The target scale was not found.
        /// </summary>
        NotFound = 0,

        /// <summary>
        /// The resend request was recorded successfully.
        /// This does not mean the physical scale has already received the data.
        /// </summary>
        Requested = 1,

        /// <summary>
        /// The registered scale model does not support this automatic resend capability.
        /// </summary>
        UnsupportedModel = 2
    }
}
