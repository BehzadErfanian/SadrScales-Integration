using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration.Internal;

namespace SadrScales.Integration.Stores
{
    /// <summary>
    /// Public Sadr Scales store/branch catalog operations.
    /// </summary>
    public sealed class SadrStoreClient
    {
        #region SQL

        private const string UpsertSql = @"
IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SADR_Store WITH (UPDLOCK, HOLDLOCK)
    WHERE StoreCode = @StoreCode
)
BEGIN
    INSERT INTO dbo.SADR_Store(StoreCode, StoreName, Descriptions)
    VALUES(@StoreCode, @StoreName, @Descriptions);
    SELECT CAST(1 AS int);
END
ELSE
BEGIN
    UPDATE dbo.SADR_Store
    SET StoreName = @StoreName,
        Descriptions = @Descriptions
    WHERE StoreCode = @StoreCode
      AND EXISTS
      (
          SELECT StoreName, Descriptions
          EXCEPT
          SELECT @StoreName, @Descriptions
      );

    DECLARE @Rows int = @@ROWCOUNT;
    SELECT CASE WHEN @Rows = 0 THEN 0 ELSE 2 END;
END;";

        #endregion

        #region Dependencies

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly SadrScalesClientOptions _options;

        #endregion

        #region Construction

        internal SadrStoreClient(SqlConnectionFactory connectionFactory, SadrScalesClientOptions options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
        }

        #endregion

        #region Read API

        /// <summary>
        /// Reads all stores ordered by their stable StoreCode identity.
        /// </summary>
        public Task<IReadOnlyList<SadrStore>> GetAllAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _connectionFactory.ExecuteReadAsync<IReadOnlyList<SadrStore>>(
                async (connection, token) =>
                {
                    var result = new List<SadrStore>();
                    const string sql = @"
SELECT StoreCode, StoreName, Descriptions
FROM dbo.SADR_Store
ORDER BY StoreCode ASC;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                result.Add(Map(reader));
                            }
                        }
                    }

                    return result;
                },
                cancellationToken);
        }

        /// <summary>
        /// Reads one store by StoreCode, or returns null when it does not exist.
        /// </summary>
        public Task<SadrStore?> GetAsync(
            string storeCode,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateStoreCode(storeCode);

            return _connectionFactory.ExecuteReadAsync<SadrStore?>(
                async (connection, token) =>
                {
                    const string sql = @"
SELECT StoreCode, StoreName, Descriptions
FROM dbo.SADR_Store
WHERE StoreCode = @StoreCode;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        command.Parameters.Add("@StoreCode", SqlDbType.VarChar, 50).Value = storeCode;

                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            if (!await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                return null;
                            }

                            return Map(reader);
                        }
                    }
                },
                cancellationToken);
        }

        #endregion

        #region Write API

        /// <summary>
        /// Inserts a store or updates it only when its semantic values changed.
        /// </summary>
        public async Task<SadrStoreUpsertResult> UpsertAsync(
            SadrStore store,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Validate(store);

            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            using (var command = new SqlCommand(UpsertSql, connection, transaction))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@StoreCode", SqlDbType.VarChar, 50).Value = store.StoreCode;
                command.Parameters.Add("@StoreName", SqlDbType.NVarChar, 100).Value = (object?)store.StoreName ?? DBNull.Value;
                command.Parameters.Add("@Descriptions", SqlDbType.NVarChar, 150).Value = (object?)store.Descriptions ?? DBNull.Value;

                try
                {
                    var scalar = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                    return (SadrStoreUpsertResult)Convert.ToInt32(scalar);
                }
                catch
                {
                    TryRollback(transaction);
                    throw;
                }
            }
        }

        #endregion

        #region Mapping

        private static SadrStore Map(SqlDataReader reader)
        {
            return new SadrStore
            {
                StoreCode = reader.GetString(0),
                StoreName = reader.IsDBNull(1) ? null : reader.GetString(1),
                Descriptions = reader.IsDBNull(2) ? null : reader.GetString(2)
            };
        }

        #endregion

        #region Validation

        private static void Validate(SadrStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            ValidateStoreCode(store.StoreCode);
            ValidateLength(store.StoreName, 100, nameof(store.StoreName));
            ValidateLength(store.Descriptions, 150, nameof(store.Descriptions));
        }

        private static void ValidateStoreCode(string storeCode)
        {
            if (string.IsNullOrWhiteSpace(storeCode))
            {
                throw new ArgumentException("StoreCode is required.", nameof(storeCode));
            }

            ValidateLength(storeCode, 50, nameof(storeCode));
        }

        private static void ValidateLength(string? value, int maximumLength, string name)
        {
            if (value != null && value.Length > maximumLength)
            {
                throw new ArgumentException(name + " exceeds the supported maximum length of " + maximumLength + ".", name);
            }
        }

        private static void TryRollback(SqlTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // Preserve the original operation exception.
            }
        }

        #endregion
    }
}
