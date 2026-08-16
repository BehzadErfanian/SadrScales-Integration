namespace SadrScales.Integration.Items
{
    /// <summary>
    /// Describes the database effect of an SDK upsert.
    /// </summary>
    public enum SadrWriteOperation
    {
        /// <summary>No effective data changed.</summary>
        Unchanged = 0,

        /// <summary>A new row was inserted.</summary>
        Inserted = 1,

        /// <summary>An existing row was updated.</summary>
        Updated = 2
    }
}
