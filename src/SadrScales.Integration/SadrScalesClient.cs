using System;
using System.Threading;
using System.Threading.Tasks;
using SadrScales.Integration.Contract;
using SadrScales.Integration.Internal;
using SadrScales.Integration.Items;
using SadrScales.Integration.Sales;

namespace SadrScales.Integration
{
    /// <summary>
    /// Entry point for the basic Sadr Scales SQL Contract v1 integration API.
    /// </summary>
    public sealed class SadrScalesClient
    {
        private readonly SadrContractValidator _contractValidator;

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
        }

        /// <summary>
        /// Gets item-group operations for the basic Contract v1 surface.
        /// </summary>
        public SadrItemGroupClient ItemGroups { get; }

        /// <summary>
        /// Gets item/PLU operations for the basic Contract v1 surface.
        /// </summary>
        public SadrItemClient Items { get; }

        /// <summary>
        /// Gets read-only sales-feed operations for the basic Contract v1 surface.
        /// </summary>
        public SadrSalesClient Sales { get; }

        /// <summary>
        /// Validates that the connected database exposes the frozen basic SQL Contract v1 schema.
        /// </summary>
        public Task ValidateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _contractValidator.ValidateAsync(cancellationToken);
        }
    }
}
