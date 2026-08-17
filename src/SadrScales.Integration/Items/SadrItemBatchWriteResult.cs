namespace SadrScales.Integration.Items
{
    /// <summary>
    /// Aggregate result of one atomic item batch upsert.
    /// </summary>
    public sealed class SadrItemBatchWriteResult
    {
        internal SadrItemBatchWriteResult(int inserted, int updated, int unchanged)
        {
            Inserted = inserted;
            Updated = updated;
            Unchanged = unchanged;
        }

        public int Inserted { get; }

        public int Updated { get; }

        public int Unchanged { get; }

        public int Total => Inserted + Updated + Unchanged;

        public bool Changed => Inserted != 0 || Updated != 0;
    }
}
