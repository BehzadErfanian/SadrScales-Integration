/*
  Sadr Scales SQL Contract v1 - synthetic item-group + item upsert

  IMPORTANT:
  - Application code must use real SQL parameters. The local variables below only make this
    file runnable in SSMS with synthetic data.
  - By default this sample ROLLS BACK. Set @ApplyChanges = 1 only in a safe test database.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @ApplyChanges bit = 0;
DECLARE @ItemClassCode varchar(50) = 'DEMO';
DECLARE @ItemClassName nvarchar(100) = N'Demo Group';
DECLARE @Descriptions nvarchar(150) = N'Synthetic Contract v1 sample';
DECLARE @PluNo int = 900001;
DECLARE @PluName nvarchar(100) = N'Demo Apple';
DECLARE @PluUnit int = 3;
DECLARE @UnitPrice int = 125000;
DECLARE @DeleteFlag int = 0;

IF @PluNo = 0
    THROW 51101, 'PluNo must be non-zero.', 1;

BEGIN TRANSACTION;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SADR_ItemClass WITH (UPDLOCK, HOLDLOCK)
    WHERE ItemClassCode = @ItemClassCode
)
BEGIN
    INSERT INTO dbo.SADR_ItemClass
        (ItemClassCode, ItemClassName, Descriptions)
    VALUES
        (@ItemClassCode, @ItemClassName, @Descriptions);
END
ELSE
BEGIN
    UPDATE dbo.SADR_ItemClass
    SET ItemClassName = @ItemClassName,
        Descriptions = @Descriptions
    WHERE ItemClassCode = @ItemClassCode;
END;

UPDATE dbo.SADR_Item
SET ItemClassCode = @ItemClassCode,
    PluUnit = @PluUnit,
    UnitPrice = @UnitPrice,
    PluName = @PluName,
    DeleteFlag = @DeleteFlag
WHERE PluNo = @PluNo;

IF @@ROWCOUNT = 0
BEGIN
    INSERT INTO dbo.SADR_Item
        (ItemClassCode, PluNo, PluUnit, UnitPrice, PluName, DeleteFlag)
    VALUES
        (@ItemClassCode, @PluNo, @PluUnit, @UnitPrice, @PluName, @DeleteFlag);
END;

SELECT
    PluNo,
    ItemClassCode,
    PluName,
    PluUnit,
    UnitPrice,
    DeleteFlag
FROM dbo.SADR_Item
WHERE PluNo = @PluNo;

IF @ApplyChanges = 1
BEGIN
    COMMIT TRANSACTION;
    PRINT 'COMMIT - synthetic item retained in the test database.';
END
ELSE
BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'ROLLBACK - dry-run complete; no data retained.';
END;
