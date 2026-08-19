SET NOCOUNT ON;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SADR_Item')
      AND name = N'UX_SADR_Item_PluNo'
      AND is_unique = 1)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_SADR_Item_PluNo
        ON dbo.SADR_Item(PluNo);
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.SADR_Item')
      AND name = N'CK_SADR_Item_PluNo_NotZero')
BEGIN
    ALTER TABLE dbo.SADR_Item
        ADD CONSTRAINT CK_SADR_Item_PluNo_NotZero
        CHECK (PluNo <> 0);
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SADR_Logs')
      AND name = N'UX_SADR_Logs_DeviceNo_FID_SubID'
      AND is_unique = 1)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_SADR_Logs_DeviceNo_FID_SubID
        ON dbo.SADR_Logs(DeviceNo, FID, SubID);
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.SADR_Logs')
      AND name = N'CK_SADR_Logs_FID_SubID_PLU_NotZero')
BEGIN
    ALTER TABLE dbo.SADR_Logs
        ADD CONSTRAINT CK_SADR_Logs_FID_SubID_PLU_NotZero
        CHECK (FID <> 0 AND SubID <> 0 AND PLU <> 0);
END;
