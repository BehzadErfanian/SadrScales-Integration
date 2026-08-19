using System;
using System.Threading;
using System.Threading.Tasks;
using SadrScales.Integration.Assignments;
using SadrScales.Integration.Contract;
using SadrScales.Integration.HotKeys;
using SadrScales.Integration.Invoices;
using SadrScales.Integration.Internal;
using SadrScales.Integration.Items;
using SadrScales.Integration.Reports;
using SadrScales.Integration.Sales;
using SadrScales.Integration.Scales;
using SadrScales.Integration.Stores;

namespace SadrScales.Integration
{
    /// <summary>
    /// Entry point for the public Sadr Scales SQL integration SDK.
    /// </summary>
    public sealed class SadrScalesClient
    {
        #region Dependencies

        private readonly SadrContractValidator _contractValidator;

        #endregion

        #region Construction

        /// <summary>
        /// Creates a client with default options.
        /// </summary>
        public SadrScalesClient(string connectionString)
            : this(new SadrScalesClientOptions(connectionString))
        {
        }

        /// <summary>
        /// Creates a client with explicit options.
        /// </summary>
        public SadrScalesClient(SadrScalesClientOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var connectionFactory = new SqlConnectionFactory(options);
            _contractValidator = new SadrContractValidator(connectionFactory, options);
            Stores = new SadrStoreClient(connectionFactory, options);
            ItemGroups = new SadrItemGroupClient(connectionFactory, options);
            Items = new SadrItemClient(connectionFactory, options);
            Sales = new SadrSalesClient(connectionFactory, options);
            Reports = new SadrReportClient(connectionFactory, options);
            Invoices = new SadrInvoiceClient(connectionFactory, options);
            Scales = new SadrScaleClient(connectionFactory, options);
            ScaleAssignments = new SadrScaleAssignmentClient(connectionFactory, options);
            ScaleMappings = new SadrScaleMappingClient(connectionFactory, options);
            HotKeys = new SadrHotKeyClient(connectionFactory, options);
        }

        #endregion

        #region Public Clients

        /// <summary>
        /// Gets store/branch operations for the SQL integration surface.
        /// </summary>
        public SadrStoreClient Stores { get; }

        /// <summary>
        /// Gets item-group operations for the SQL integration surface.
        /// </summary>
        public SadrItemGroupClient ItemGroups { get; }

        /// <summary>
        /// Gets item/PLU operations for the SQL integration surface.
        /// </summary>
        public SadrItemClient Items { get; }

        /// <summary>
        /// Gets read-only incremental feed and filtered/paged sales-query operations.
        /// </summary>
        public SadrSalesClient Sales { get; }

        /// <summary>
        /// Gets typed Daily, Scale and Item sales-report operations.
        /// </summary>
        public SadrReportClient Reports { get; }

        /// <summary>
        /// Gets structured-invoice lookup and explicit acknowledgement operations.
        /// </summary>
        public SadrInvoiceClient Invoices { get; }

        /// <summary>
        /// Gets registered-scale metadata, coarse SQL status and AutoSend resend-request operations.
        /// </summary>
        public SadrScaleClient Scales { get; }

        /// <summary>
        /// Gets canonical multi-group assignment operations for registered scales.
        /// </summary>
        public SadrScaleAssignmentClient ScaleAssignments { get; }

        /// <summary>
        /// Gets per-scale PLU/item-code and optional HotKey-position mapping operations.
        /// </summary>
        public SadrScaleMappingClient ScaleMappings { get; }

        /// <summary>
        /// Gets user-managed item-group HotKey template operations.
        /// </summary>
        public SadrHotKeyClient HotKeys { get; }

        #endregion

        #region Validation

        /// <summary>
        /// Validates that the connected database exposes the frozen basic SQL Contract v1 schema.
        /// </summary>
        /// <remarks>
        /// Vendor-Ready capability validation is additive and will be introduced separately without changing
        /// the meaning of this published v1 validation method.
        /// </remarks>
        public Task ValidateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _contractValidator.ValidateAsync(cancellationToken);
        }

        #endregion
    }
}
