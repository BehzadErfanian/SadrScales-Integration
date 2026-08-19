using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration.Internal;

namespace SadrScales.Integration.Scales
{
    /// <summary>
    /// SQL-readable scale registry/status operations and idempotent AutoSend resend requests.
    /// </summary>
    public sealed class SadrScaleClient
    {
        #region SQL

        private const string ReadColumns = @"
    ScaleID, Port, Mac, ItemClassCode, Version, DeviceName,
    StoreName, ScaleIP, AutoSendItems, Status, AutoGetInvoice,
    Category, StoreCode, HotKeyCountPerPage, HotKeyPageCount, Used";

        #endregion

        #region Dependencies

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly SadrScalesClientOptions _options;

        #endregion

        #region Construction

        internal SadrScaleClient(SqlConnectionFactory connectionFactory, SadrScalesClientOptions options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
        }

        #endregion

        #region Read API

        /// <summary>
        /// Reads all registered scales ordered by scale number.
        /// </summary>
        public Task<IReadOnlyList<SadrScale>> GetAllAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _connectionFactory.ExecuteReadAsync<IReadOnlyList<SadrScale>>(
                async (connection, token) =>
                {
                    var result = new List<SadrScale>();
                    var sql = "SELECT " + ReadColumns + " FROM dbo.SADR_Scale ORDER BY ScaleID ASC;";

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
        /// Reads one registered scale, or returns null when the scale number does not exist.
        /// </summary>
        public Task<SadrScale?> GetAsync(
            int scaleId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateScaleId(scaleId);

            return _connectionFactory.ExecuteReadAsync<SadrScale?>(
                async (connection, token) =>
                {
                    var sql = "SELECT " + ReadColumns + " FROM dbo.SADR_Scale WHERE ScaleID = @ScaleID;";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;

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

        /// <summary>
        /// Reads the coarse Online/Offline state persisted by Sadr Scales.
        /// Missing or unrecognized state is returned as <see cref="SadrScaleStatus.Unknown"/>.
        /// </summary>
        public Task<SadrScaleStatus> GetStatusAsync(
            int scaleId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateScaleId(scaleId);

            return _connectionFactory.ExecuteReadAsync(
                async (connection, token) =>
                {
                    const string sql = "SELECT Status FROM dbo.SADR_Scale WHERE ScaleID = @ScaleID;";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;
                        var value = await command.ExecuteScalarAsync(token).ConfigureAwait(false);
                        return ParseStatus(value == null || value == DBNull.Value ? null : Convert.ToString(value));
                    }
                },
                cancellationToken);
        }

        #endregion

        #region Resend API

        /// <summary>
        /// Records an item AutoSend resend request by resetting the internal item-send watermark.
        /// </summary>
        /// <remarks>
        /// Success means the request was recorded in SQL. Actual device transfer occurs during a later eligible
        /// AutoSend cycle when the scale is enabled, connected and configured for automatic sending.
        /// </remarks>
        public async Task<SadrResendRequestResult> RequestItemResendAsync(
            int scaleId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateScaleId(scaleId);

            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            using (var command = new SqlCommand(
                "UPDATE dbo.SADR_Scale SET LastSendItem = 0 WHERE ScaleID = @ScaleID;",
                connection))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;
                var changed = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                return changed == 1
                    ? SadrResendRequestResult.Requested
                    : SadrResendRequestResult.NotFound;
            }
        }

        /// <summary>
        /// Records a HotKey AutoSend resend request for models whose 5.2.1 runtime supports automatic HotKey sending.
        /// </summary>
        public async Task<SadrResendRequestResult> RequestHotKeyResendAsync(
            int scaleId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateScaleId(scaleId);

            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            {
                string? model;
                using (var lookup = new SqlCommand(
                    "SELECT Category FROM dbo.SADR_Scale WHERE ScaleID = @ScaleID;",
                    connection))
                {
                    lookup.CommandTimeout = _options.CommandTimeoutSeconds;
                    lookup.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;
                    var value = await lookup.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                    if (value == null || value == DBNull.Value)
                    {
                        return SadrResendRequestResult.NotFound;
                    }

                    model = Convert.ToString(value);
                }

                if (!SupportsAutomaticHotKeySend(model))
                {
                    return SadrResendRequestResult.UnsupportedModel;
                }

                using (var command = new SqlCommand(
                    "UPDATE dbo.SADR_Scale SET LastSendKey = 0 WHERE ScaleID = @ScaleID;",
                    connection))
                {
                    command.CommandTimeout = _options.CommandTimeoutSeconds;
                    command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scaleId;
                    var changed = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    return changed == 1
                        ? SadrResendRequestResult.Requested
                        : SadrResendRequestResult.NotFound;
                }
            }
        }

        #endregion

        #region Mapping

        private static SadrScale Map(SqlDataReader reader)
        {
            return new SadrScale
            {
                ScaleId = reader.GetInt32(0),
                Port = GetInt32(reader, 1),
                Mac = GetNullableString(reader, 2),
                PrimaryItemGroupCode = GetNullableString(reader, 3),
                Version = GetNullableString(reader, 4),
                DeviceName = GetNullableString(reader, 5),
                StoreName = GetNullableString(reader, 6),
                IpAddress = GetNullableString(reader, 7),
                AutoSendItems = GetBoolean(reader, 8),
                Status = ParseStatus(GetNullableString(reader, 9)),
                AutoGetInvoice = GetBoolean(reader, 10),
                Model = GetNullableString(reader, 11),
                StoreCode = GetNullableString(reader, 12),
                HotKeyCountPerPage = GetInt32(reader, 13),
                HotKeyPageCount = GetInt32(reader, 14),
                Used = GetBoolean(reader, 15)
            };
        }

        private static SadrScaleStatus ParseStatus(string? value)
        {
            if (string.Equals(value, "Online", StringComparison.OrdinalIgnoreCase))
            {
                return SadrScaleStatus.Online;
            }

            if (string.Equals(value, "Offline", StringComparison.OrdinalIgnoreCase))
            {
                return SadrScaleStatus.Offline;
            }

            return SadrScaleStatus.Unknown;
        }

        private static bool SupportsAutomaticHotKeySend(string? model)
        {
            return string.Equals(model, "LSG", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(model, "LSG_24D", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(model, "TSG", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(model, "LS6", StringComparison.OrdinalIgnoreCase);
        }

        private static string? GetNullableString(SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : Convert.ToString(reader.GetValue(ordinal));
        }

        private static int GetInt32(SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static bool GetBoolean(SqlDataReader reader, int ordinal)
        {
            return !reader.IsDBNull(ordinal) && Convert.ToBoolean(reader.GetValue(ordinal));
        }

        #endregion

        #region Validation

        private static void ValidateScaleId(int scaleId)
        {
            if (scaleId < 1 || scaleId > 99)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleId), "Scale ID must be between 1 and 99.");
            }
        }

        #endregion
    }
}
