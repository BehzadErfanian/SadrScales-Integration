namespace SadrScales.Integration.Stores
{
    /// <summary>
    /// Outcome of a semantic store upsert.
    /// </summary>
    public enum SadrStoreUpsertResult
    {
        /// <summary>The existing row already had the requested values.</summary>
        Unchanged = 0,

        /// <summary>A new store row was inserted.</summary>
        Inserted = 1,

        /// <summary>An existing store row was changed.</summary>
        Updated = 2
    }
}
