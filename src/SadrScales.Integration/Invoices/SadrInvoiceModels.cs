using System;
using System.Collections.Generic;

namespace SadrScales.Integration.Invoices
{
    /// <summary>
    /// Describes the result of looking up one structured Sadr Scales invoice.
    /// </summary>
    public enum SadrInvoiceLookupStatus
    {
        /// <summary>
        /// The invoice exists and has not yet been explicitly acknowledged by the destination.
        /// </summary>
        FoundUnread = 0,

        /// <summary>
        /// The invoice exists and was acknowledged previously. The complete invoice is still returned.
        /// </summary>
        AlreadyRead = 1,

        /// <summary>
        /// No structured invoice matched the requested identity.
        /// </summary>
        NotFound = 2
    }

    /// <summary>
    /// Describes the result of explicitly acknowledging one structured invoice.
    /// </summary>
    public enum SadrInvoiceAckStatus
    {
        /// <summary>
        /// The invoice was found and changed from unread to acknowledged.
        /// </summary>
        Acknowledged = 0,

        /// <summary>
        /// The invoice was already acknowledged. No harmful second mutation was performed.
        /// </summary>
        AlreadyAcknowledged = 1,

        /// <summary>
        /// No structured invoice matched the requested identity.
        /// </summary>
        NotFound = 2
    }

    /// <summary>
    /// Complete result returned by a structured-invoice lookup.
    /// </summary>
    public sealed class SadrInvoiceLookupResult
    {
        #region Construction

        internal SadrInvoiceLookupResult(SadrInvoiceLookupStatus status, SadrInvoice? invoice)
        {
            Status = status;
            Invoice = invoice;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Gets the lookup state. <see cref="SadrInvoiceLookupStatus.AlreadyRead"/> is informational and does not block recovery.
        /// </summary>
        public SadrInvoiceLookupStatus Status { get; }

        /// <summary>
        /// Gets the complete invoice when found; otherwise <see langword="null"/>.
        /// </summary>
        public SadrInvoice? Invoice { get; }

        #endregion
    }

    /// <summary>
    /// Structured invoice header plus all persisted detail rows from Sadr Scales.
    /// </summary>
    public sealed class SadrInvoice
    {
        #region Construction

        internal SadrInvoice(
            int totalId,
            int scaleId,
            DateTime? saleDateTime,
            int receiptNo,
            string? totalBarcode,
            string itemBarcode,
            int? transactionCount,
            int? subDiscountAmount,
            int? discountAmount,
            int? aTaxAmount,
            int? vTaxAmount,
            int? priceWithTax,
            int? clerkNumber,
            bool isAcknowledged,
            IReadOnlyList<SadrInvoiceItem> items)
        {
            TotalId = totalId;
            ScaleId = scaleId;
            SaleDateTime = saleDateTime;
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
            IsAcknowledged = isAcknowledged;
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        #endregion

        #region Identity and Read State

        /// <summary>
        /// Gets the internal structured-invoice identifier persisted in <c>SADR_Total.TotalID</c>.
        /// </summary>
        public int TotalId { get; }

        /// <summary>
        /// Gets the Sadr scale number that produced the invoice.
        /// </summary>
        public int ScaleId { get; }

        /// <summary>
        /// Gets the persisted aggregate barcode used for POS/ERP invoice lookup.
        /// </summary>
        public string? TotalBarcode { get; }

        /// <summary>
        /// Gets the header barcode persisted by Sadr Scales.
        /// </summary>
        public string ItemBarcode { get; }

        /// <summary>
        /// Gets whether the destination has previously acknowledged the invoice.
        /// </summary>
        public bool IsAcknowledged { get; }

        #endregion

        #region Header Data

        /// <summary>
        /// Gets the sale date/time when available.
        /// </summary>
        public DateTime? SaleDateTime { get; }

        /// <summary>
        /// Gets the receipt number stored by the source scale.
        /// </summary>
        public int ReceiptNo { get; }

        /// <summary>
        /// Gets the transaction count (<c>NTrans</c>) when available.
        /// </summary>
        public int? TransactionCount { get; }

        /// <summary>
        /// Gets the persisted sub-discount amount when available.
        /// </summary>
        public int? SubDiscountAmount { get; }

        /// <summary>
        /// Gets the persisted discount amount when available.
        /// </summary>
        public int? DiscountAmount { get; }

        /// <summary>
        /// Gets the persisted A-tax amount without changing the source schema meaning.
        /// </summary>
        public int? ATaxAmount { get; }

        /// <summary>
        /// Gets the persisted V-tax amount without changing the source schema meaning.
        /// </summary>
        public int? VTaxAmount { get; }

        /// <summary>
        /// Gets the persisted price-with-tax value when available.
        /// </summary>
        public int? PriceWithTax { get; }

        /// <summary>
        /// Gets the clerk/salesman number when available.
        /// </summary>
        public int? ClerkNumber { get; }

        /// <summary>
        /// Gets every persisted structured detail row in source order.
        /// </summary>
        public IReadOnlyList<SadrInvoiceItem> Items { get; }

        #endregion
    }

    /// <summary>
    /// One persisted detail row belonging to a structured Sadr Scales invoice.
    /// </summary>
    public sealed class SadrInvoiceItem
    {
        #region Construction

        internal SadrInvoiceItem(
            int detailId,
            int totalId,
            string? totalBarcode,
            string itemBarcode,
            int? transactionNo,
            int pluNo,
            double? weight,
            double? quantity,
            int? unitPrice,
            int? unitPriceAfterDiscount,
            int? stPointDiscountStatus,
            int? totalPriceDiscountAmount,
            int? actualPrice,
            int? taxRateNo)
        {
            DetailId = detailId;
            TotalId = totalId;
            TotalBarcode = totalBarcode;
            ItemBarcode = itemBarcode;
            TransactionNo = transactionNo;
            PluNo = pluNo;
            Weight = weight;
            Quantity = quantity;
            UnitPrice = unitPrice;
            UnitPriceAfterDiscount = unitPriceAfterDiscount;
            StPointDiscountStatus = stPointDiscountStatus;
            TotalPriceDiscountAmount = totalPriceDiscountAmount;
            ActualPrice = actualPrice;
            TaxRateNo = taxRateNo;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Gets the structured detail identifier.
        /// </summary>
        public int DetailId { get; }

        /// <summary>
        /// Gets the parent <c>SADR_Total.TotalID</c>.
        /// </summary>
        public int TotalId { get; }

        /// <summary>
        /// Gets the aggregate barcode copied onto this detail row when available.
        /// </summary>
        public string? TotalBarcode { get; }

        /// <summary>
        /// Gets the detail item barcode.
        /// </summary>
        public string ItemBarcode { get; }

        /// <summary>
        /// Gets the source transaction/sub-item number when available.
        /// </summary>
        public int? TransactionNo { get; }

        /// <summary>
        /// Gets the PLU number.
        /// </summary>
        public int PluNo { get; }

        /// <summary>
        /// Gets the persisted weight value when this line is weight-based.
        /// </summary>
        public double? Weight { get; }

        /// <summary>
        /// Gets the persisted quantity value when this line is count-based.
        /// </summary>
        public double? Quantity { get; }

        /// <summary>
        /// Gets the unit price when available.
        /// </summary>
        public int? UnitPrice { get; }

        /// <summary>
        /// Gets the unit price after discount when available.
        /// </summary>
        public int? UnitPriceAfterDiscount { get; }

        /// <summary>
        /// Gets the persisted <c>StPointDiscStat</c> value without redefining its source meaning.
        /// </summary>
        public int? StPointDiscountStatus { get; }

        /// <summary>
        /// Gets the persisted total-price discount amount when available.
        /// </summary>
        public int? TotalPriceDiscountAmount { get; }

        /// <summary>
        /// Gets the actual line price when available.
        /// </summary>
        public int? ActualPrice { get; }

        /// <summary>
        /// Gets the persisted tax-rate number when available.
        /// </summary>
        public int? TaxRateNo { get; }

        #endregion
    }
}
