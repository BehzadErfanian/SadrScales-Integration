using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SadrScales.Integration.Internal
{
    internal sealed class SqlConnectionFactory
    {
        private readonly SadrScalesClientOptions _options;

        public SqlConnectionFactory(SadrScalesClientOptions options)
        {
            _options = options;
        }

        public async Task<SqlConnection> OpenAsync(CancellationToken cancellationToken)
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
