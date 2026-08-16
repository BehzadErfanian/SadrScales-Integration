/*
  Sadr Scales SQL Contract v1 - incremental read-only sales feed

  The variables are synthetic stand-ins. Application code must supply real query parameters.
  Persist the returned rows in the destination system first. Advance your destination-owned
  cursor only after that destination commit succeeds.
*/
SET NOCOUNT ON;

DECLARE @LastProcessedId bigint = 0;
DECLARE @BatchSize int = 100;

IF @BatchSize < 1 OR @BatchSize > 5000
    THROW 51201, 'BatchSize must be between 1 and 5000 for this sample.', 1;

SELECT TOP (@BatchSize)
    ID,
    DeviceNo,
    Identify,
    [DateTime],
    FID,
    SID,
    Salesman,
    SubID,
    TotalPrice,
    PLU,
    Class,
    Dept,
    Amount,
    Unit,
    LogType,
    Tax,
    Text1,
    Text2,
    Text3,
    Text4,
    UnitPrice,
    CoFID,
    PLUName
FROM dbo.SADR_Logs
WHERE ID > @LastProcessedId
ORDER BY ID ASC;

/*
Destination rules:
1. Treat (DeviceNo, FID, SubID) as the preferred stable duplicate key.
2. IDs may contain gaps; never require ID = previous + 1.
3. Do not UPDATE or DELETE dbo.SADR_Logs to acknowledge the basic feed.
4. Store LastProcessedId in your POS/ERP/accounting database after destination commit.
*/
