using System;
using System.Threading;
using System.Threading.Tasks;
using SadrScales.Integration.Contract;
using SadrScales.Integration.Invoices;
using SadrScales.Integration.Internal;
using SadrScales.Integration.Items;
using SadrScales.Integration.Sales;

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
            ItemGroups = new SadrItemGroupClient(connectionFactory, options);
            Items = new SadrItemClient(connectionFactory, options);
            Sales = new SadrSalesClient(connectionFactory, options);
            Invoices = new SadrInvoiceClient(connectionFactory, options);
        }

        #endregion

        #region Public Clients

        /// <summary>
        /// Gets item-group operations for the SQL integration surface.
        /// </summary>
        public SadrItemGroupClient ItemGroups { get; }

        /// <summary>
        /// Gets item/PLU operations for the SQL integration surface.
        /// </summary>
        public SadrItemClient Items { get; }

        /// <summary>
        /// Gets read-only sales-feed operations for the SQL integration surface.
        /// </summary>
        public SadrSalesClient Sales { get; }

        /// <summary>
        /// Gets structured-invoice lookup and explicit acknowledgement operations.
        /// </summary>
        public SadrInvoiceClient Invoices { get; }

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
