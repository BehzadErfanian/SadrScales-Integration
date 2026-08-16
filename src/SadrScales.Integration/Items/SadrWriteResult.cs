namespace SadrScales.Integration.Items
{
    /// <summary>
    /// Result of a Contract v1 upsert operation.
    /// </summary>
    public sealed class SadrWriteResult
    {
        internal SadrWriteResult(SadrWriteOperation operation)
        {
            Operation = operation;
        }

        /// <summary>
        /// Gets the effective database operation.
        /// </summary>
        public SadrWriteOperation Operation { get; }

        /// <summary>
        /// Gets whether the operation changed persisted data.
        /// </summary>
        public bool Changed => Operation != SadrWriteOperation.Unchanged;
    }
}
