/*
    Vendor-Ready Sadr Scales 5.2.1 sample
    Registered scale read + coarse status + optional AutoSend resend requests.

    SAFE DEFAULT:
      @ApplyResend = 0, so this script is read-only unless the developer explicitly enables a write.

    IMPORTANT:
      Resetting LastSendItem/LastSendKey records a request for a later eligible AutoSend cycle.
      It is NOT proof that the physical scale has received the data.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @ScaleID int = 1;
DECLARE @ApplyResend bit = 0;       -- Keep 0 for read-only inspection.
DECLARE @RequestItems bit = 0;      -- Set to 1 together with @ApplyResend to request item resend.
DECLARE @RequestHotKeys bit = 0;    -- Set to 1 together with @ApplyResend to request HotKey resend.

IF @ScaleID < 1 OR @ScaleID > 99
    THROW 51020, 'ScaleID must be between 1 and 99.', 1;

-- 1) Read all registered scales. Sadr Scales 5.2.1 persists Online/Offline into SADR_Scale.Status.
SELECT
    ScaleID,
    DeviceName,
    ScaleIP,
    Port,
    Category AS Model,
    StoreCode,
    StoreName,
    ItemClassCode AS PrimaryItemGroupCode,
    CASE
        WHEN UPPER(ISNULL(Status, '')) = 'ONLINE' THEN 'Online'
        WHEN UPPER(ISNULL(Status, '')) = 'OFFLINE' THEN 'Offline'
        ELSE 'Unknown'
    END AS IntegrationStatus,
    Used,
    AutoSendItems,
    AutoGetInvoice,
    Version,
    HotKeyCountPerPage,
    HotKeyPageCount
FROM dbo.SADR_Scale
ORDER BY ScaleID ASC;

-- 2) Show the selected scale before any optional write.
SELECT
    ScaleID,
    Category AS Model,
    Status,
    AutoSendItems,
    Used
FROM dbo.SADR_Scale
WHERE ScaleID = @ScaleID;

IF @ApplyResend = 0
BEGIN
    SELECT 'READ_ONLY' AS Result,
           'No resend state was changed. Set @ApplyResend = 1 explicitly to test a resend request.' AS Message;
    RETURN;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.SADR_Scale WHERE ScaleID = @ScaleID)
BEGIN
    SELECT 'NotFound' AS Result, @ScaleID AS ScaleID;
    RETURN;
END;

-- 3) Optional item resend request.
IF @RequestItems = 1
BEGIN
    UPDATE dbo.SADR_Scale
    SET LastSendItem = 0
    WHERE ScaleID = @ScaleID;

    SELECT 'Requested' AS ItemResendResult,
           @ScaleID AS ScaleID,
           'Request recorded. Wait for the next eligible AutoSend cycle.' AS Message;
END;

-- 4) Optional HotKey resend request.
-- Sadr Scales 5.2.1 has automatic HotKey transfer for these registered model categories.
IF @RequestHotKeys = 1
BEGIN
    DECLARE @Model varchar(50);
    SELECT @Model = Category
    FROM dbo.SADR_Scale
    WHERE ScaleID = @ScaleID;

    IF UPPER(ISNULL(@Model, '')) NOT IN ('LSG', 'LSG_24D', 'TSG', 'LS6')
    BEGIN
        SELECT 'UnsupportedModel' AS HotKeyResendResult,
               @ScaleID AS ScaleID,
               @Model AS Model,
               'The 5.2.1 runtime does not expose an automatic HotKey-send path for this model.' AS Message;
    END
    ELSE
    BEGIN
        UPDATE dbo.SADR_Scale
        SET LastSendKey = 0
        WHERE ScaleID = @ScaleID;

        SELECT 'Requested' AS HotKeyResendResult,
               @ScaleID AS ScaleID,
               @Model AS Model,
               'Request recorded. Wait for the next eligible AutoSend cycle.' AS Message;
    END;
END;

IF @RequestItems = 0 AND @RequestHotKeys = 0
BEGIN
    SELECT 'NO_REQUEST_SELECTED' AS Result,
           'Writes were enabled, but neither resend option was selected.' AS Message;
END;
