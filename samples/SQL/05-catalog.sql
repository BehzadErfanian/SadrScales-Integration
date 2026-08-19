/*
    Vendor-Ready Sadr Scales 5.2.1 sample
    Stores + item groups + item catalog + logical delete + price history.

    SAFE DEFAULT:
      @ApplyChanges = 0 and @SoftDeletePlu = 0, so this script is read-only by default.

    IMPORTANT:
      Item deletion is logical only: DeleteFlag = 1. Do not physically DELETE PLU rows.
      Price history is read-only in the 1.1.0 Vendor-Ready contract.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @ApplyChanges bit = 0;
DECLARE @DemoStoreCode varchar(50) = 'DEMO';
DECLARE @DemoStoreName nvarchar(100) = N'Demo Store';
DECLARE @DemoStoreDescriptions nvarchar(150) = N'Vendor-Ready SQL sample';
DECLARE @SelectedPlu int = 900001;
DECLARE @SoftDeletePlu int = 0; -- 0 = do not soft-delete anything.

-- 1) Read stores.
SELECT StoreCode, StoreName, Descriptions
FROM dbo.SADR_Store
ORDER BY StoreCode ASC;

-- 2) Read item groups.
SELECT ItemClassCode, ItemClassName, Descriptions
FROM dbo.SADR_ItemClass
ORDER BY ItemClassCode ASC;

-- 3) Read active PLUs only (normal catalog view).
SELECT
    ItemClassCode, PluNo, PluUnit, UnitPrice, PrintFormat, PluCost,
    BarFormat, BarFlags, ItemCode, IndexBarcode, Tare,
    ShelfDate, ShelfDatePrint, SaleDatePrint, SaleTimePrint,
    OnlyTare, TaxRate, PluName,
    Text1, Text2, Text3, Text4, Text5, Text6, Text7,
    DeleteFlag
FROM dbo.SADR_Item
WHERE ISNULL(DeleteFlag, 0) = 0
ORDER BY PluNo ASC;

-- 4) Read one PLU even when logically deleted, so recovery/inspection remains possible.
SELECT
    ItemClassCode, PluNo, PluUnit, UnitPrice, PrintFormat, PluCost,
    BarFormat, BarFlags, ItemCode, IndexBarcode, Tare,
    ShelfDate, ShelfDatePrint, SaleDatePrint, SaleTimePrint,
    OnlyTare, TaxRate, PluName,
    Text1, Text2, Text3, Text4, Text5, Text6, Text7,
    DeleteFlag
FROM dbo.SADR_Item
WHERE PluNo = @SelectedPlu;

-- 5) Read recent price history for the selected PLU. This section performs no writes.
SELECT TOP (100)
    ID, PluNo, IndexBarcode, PluName, LastPrice, NewPrice, [DateTime], [User]
FROM dbo.SADR_PriceLog
WHERE PluNo = @SelectedPlu
ORDER BY [DateTime] DESC, ID DESC;

IF @ApplyChanges = 0
BEGIN
    SELECT 'READ_ONLY' AS Result,
           'No catalog changes were applied. Set @ApplyChanges = 1 explicitly to test sanctioned writes.' AS Message;
    RETURN;
END;

BEGIN TRANSACTION;
BEGIN TRY
    -- 6) Optional semantic Store upsert. StoreCode is the stable identity.
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.SADR_Store WITH (UPDLOCK, HOLDLOCK)
        WHERE StoreCode = @DemoStoreCode
    )
    BEGIN
        INSERT INTO dbo.SADR_Store(StoreCode, StoreName, Descriptions)
        VALUES(@DemoStoreCode, @DemoStoreName, @DemoStoreDescriptions);
    END
    ELSE
    BEGIN
        UPDATE dbo.SADR_Store
        SET StoreName = @DemoStoreName,
            Descriptions = @DemoStoreDescriptions
        WHERE StoreCode = @DemoStoreCode;
    END;

    -- 7) Optional logical item delete. Never physically DELETE a PLU row.
    IF @SoftDeletePlu <> 0
    BEGIN
        UPDATE dbo.SADR_Item
        SET DeleteFlag = 1
        WHERE PluNo = @SoftDeletePlu;

        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'NotFound' AS SoftDeleteResult, @SoftDeletePlu AS PluNo;
        END
        ELSE
        BEGIN
            SELECT 'DeletedOrAlreadyDeleted' AS SoftDeleteResult,
                   @SoftDeletePlu AS PluNo,
                   'DeleteFlag is 1; the row remains in the catalog for synchronization/recovery.' AS Message;
        END;
    END;

    COMMIT TRANSACTION;

    SELECT 'APPLIED' AS Result,
           'Store upsert and any selected logical delete were committed.' AS Message;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;

/*
    For a full Item Group + PLU parameterized upsert example, see:
      samples/SQL/01-upsert-item.sql

    Application code must use SQL parameters instead of string concatenation.
*/
