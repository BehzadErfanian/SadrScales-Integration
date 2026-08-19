using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration.Internal;

namespace SadrScales.Integration.Sales
{
    /// <summary>
    /// Read-only sales feed and additive Vendor-Ready query operations.
    /// </summary>
    public sealed partial class SadrSalesClient
    {
        private const string ReadSql = @"
SELECT TOP (@BatchSize)
    ID, DeviceNo, Identify, [DateTime], FID, SID,
    Salesman, SubID, TotalPrice, PLU, Class, Dept,
    Amount, Unit, LogType, Tax,
    Text1, Text2, Text3, Text4,
    UnitPrice, CoFID, PLUName
FROM dbo.SADR_Logs
WHERE ID > @LastProcessedId
ORDER BY ID ASC;";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly SadrScalesClientOptions _options;

        internal SadrSalesClient(SqlConnectionFactory connectionFactory, SadrScalesClientOptions options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
        }

        /// <summary>
        /// Reads the next batch after a destination-owned cursor without modifying Sadr Scales data.
        /// Recognized transient connection/read failures are retried within the configured bounded policy.
        /// </summary>
        /// <remarks>
        /// This is the synchronization/feed API. It remains ID-ascending and separate from newest-first user queries.
        /// The caller must persist destination data first and persist <see cref="SadrSalesBatch.LastReadId"/>
        /// only after the destination transaction succeeds.
        /// </remarks>
        public Task<SadrSalesBatch> ReadAfterAsync(
            long lastProcessedId,
            int batchSize = 100,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (lastProcessedId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lastProcessedId), "The sales cursor cannot be negative.");
            }

            if (batchSize < 1 || batchSize > 5000)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be between 1 and 5000.");
            }

            return _connectionFactory.ExecuteReadAsync(
                async (connection, token) =>
                {
                    var rows = new List<SadrSaleRow>(batchSize);

                    using (var command = new SqlCommand(ReadSql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        command.Parameters.Add("@BatchSize", SqlDbType.Int).Value = batchSize;
                        command.Parameters.Add("@LastProcessedId", SqlDbType.BigInt).Value = lastProcessedId;

                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                rows.Add(Map(reader));
                            }
                        }
                    }

                    return new SadrSalesBatch(rows, lastProcessedId);
                },
                cancellationToken);
        }

        private static SadrSaleRow Map(SqlDataReader reader)
        {
            return new SadrSaleRow(
                reader.GetInt32(0),
                reader.GetInt32(1),
                GetNullableString(reader, 2),
                reader.GetDateTime(3),
                reader.GetInt32(4),
                reader.GetInt32(5),
                reader.GetInt32(6),
                reader.GetInt32(7),
                reader.GetDouble(8),
                reader.GetInt32(9),
                reader.GetInt32(10),
                reader.GetInt32(11),
                reader.GetDouble(12),
                reader.GetInt32(13),
                reader.GetInt32(14),
                reader.GetDouble(15),
                GetNullableString(reader, 16),
                GetNullableString(reader, 17),
                GetNullableString(reader, 18),
                GetNullableString(reader, 19),
                reader.GetDouble(20),
                reader.GetInt32(21),
                reader.GetString(22));
        }

        private static string? GetNullableString(SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
    }
}
