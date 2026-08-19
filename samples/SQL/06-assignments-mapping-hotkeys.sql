/*
Sadr Scales Integration 1.1.0 — Vendor-Ready configuration sample

Covers:
  1) canonical scale -> item-group assignments;
  2) per-scale PLU / ItemCode / optional HotKey mapping;
  3) user-managed group HotKey templates.

SAFE DEFAULT:
  @ApplyChanges = 0 performs reads only.

Important:
  - Configuration writes are full replacements inside a transaction.
  - Mapping changes reset LastSendItem + LastSendKey for the selected scale.
  - Assignment changes reset LastSendItem for the selected scale.
  - Group HotKey changes reset LastSendKey only for scales assigned to that group.
  - Group HotKey replace never deletes rows whose PluNo <= 0; those rows may be internal/system data.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @ScaleID int = 1;
DECLARE @ItemClassCode varchar(50) = '0';
DECLARE @PluNo int = 900001;
DECLARE @ScaleItemCode int = 1;
DECLARE @PageNo int = NULL;
DECLARE @KeyNo int = NULL;
DECLARE @ApplyChanges bit = 0;
DECLARE @ReplaceAssignments bit = 0;
DECLARE @ReplaceMapping bit = 0;
DECLARE @ReplaceHotKeys bit = 0;

/* ================================================================
   READ — always safe
   ================================================================ */

SELECT ItemClassCode
FROM dbo.SADR_ScaleItemClass
WHERE ScaleID = @ScaleID
ORDER BY ItemClassCode;

SELECT ScaleID, PluNo, ItemCode, PageNo, KeyNo
FROM dbo.SADR_ScaleItemMap
WHERE ScaleID = @ScaleID
ORDER BY PluNo;

SELECT ItemClassCode, PageNo, KeyNo, PluNo
FROM dbo.SADR_KeyAssignment
WHERE ItemClassCode = @ItemClassCode
  AND PluNo > 0
ORDER BY PageNo, KeyNo;

IF @ApplyChanges = 0
BEGIN
    PRINT 'READ-ONLY - set @ApplyChanges = 1 only on an intentional test/development database.';
    RETURN;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.SADR_Scale WHERE ScaleID = @ScaleID)
    THROW 51001, 'Selected ScaleID does not exist.', 1;

IF NOT EXISTS (SELECT 1 FROM dbo.SADR_ItemClass WHERE ItemClassCode = @ItemClassCode)
    THROW 51002, 'Selected ItemClassCode does not exist.', 1;

BEGIN TRANSACTION;

/* ================================================================
   REPLACE SCALE GROUP ASSIGNMENTS
   Example replaces the selected scale with one group.
   ================================================================ */
IF @ReplaceAssignments = 1
BEGIN
    DELETE FROM dbo.SADR_ScaleItemClass
    WHERE ScaleID = @ScaleID;

    INSERT INTO dbo.SADR_ScaleItemClass(ScaleID, ItemClassCode)
    VALUES(@ScaleID, @ItemClassCode);

    UPDATE dbo.SADR_Scale
    SET LastSendItem = 0
    WHERE ScaleID = @ScaleID;
END;

/* ================================================================
   REPLACE PER-SCALE ITEM MAPPING
   Example replaces the complete selected-scale map with one row.
   PageNo/KeyNo must be both NULL or both populated and must fit the
   selected scale's HotKeyCountPerPage / HotKeyPageCount layout.
   ================================================================ */
IF @ReplaceMapping = 1
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SADR_Item WHERE PluNo = @PluNo)
        THROW 51003, 'Selected PluNo does not exist.', 1;

    IF (@PageNo IS NULL AND @KeyNo IS NOT NULL)
       OR (@PageNo IS NOT NULL AND @KeyNo IS NULL)
        THROW 51004, 'PageNo and KeyNo must both be NULL or both be populated.', 1;

    IF @ScaleItemCode <= 0
        THROW 51005, 'Scale ItemCode must be positive.', 1;

    IF @PageNo IS NOT NULL
    BEGIN
        DECLARE @HotKeyCountPerPage int;
        DECLARE @HotKeyPageCount int;

        SELECT
            @HotKeyCountPerPage = ISNULL(HotKeyCountPerPage, 0),
            @HotKeyPageCount = ISNULL(HotKeyPageCount, 0)
        FROM dbo.SADR_Scale
        WHERE ScaleID = @ScaleID;

        IF @HotKeyCountPerPage <= 0
           OR @HotKeyPageCount <= 0
           OR @PageNo < 0
           OR @PageNo >= @HotKeyPageCount
           OR @KeyNo <= 0
           OR @KeyNo > @HotKeyCountPerPage
            THROW 51006, 'Requested HotKey position does not fit the selected scale layout.', 1;
    END;

    DELETE FROM dbo.SADR_ScaleItemMap
    WHERE ScaleID = @ScaleID;

    INSERT INTO dbo.SADR_ScaleItemMap(ScaleID, PluNo, ItemCode, PageNo, KeyNo)
    VALUES(@ScaleID, @PluNo, @ScaleItemCode, @PageNo, @KeyNo);

    UPDATE dbo.SADR_Scale
    SET LastSendItem = 0,
        LastSendKey = 0
    WHERE ScaleID = @ScaleID;
END;

/* ================================================================
   REPLACE USER-MANAGED GROUP HOTKEY TEMPLATE
   Example replaces positive-PLU user keys with one key.
   Internal/system rows whose PluNo <= 0 are intentionally preserved.
   ================================================================ */
IF @ReplaceHotKeys = 1
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SADR_Item WHERE PluNo = @PluNo)
        THROW 51007, 'Selected PluNo does not exist.', 1;

    IF @PageNo IS NULL OR @KeyNo IS NULL OR @PageNo < 0 OR @KeyNo <= 0
        THROW 51008, 'HotKey replacement requires a non-negative PageNo and positive KeyNo.', 1;

    DELETE FROM dbo.SADR_KeyAssignment
    WHERE ItemClassCode = @ItemClassCode
      AND PluNo > 0;

    INSERT INTO dbo.SADR_KeyAssignment(ItemClassCode, PageNo, KeyNo, PluNo)
    VALUES(@ItemClassCode, @PageNo, @KeyNo, @PluNo);

    UPDATE s
    SET LastSendKey = 0
    FROM dbo.SADR_Scale s
    WHERE EXISTS
    (
        SELECT 1
        FROM dbo.SADR_ScaleItemClass sic
        WHERE sic.ScaleID = s.ScaleID
          AND sic.ItemClassCode = @ItemClassCode
    );
END;

COMMIT TRANSACTION;

PRINT 'APPLIED - requested configuration replacement(s) committed.';

SELECT ItemClassCode
FROM dbo.SADR_ScaleItemClass
WHERE ScaleID = @ScaleID
ORDER BY ItemClassCode;

SELECT ScaleID, PluNo, ItemCode, PageNo, KeyNo
FROM dbo.SADR_ScaleItemMap
WHERE ScaleID = @ScaleID
ORDER BY PluNo;

SELECT ItemClassCode, PageNo, KeyNo, PluNo
FROM dbo.SADR_KeyAssignment
WHERE ItemClassCode = @ItemClassCode
ORDER BY PageNo, KeyNo;
