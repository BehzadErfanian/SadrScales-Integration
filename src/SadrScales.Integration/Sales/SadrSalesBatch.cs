using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SadrScales.Integration.Sales
{
    /// <summary>
    /// A read-only batch from <c>SADR_Logs</c>.
    /// </summary>
    public sealed class SadrSalesBatch
    {
        internal SadrSalesBatch(List<SadrSaleRow> rows, long inputCursor)
        {
            Rows = new ReadOnlyCollection<SadrSaleRow>(rows);
            LastReadId = rows.Count == 0 ? inputCursor : rows[rows.Count - 1].Id;
        }

        /// <summary>
        /// Gets the rows in ascending <c>SADR_Logs.ID</c> order.
        /// </summary>
        public IReadOnlyList<SadrSaleRow> Rows { get; }

        /// <summary>
        /// Gets the largest ID read in this batch, or the input cursor when the batch is empty.
        /// Persist this value only after the destination transaction succeeds.
        /// </summary>
        public long LastReadId { get; }

        /// <summary>
        /// Gets whether this batch contains at least one row.
        /// </summary>
        public bool HasRows => Rows.Count != 0;
    }
}
