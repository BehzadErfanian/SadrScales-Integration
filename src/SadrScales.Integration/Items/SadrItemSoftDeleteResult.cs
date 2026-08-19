namespace SadrScales.Integration.Items
{
    /// <summary>
    /// Outcome of a logical item delete using the Sadr Scales DeleteFlag contract.
    /// </summary>
    public enum SadrItemSoftDeleteResult
    {
        /// <summary>The PLU does not exist.</summary>
        NotFound = 0,

        /// <summary>The PLU was marked as deleted.</summary>
        Deleted = 1,

        /// <summary>The PLU was already logically deleted.</summary>
        AlreadyDeleted = 2
    }
}
