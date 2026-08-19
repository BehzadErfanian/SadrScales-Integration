using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration.Internal;

namespace SadrScales.Integration.Invoices
{
    /// <summary>
    /// Structured-invoice lookup and explicit acknowledgement operations for Sadr Scales 5.2.1 SQL integration.
    /// </summary>
    public sealed class SadrInvoiceClient
    {
        #region SQL Contract

        private const string ReadHeaderSql = @"
SELECT TOP (1)
    TotalID,
    ScaleID,
    SaleDateTime,
    LableStatus,
    ReceiptNo,
    TotalBarcode,
    ItemBarcode,
    NTrans,
    SubDiscAmt,
    DiscAmt,
    AmtOfATax,
    AmtOfVTax,
    PriceWTax,
    ClerkNo
FROM dbo.SADR_Total
WHERE TotalBarcode = @TotalBarcode
ORDER BY TotalID DESC;";

        private const string ReadItemsSql = @"
SELECT
    DetailID,
    TotalID,
    TotalBarcode,
    ItemBarcode,
    TransNo,
    PluNo,
    Weight,
    QTY,
    Uprice,
    UpriceAfDisc,
    StPointDiscStat,
    TTLPriceDiscAmt,
    ActPrice,
    TaxRtNo
FROM dbo.SADR_Detail
WHERE TotalID = @TotalID
ORDER BY DetailID ASC;";

        private const string ReadAckStateSql = @"
SELECT TOP (1)
    TotalID,
    LableStatus
FROM dbo.SADR_Total WITH (UPDLOCK, HOLDLOCK)
WHERE TotalBarcode = @TotalBarcode
ORDER BY TotalID DESC;";

        private const string SetAcknowledgedSql = @"
UPDATE dbo.SADR_Total
SET LableStatus = 1
WHERE TotalID = @TotalID;";

        #endregion

        #region Dependencies and Construction

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly SadrScalesClientOptions _options;

        internal SadrInvoiceClient(SqlConnectionFactory connectionFactory, SadrScalesClientOptions options)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        #endregion

        #region Public Lookup API

        /// <summary>
        /// Reads a structured invoice by its aggregate TotalBarcode without changing acknowledgement state.
        /// </summary>
        /// <remarks>
        /// Both unread and previously acknowledged invoices return their complete persisted data.
        /// <see cref="SadrInvoiceLookupStatus.AlreadyRead"/> is informational and must not be treated as a recovery block.
        /// </remarks>
        public Task<SadrInvoiceLookupResult> GetByBarcodeAsync(
            string totalBarcode,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string normalizedBarcode = ValidateTotalBarcode(totalBarcode);

            return _connectionFactory.ExecuteReadAsync(
                (connection, token) => ReadInvoiceAsync(connection, normalizedBarcode, token),
                cancellationToken);
        }

        /// <summary>
        /// Reads a structured invoice by the source ScaleID and FID identity used to build TotalBarcode.
        /// </summary>
        public Task<SadrInvoiceLookupResult> GetAsync(
            int scaleId,
            int fid,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetByBarcodeAsync(BuildTotalBarcode(scaleId, fid), cancellationToken);
        }

        /// <summary>
        /// Builds the aggregate barcode used by Sadr Scales 5.2.1 structured invoices.
        /// </summary>
        public static string BuildTotalBarcode(int scaleId, int fid)
        {
            ValidateScaleAndFid(scaleId, fid);

            return "25" +
                   scaleId.ToString("D3", CultureInfo.InvariantCulture) +
                   fid.ToString("D9", CultureInfo.InvariantCulture);
        }

        #endregion

        #region Public ACK API

        /// <summary>
        /// Explicitly acknowledges one structured invoice after the destination has successfully committed its own copy.
        /// </summary>
        /// <remarks>
        /// This operation is idempotent. A repeated acknowledgement returns
        /// <see cref="SadrInvoiceAckStatus.AlreadyAcknowledged"/> and does not damage the invoice.
        /// </remarks>
        public async Task<SadrInvoiceAckStatus> AcknowledgeAsync(
            string totalBarcode,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string normalizedBarcode = ValidateTotalBarcode(totalBarcode);

            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    int? totalId = null;
                    bool alreadyAcknowledged = false;

                    using (var readCommand = new SqlCommand(ReadAckStateSql, connection, transaction))
                    {
                        readCommand.CommandTimeout = _options.CommandTimeoutSeconds;
                        readCommand.Parameters.Add("@TotalBarcode", SqlDbType.VarChar, 50).Value = normalizedBarcode;

                        using (var reader = await readCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                totalId = reader.GetInt32(0);
                                alreadyAcknowledged = !reader.IsDBNull(1) && reader.GetInt32(1) == 1;
                            }
                        }
                    }

                    if (!totalId.HasValue)
                    {
                        transaction.Commit();
                        return SadrInvoiceAckStatus.NotFound;
                    }

                    if (alreadyAcknowledged)
                    {
                        transaction.Commit();
                        return SadrInvoiceAckStatus.AlreadyAcknowledged;
                    }

                    using (var updateCommand = new SqlCommand(SetAcknowledgedSql, connection, transaction))
                    {
                        updateCommand.CommandTimeout = _options.CommandTimeoutSeconds;
                        updateCommand.Parameters.Add("@TotalID", SqlDbType.Int).Value = totalId.Value;

                        int affected = await updateCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        if (affected != 1)
                        {
                            throw new InvalidOperationException(
                                "Structured invoice acknowledgement did not update exactly one SADR_Total row.");
                        }
                    }

                    transaction.Commit();
                    return SadrInvoiceAckStatus.Acknowledged;
                }
                catch
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                        // Preserve the original write/read failure. The connection is disposed immediately after this scope.
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Explicitly acknowledges one structured invoice by ScaleID and FID.
        /// </summary>
        public Task<SadrInvoiceAckStatus> AcknowledgeAsync(
            int scaleId,
            int fid,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return AcknowledgeAsync(BuildTotalBarcode(scaleId, fid), cancellationToken);
        }

        #endregion

        #region SQL Mapping

        private async Task<SadrInvoiceLookupResult> ReadInvoiceAsync(
            SqlConnection connection,
            string totalBarcode,
            CancellationToken cancellationToken)
        {
            HeaderRow? header = null;

            using (var command = new SqlCommand(ReadHeaderSql, connection))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@TotalBarcode", SqlDbType.VarChar, 50).Value = totalBarcode;

                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        header = new HeaderRow(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            GetNullableDateTime(reader, 2),
                            !reader.IsDBNull(3) && reader.GetInt32(3) == 1,
                            reader.GetInt32(4),
                            GetNullableString(reader, 5),
                            reader.GetString(6),
                            GetNullableInt32(reader, 7),
                            GetNullableInt32(reader, 8),
                            GetNullableInt32(reader, 9),
                            GetNullableInt32(reader, 10),
                            GetNullableInt32(reader, 11),
                            GetNullableInt32(reader, 12),
                            GetNullableInt32(reader, 13));
                    }
                }
            }

            if (header == null)
            {
                return new SadrInvoiceLookupResult(SadrInvoiceLookupStatus.NotFound, null);
            }

            var items = new List<SadrInvoiceItem>();

            using (var command = new SqlCommand(ReadItemsSql, connection))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@TotalID", SqlDbType.Int).Value = header.TotalId;

                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        items.Add(new SadrInvoiceItem(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            GetNullableString(reader, 2),
                            reader.GetString(3),
                            GetNullableInt32(reader, 4),
                            reader.GetInt32(5),
                            GetNullableDouble(reader, 6),
                            GetNullableDouble(reader, 7),
                            GetNullableInt32(reader, 8),
                            GetNullableInt32(reader, 9),
                            GetNullableInt32(reader, 10),
                            GetNullableInt32(reader, 11),
                            GetNullableInt32(reader, 12),
                            GetNullableInt32(reader, 13)));
                    }
                }
            }

            var invoice = new SadrInvoice(
                header.TotalId,
                header.ScaleId,
                header.SaleDateTime,
                header.ReceiptNo,
                header.TotalBarcode,
                header.ItemBarcode,
                header.TransactionCount,
                header.SubDiscountAmount,
                header.DiscountAmount,
                header.ATaxAmount,
                header.VTaxAmount,
                header.PriceWithTax,
                header.ClerkNumber,
                header.IsAcknowledged,
                items.AsReadOnly());

            return new SadrInvoiceLookupResult(
                header.IsAcknowledged
                    ? SadrInvoiceLookupStatus.AlreadyRead
                    : SadrInvoiceLookupStatus.FoundUnread,
                invoice);
        }

        private static int? GetNullableInt32(SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
        }

        private static double? GetNullableDouble(SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (double?)null : reader.GetDouble(ordinal);
        }

        private static DateTime? GetNullableDateTime(SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
        }

        private static string? GetNullableString(SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        #endregion

        #region Validation

        private static string ValidateTotalBarcode(string totalBarcode)
        {
            if (totalBarcode == null)
            {
                throw new ArgumentNullException(nameof(totalBarcode));
            }

            string normalized = totalBarcode.Trim();
            if (normalized.Length != 14 || !normalized.StartsWith("25", StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "A Sadr structured TotalBarcode must contain 14 digits and start with 25.",
                    nameof(totalBarcode));
            }

            for (int index = 0; index < normalized.Length; index++)
            {
                char value = normalized[index];
                if (value < '0' || value > '9')
                {
                    throw new ArgumentException(
                        "A Sadr structured TotalBarcode must contain digits only.",
                        nameof(totalBarcode));
                }
            }

            return normalized;
        }

        private static void ValidateScaleAndFid(int scaleId, int fid)
        {
            if (scaleId < 1 || scaleId > 99)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleId), "ScaleID must be between 1 and 99.");
            }

            if (fid < 1 || fid > 999999999)
            {
                throw new ArgumentOutOfRangeException(nameof(fid), "FID must be between 1 and 999999999.");
            }
        }

        #endregion

        #region Internal Header Model

        private sealed class HeaderRow
        {
            public HeaderRow(
                int totalId,
                int scaleId,
                DateTime? saleDateTime,
                bool isAcknowledged,
                int receiptNo,
                string? totalBarcode,
                string itemBarcode,
                int? transactionCount,
                int? subDiscountAmount,
                int? discountAmount,
                int? aTaxAmount,
                int? vTaxAmount,
                int? priceWithTax,
                int? clerkNumber)
            {
                TotalId = totalId;
                ScaleId = scaleId;
                SaleDateTime = saleDateTime;
                IsAcknowledged = isAcknowledged;
                ReceiptNo = receiptNo;
                TotalBarcode = totalBarcode;
                ItemBarcode = itemBarcode;
                TransactionCount = transactionCount;
                SubDiscountAmount = subDiscountAmount;
                DiscountAmount = discountAmount;
                ATaxAmount = aTaxAmount;
                VTaxAmount = vTaxAmount;
                PriceWithTax = priceWithTax;
                ClerkNumber = clerkNumber;
            }

            public int TotalId { get; }
            public int ScaleId { get; }
            public DateTime? SaleDateTime { get; }
            public bool IsAcknowledged { get; }
            public int ReceiptNo { get; }
            public string? TotalBarcode { get; }
            public string ItemBarcode { get; }
            public int? TransactionCount { get; }
            public int? SubDiscountAmount { get; }
            public int? DiscountAmount { get; }
            public int? ATaxAmount { get; }
            public int? VTaxAmount { get; }
            public int? PriceWithTax { get; }
            public int? ClerkNumber { get; }
        }

        #endregion
    }
}
