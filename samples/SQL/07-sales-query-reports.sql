/*
Sadr Scales Integration 1.1.0 — Vendor-Ready Sales Query + Reports

READ-ONLY sample. It never updates SADR_Logs.

Semantics match Sadr Scales 5.2.1:
- filters: date range, Identify, PLU, Scale/DeviceNo, FID;
- newest-first page ordered by DateTime DESC, ID DESC;
- page size clamped to 50..2000;
- invoice count = distinct DeviceNo + FID;
- Unit 0/1/3 contributes to TotalWeight;
- Unit 2 contributes to TotalQuantity;
- reports: Daily, Scale, Item (item report top 5000).
*/

SET NOCOUNT ON;

DECLARE @StartDate datetime = NULL;
DECLARE @EndDate datetime = NULL;          -- exclusive
DECLARE @Identify nvarchar(50) = NULL;     -- exact match
DECLARE @Plu int = NULL;
DECLARE @ScaleId int = NULL;
DECLARE @Fid int = NULL;
DECLARE @PageNumber int = 1;
DECLARE @PageSize int = 200;

SET @PageNumber = CASE WHEN @PageNumber < 1 THEN 1 ELSE @PageNumber END;
SET @PageSize = CASE
    WHEN @PageSize < 50 THEN 50
    WHEN @PageSize > 2000 THEN 2000
    ELSE @PageSize
END;

DECLARE @Offset int = (@PageNumber - 1) * @PageSize;

/* ================================================================
   SUMMARY — complete filter, not only current page
   ================================================================ */
SELECT
    COUNT_BIG(*) AS RecordCount,
    COUNT_BIG(DISTINCT CONVERT(varchar(20), DeviceNo) + ':' + CONVERT(varchar(20), FID)) AS InvoiceCount,
    ISNULL(SUM(CONVERT(decimal(38, 3), TotalPrice)), 0) AS TotalPrice,
    ISNULL(SUM(CASE [Unit]
                  WHEN 0 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 1 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 3 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalWeight,
    ISNULL(SUM(CASE [Unit]
                  WHEN 2 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalQuantity
FROM dbo.SADR_Logs
WHERE (@StartDate IS NULL OR [DateTime] >= @StartDate)
  AND (@EndDate IS NULL OR [DateTime] < @EndDate)
  AND (@Identify IS NULL OR Identify = @Identify)
  AND (@Plu IS NULL OR PLU = @Plu)
  AND (@ScaleId IS NULL OR DeviceNo = @ScaleId)
  AND (@Fid IS NULL OR FID = @Fid);

/* ================================================================
   NEWEST-FIRST QUERY PAGE
   This is search/report UI behavior, NOT destination cursor sync.
   ================================================================ */
SELECT
    ID, DeviceNo, Identify, [DateTime], FID, SID,
    Salesman, SubID, TotalPrice, PLU, Class, Dept,
    Amount, Unit, LogType, Tax,
    Text1, Text2, Text3, Text4,
    UnitPrice, CoFID, PLUName
FROM dbo.SADR_Logs
WHERE (@StartDate IS NULL OR [DateTime] >= @StartDate)
  AND (@EndDate IS NULL OR [DateTime] < @EndDate)
  AND (@Identify IS NULL OR Identify = @Identify)
  AND (@Plu IS NULL OR PLU = @Plu)
  AND (@ScaleId IS NULL OR DeviceNo = @ScaleId)
  AND (@Fid IS NULL OR FID = @Fid)
ORDER BY [DateTime] DESC, ID DESC
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;

/* ================================================================
   DAILY REPORT
   ================================================================ */
SELECT
    CONVERT(date, [DateTime]) AS SaleDate,
    COUNT_BIG(*) AS RecordCount,
    COUNT(DISTINCT CONVERT(varchar(20), DeviceNo) + ':' + CONVERT(varchar(20), FID)) AS InvoiceCount,
    ISNULL(SUM(CONVERT(decimal(38, 3), TotalPrice)), 0) AS TotalPrice,
    ISNULL(SUM(CASE [Unit]
                  WHEN 0 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 1 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 3 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalWeight,
    ISNULL(SUM(CASE [Unit]
                  WHEN 2 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalQuantity
FROM dbo.SADR_Logs
WHERE (@StartDate IS NULL OR [DateTime] >= @StartDate)
  AND (@EndDate IS NULL OR [DateTime] < @EndDate)
  AND (@Identify IS NULL OR Identify = @Identify)
  AND (@Plu IS NULL OR PLU = @Plu)
  AND (@ScaleId IS NULL OR DeviceNo = @ScaleId)
  AND (@Fid IS NULL OR FID = @Fid)
GROUP BY CONVERT(date, [DateTime])
ORDER BY SaleDate DESC;

/* ================================================================
   SCALE REPORT
   ================================================================ */
SELECT
    DeviceNo AS ScaleId,
    MAX(ISNULL(Identify, N'')) AS Identify,
    COUNT_BIG(*) AS RecordCount,
    COUNT(DISTINCT CONVERT(varchar(20), DeviceNo) + ':' + CONVERT(varchar(20), FID)) AS InvoiceCount,
    ISNULL(SUM(CONVERT(decimal(38, 3), TotalPrice)), 0) AS TotalPrice,
    ISNULL(SUM(CASE [Unit]
                  WHEN 0 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 1 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 3 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalWeight,
    ISNULL(SUM(CASE [Unit]
                  WHEN 2 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalQuantity
FROM dbo.SADR_Logs
WHERE (@StartDate IS NULL OR [DateTime] >= @StartDate)
  AND (@EndDate IS NULL OR [DateTime] < @EndDate)
  AND (@Identify IS NULL OR Identify = @Identify)
  AND (@Plu IS NULL OR PLU = @Plu)
  AND (@ScaleId IS NULL OR DeviceNo = @ScaleId)
  AND (@Fid IS NULL OR FID = @Fid)
GROUP BY DeviceNo
ORDER BY TotalPrice DESC, DeviceNo;

/* ================================================================
   ITEM REPORT — top 5000, matching 5.2.1
   ================================================================ */
SELECT TOP (5000)
    PLU,
    MAX(ISNULL(PLUName, N'')) AS PLUName,
    COUNT_BIG(*) AS RecordCount,
    COUNT(DISTINCT CONVERT(varchar(20), DeviceNo) + ':' + CONVERT(varchar(20), FID)) AS InvoiceCount,
    ISNULL(SUM(CONVERT(decimal(38, 3), TotalPrice)), 0) AS TotalPrice,
    ISNULL(SUM(CASE [Unit]
                  WHEN 0 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 1 THEN CONVERT(decimal(38, 3), Amount)
                  WHEN 3 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalWeight,
    ISNULL(SUM(CASE [Unit]
                  WHEN 2 THEN CONVERT(decimal(38, 3), Amount)
                  ELSE CONVERT(decimal(38, 3), 0)
             END), 0) AS TotalQuantity
FROM dbo.SADR_Logs
WHERE (@StartDate IS NULL OR [DateTime] >= @StartDate)
  AND (@EndDate IS NULL OR [DateTime] < @EndDate)
  AND (@Identify IS NULL OR Identify = @Identify)
  AND (@Plu IS NULL OR PLU = @Plu)
  AND (@ScaleId IS NULL OR DeviceNo = @ScaleId)
  AND (@Fid IS NULL OR FID = @Fid)
GROUP BY PLU
ORDER BY TotalPrice DESC, PLU;
