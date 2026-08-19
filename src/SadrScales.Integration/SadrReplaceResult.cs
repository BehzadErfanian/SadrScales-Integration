namespace SadrScales.Integration
{
    /// <summary>
    /// Common result for public replace-style configuration operations.
    /// </summary>
    public enum SadrReplaceResult
    {
        /// <summary>The target scale/group does not exist.</summary>
        NotFound = 0,

        /// <summary>The requested configuration already matched the persisted configuration.</summary>
        Unchanged = 1,

        /// <summary>The persisted configuration was atomically replaced.</summary>
        Replaced = 2
    }
}
