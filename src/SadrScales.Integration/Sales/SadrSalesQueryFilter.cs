using System;

namespace SadrScales.Integration.Sales
{
    /// <summary>
    /// Filter and paging options for read-only sales query/report operations.
    /// </summary>
    public sealed class SadrSalesQueryFilter
    {
        /// <summary>Gets or sets the optional inclusive start date/time.</summary>
        public DateTime? StartDateInclusive { get; set; }

        /// <summary>Gets or sets the optional exclusive end date/time.</summary>
        public DateTime? EndDateExclusive { get; set; }

        /// <summary>Gets or sets the optional exact SADR_Logs Identify value.</summary>
        public string? Identify { get; set; }

        /// <summary>Gets or sets the optional PLU filter.</summary>
        public int? Plu { get; set; }

        /// <summary>Gets or sets the optional registered Scale ID / DeviceNo filter.</summary>
        public int? ScaleId { get; set; }

        /// <summary>Gets or sets the optional invoice FID filter.</summary>
        public int? Fid { get; set; }

        /// <summary>Gets or sets the one-based page number. Values below one normalize to one.</summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the requested page size. Sadr Scales 5.2.1 semantics clamp this value to 50..2000.
        /// </summary>
        public int PageSize { get; set; } = 200;

        internal SadrSalesQueryFilter Clone()
        {
            return new SadrSalesQueryFilter
            {
                StartDateInclusive = StartDateInclusive,
                EndDateExclusive = EndDateExclusive,
                Identify = Identify,
                Plu = Plu,
                ScaleId = ScaleId,
                Fid = Fid,
                PageNumber = PageNumber,
                PageSize = PageSize
            };
        }
    }
}
