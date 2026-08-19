/*
Sadr Scales 5.2.1 — Structured Invoice lookup + explicit ACK

IMPORTANT:
1) Lookup never acknowledges the invoice.
2) Save/commit the invoice in the destination application first.
3) Only after the destination commit succeeds, run the ACK section.
4) AlreadyRead is a warning, not a block: the full invoice is still returned.

TotalBarcode format:
    25 + ScaleID(D3) + FID(D9)
Example:
    ScaleID = 12, FID = 3456 -> 25012000003456
*/

DECLARE @TotalBarcode varchar(50) = '25012000003456';
DECLARE @Acknowledge bit = 0; -- Keep 0 for lookup-only. Set to 1 only AFTER destination commit succeeds.

/* -------------------------------------------------------------------------
   1. Read invoice header and current read state
   ------------------------------------------------------------------------- */
SELECT TOP (1)
    TotalID,
    ScaleID,
    SaleDateTime,
    ReceiptNo,
    TotalBarcode,
    ItemBarcode,
    NTrans,
    SubDiscAmt,
    DiscAmt,
    AmtOfATax,
    AmtOfVTax,
    PriceWTax,
    ClerkNo,
    CASE
        WHEN ISNULL(LableStatus, 0) = 1 THEN 'AlreadyRead'
        ELSE 'FoundUnread'
    END AS ReadStatus
FROM dbo.SADR_Total
WHERE TotalBarcode = @TotalBarcode
ORDER BY TotalID DESC;

/* -------------------------------------------------------------------------
   2. Read every persisted detail row
   ------------------------------------------------------------------------- */
SELECT
    d.DetailID,
    d.TotalID,
    d.TotalBarcode,
    d.ItemBarcode,
    d.TransNo,
    d.PluNo,
    d.Weight,
    d.QTY,
    d.Uprice,
    d.UpriceAfDisc,
    d.StPointDiscStat,
    d.TTLPriceDiscAmt,
    d.ActPrice,
    d.TaxRtNo
FROM dbo.SADR_Detail AS d
INNER JOIN dbo.SADR_Total AS t
    ON t.TotalID = d.TotalID
WHERE t.TotalBarcode = @TotalBarcode
ORDER BY d.DetailID ASC;

/* -------------------------------------------------------------------------
   3. Explicit ACK — run only after the destination transaction COMMIT succeeds
   ------------------------------------------------------------------------- */
IF @Acknowledge = 1
BEGIN
    SET XACT_ABORT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    BEGIN TRANSACTION;

    DECLARE @TotalID int = NULL;
    DECLARE @CurrentStatus int = NULL;

    SELECT TOP (1)
        @TotalID = TotalID,
        @CurrentStatus = LableStatus
    FROM dbo.SADR_Total WITH (UPDLOCK, HOLDLOCK)
    WHERE TotalBarcode = @TotalBarcode
    ORDER BY TotalID DESC;

    IF @TotalID IS NULL
    BEGIN
        SELECT 'NotFound' AS AckStatus;
    END
    ELSE IF ISNULL(@CurrentStatus, 0) = 1
    BEGIN
        SELECT 'AlreadyAcknowledged' AS AckStatus;
    END
    ELSE
    BEGIN
        UPDATE dbo.SADR_Total
        SET LableStatus = 1
        WHERE TotalID = @TotalID;

        SELECT 'Acknowledged' AS AckStatus;
    END

    COMMIT TRANSACTION;
END;
