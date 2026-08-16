using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SadrScales.Integration.Internal
{
    internal sealed class SqlConnectionFactory
    {
        private readonly SadrScalesClientOptions _options;
        private readonly SqlTransientRetryPolicy _retryPolicy;

        public SqlConnectionFactory(SadrScalesClientOptions options)
        {
            _options = options;
            _retryPolicy = new SqlTransientRetryPolicy(options);
        }

        /// <summary>
        /// Opens a connection with bounded transient retry. Retrying connection establishment is safe because
        /// no SQL operation or transaction has started yet.
        /// </summary>
        public Task<SqlConnection> OpenAsync(CancellationToken cancellationToken)
        {
            return _retryPolicy.ExecuteAsync(OpenOnceAsync, cancellationToken);
        }

        /// <summary>
        /// Re-executes a complete read-only operation on a fresh connection after a recognized transient failure.
        /// Transactional write operations intentionally do not use this method.
        /// </summary>
        public Task<T> ExecuteReadAsync<T>(
            Func<SqlConnection, CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            return _retryPolicy.ExecuteAsync(
                async token =>
                {
                    using (var connection = await OpenOnceAsync(token).ConfigureAwait(false))
                    {
                        return await operation(connection, token).ConfigureAwait(false);
                    }
                },
                cancellationToken);
        }

        private async Task<SqlConnection> OpenOnceAsync(CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(_options.ConnectionString);
            try
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }
    }
}
