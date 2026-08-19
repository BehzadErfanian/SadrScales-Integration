using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadrScales.Integration.DemoLab;
using SadrScales.Integration.Invoices;
using SadrScales.Integration.Scales;

namespace SadrScales.Integration.SqlTests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class DemoDataWriterSqlTests
    {
        #region End-to-End Demo Writer

        [TestMethod]
        public async Task Guarded_Demo_Writer_Should_Create_Data_Consumable_By_The_Public_Sdk_And_Require_Reset()
        {
            using (SqlTestDatabase database = SqlTestDatabase.Create())
            {
                EnsureWriterSchema(database);

                var guard = new SadrDemoDatabaseGuard(database.ConnectionString);
                SadrDemoDatabaseSafety initial = await guard.InspectAsync();
                Assert.IsTrue(initial.CanInitializeMarker, initial.Message);
                await guard.InitializeMarkerAsync(database.DatabaseName);

                var options = new SadrDemoScenarioOptions
                {
                    Seed = 12345,
                    StoreCount = 2,
                    GroupCount = 2,
                    ItemCount = 10,
                    ScaleCount = 2,
                    InvoiceCount = 4
                };
                SadrDemoScenario expected = SadrDemoScenarioFactory.Create(options);
                var writer = new SadrDemoDataWriter(database.ConnectionString);

                SadrDemoGenerationResult generated = await writer.GenerateAsync(options);

                Assert.AreEqual(12345, generated.Seed);
                Assert.AreEqual(2, generated.StoreCount);
                Assert.AreEqual(2, generated.GroupCount);
                Assert.AreEqual(10, generated.ItemCount);
                Assert.AreEqual(2, generated.ScaleCount);
                Assert.AreEqual(4, generated.InvoiceCount);
                Assert.AreEqual(expected.Invoices.Sum(invoice => invoice.Lines.Count), generated.SalesRowCount);

                var client = new SadrScalesClient(database.ConnectionString);
                await client.ValidateAsync();

                var stores = await client.Stores.GetAllAsync();
                Assert.AreEqual(2, stores.Count(store => store.StoreCode.StartsWith("DEMO-S", StringComparison.Ordinal)));

                var groups = await client.ItemGroups.GetAllAsync();
                Assert.AreEqual(2, groups.Count(group => group.ItemClassCode.StartsWith("DEMO-G", StringComparison.Ordinal)));

                var items = await client.Items.GetAllAsync();
                Assert.AreEqual(10, items.Count);

                var scales = await client.Scales.GetAllAsync();
                Assert.AreEqual(2, scales.Count);
                Assert.IsTrue(scales.All(scale => scale.Status == SadrScaleStatus.Offline));
                CollectionAssert.AreEqual(new[] { 81, 82 }, scales.Select(scale => scale.ScaleId).ToArray());

                Assert.IsTrue((await client.ScaleAssignments.GetGroupsAsync(81)).Count >= 1);
                Assert.IsTrue((await client.ScaleMappings.GetAsync(81)).Count >= 1);
                Assert.IsTrue((await client.HotKeys.GetGroupAsync(expected.Groups[0].ItemClassCode)).Count >= 1);

                SadrDemoInvoice firstInvoice = expected.Invoices[0];
                SadrInvoiceLookupResult lookup = await client.Invoices.GetByBarcodeAsync(firstInvoice.TotalBarcode);
                Assert.AreEqual(SadrInvoiceLookupStatus.FoundUnread, lookup.Status);
                Assert.IsNotNull(lookup.Invoice);
                Assert.AreEqual(firstInvoice.Lines.Count, lookup.Invoice!.Items.Count);

                var sales = await client.Sales.QueryAsync();
                Assert.AreEqual((long)generated.SalesRowCount, sales.Summary.RecordCount);
                Assert.AreEqual((long)generated.InvoiceCount, sales.Summary.InvoiceCount);
                Assert.AreEqual(generated.InvoiceCount, (await client.Invoices.GetByBarcodeAsync(
                    expected.Invoices[3].TotalBarcode)).Invoice == null ? 0 : generated.InvoiceCount);
                Assert.IsTrue((await client.Reports.GetDailyAsync()).Count >= 1);
                Assert.IsTrue((await client.Reports.GetByScaleAsync()).Count >= 1);
                Assert.IsTrue((await client.Reports.GetByItemAsync()).Count >= 1);

                Assert.AreEqual(0, database.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM dbo.SADR_Scale WHERE Used <> 0 OR AutoSendItems <> 0 OR AutoGetInvoice <> 0 OR Status <> 'Offline';"),
                    "Demo scales must remain disabled/offline and must not trigger normal device traffic.");
                Assert.AreEqual(12345, database.ExecuteScalar<int>(
                    "SELECT LastSeed FROM dbo.SADR_IntegrationDemoMarker WHERE MarkerId = 1;"));

                await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                    () => writer.GenerateAsync(options));

                await guard.ResetDemoDataAsync();
                SadrDemoDatabaseSafety reset = await guard.InspectAsync();
                Assert.IsTrue(reset.IsBusinessDataEmpty);
                Assert.IsTrue(reset.CanWriteDemoData);

                SadrDemoGenerationResult regenerated = await writer.GenerateAsync(options);
                Assert.AreEqual(generated.SalesRowCount, regenerated.SalesRowCount);
                SadrInvoiceLookupResult regeneratedInvoice = await client.Invoices.GetByBarcodeAsync(firstInvoice.TotalBarcode);
                Assert.AreEqual(SadrInvoiceLookupStatus.FoundUnread, regeneratedInvoice.Status);
            }
        }

        #endregion

        #region Synthetic Migrated 5.2.1 Schema

        private static void EnsureWriterSchema(SqlTestDatabase database)
        {
            database.ExecuteNonQuery(@"
CREATE TABLE dbo.SADR_Store
(
    StoreCode varchar(50) NOT NULL,
    StoreName nvarchar(100) NULL,
    Descriptions nvarchar(150) NULL,
    CONSTRAINT PK_SADR_Store_DemoWriter PRIMARY KEY CLUSTERED (StoreCode ASC)
);
INSERT INTO dbo.SADR_Store(StoreCode, StoreName, Descriptions)
VALUES('0', N'Default', N'Default Store');

CREATE TABLE dbo.SADR_Scale
(
    ScaleID int NOT NULL,
    Port int NOT NULL CONSTRAINT DF_Demo_Scale_Port DEFAULT(5000),
    Mac varchar(50) NULL,
    StoreCode varchar(50) NULL CONSTRAINT DF_Demo_Scale_StoreCode DEFAULT('0'),
    ItemClassCode varchar(50) NULL CONSTRAINT DF_Demo_Scale_Group DEFAULT('0'),
    Category nvarchar(50) NULL,
    GroupName nvarchar(50) NULL,
    Version varchar(50) NULL,
    DeviceName nvarchar(50) NULL,
    StoreName nvarchar(50) NULL,
    ScaleIP varchar(20) NULL,
    AutoSendItems int NULL CONSTRAINT DF_Demo_Scale_AutoSend DEFAULT(0),
    Status nvarchar(50) NULL,
    LastSendItem bigint NOT NULL CONSTRAINT DF_Demo_Scale_LastItem DEFAULT(0),
    LastSendKey bigint NOT NULL CONSTRAINT DF_Demo_Scale_LastKey DEFAULT(0),
    LastReceiveFID int NOT NULL CONSTRAINT DF_Demo_Scale_LastFid DEFAULT(0),
    AutoGetInvoice int NULL CONSTRAINT DF_Demo_Scale_AutoGet DEFAULT(0),
    SendScaleDetail int NULL CONSTRAINT DF_Demo_Scale_SendDetail DEFAULT(0),
    GetScaleDetail int NULL CONSTRAINT DF_Demo_Scale_GetDetail DEFAULT(0),
    HotKeyCountPerPage smallint NOT NULL CONSTRAINT DF_Demo_Scale_KeyCount DEFAULT(40),
    HotKeyPageCount tinyint NOT NULL CONSTRAINT DF_Demo_Scale_PageCount DEFAULT(3),
    Used int NULL CONSTRAINT DF_Demo_Scale_Used DEFAULT(0),
    CONSTRAINT PK_SADR_Scale_DemoWriter PRIMARY KEY CLUSTERED (ScaleID ASC),
    CONSTRAINT FK_Demo_Scale_Store FOREIGN KEY(StoreCode) REFERENCES dbo.SADR_Store(StoreCode),
    CONSTRAINT FK_Demo_Scale_Group FOREIGN KEY(ItemClassCode) REFERENCES dbo.SADR_ItemClass(ItemClassCode)
);
CREATE UNIQUE INDEX UX_Demo_Scale_IP_Port ON dbo.SADR_Scale(ScaleIP, Port) WHERE ScaleIP IS NOT NULL;

CREATE TABLE dbo.SADR_ScaleItemClass
(
    ScaleID int NOT NULL,
    ItemClassCode varchar(50) NOT NULL,
    CONSTRAINT PK_Demo_ScaleItemClass PRIMARY KEY CLUSTERED (ScaleID, ItemClassCode),
    CONSTRAINT FK_Demo_ScaleItemClass_Scale FOREIGN KEY(ScaleID) REFERENCES dbo.SADR_Scale(ScaleID) ON DELETE CASCADE,
    CONSTRAINT FK_Demo_ScaleItemClass_Group FOREIGN KEY(ItemClassCode) REFERENCES dbo.SADR_ItemClass(ItemClassCode)
);

CREATE TABLE dbo.SADR_ScaleItemMap
(
    ScaleID int NOT NULL,
    PluNo int NOT NULL,
    ItemCode int NOT NULL,
    PageNo int NULL,
    KeyNo int NULL,
    CONSTRAINT PK_Demo_ScaleItemMap PRIMARY KEY CLUSTERED (ScaleID, PluNo),
    CONSTRAINT FK_Demo_ScaleItemMap_Scale FOREIGN KEY(ScaleID) REFERENCES dbo.SADR_Scale(ScaleID) ON DELETE CASCADE,
    CONSTRAINT FK_Demo_ScaleItemMap_Item FOREIGN KEY(PluNo) REFERENCES dbo.SADR_Item(PluNo) ON DELETE CASCADE,
    CONSTRAINT CK_Demo_ScaleItemMap_ItemCode CHECK(ItemCode > 0),
    CONSTRAINT CK_Demo_ScaleItemMap_Page CHECK(PageNo IS NULL OR PageNo BETWEEN 0 AND 2),
    CONSTRAINT CK_Demo_ScaleItemMap_Key CHECK(KeyNo IS NULL OR KeyNo > 0),
    CONSTRAINT CK_Demo_ScaleItemMap_Pair CHECK((PageNo IS NULL AND KeyNo IS NULL) OR (PageNo IS NOT NULL AND KeyNo IS NOT NULL))
);
CREATE UNIQUE INDEX UX_Demo_ScaleItemMap_ItemCode ON dbo.SADR_ScaleItemMap(ScaleID, ItemCode);
CREATE UNIQUE INDEX UX_Demo_ScaleItemMap_HotKey ON dbo.SADR_ScaleItemMap(ScaleID, PageNo, KeyNo)
WHERE PageNo IS NOT NULL AND KeyNo IS NOT NULL;

CREATE TABLE dbo.SADR_KeyAssignment
(
    ItemClassCode varchar(50) NOT NULL,
    PageNo int NOT NULL,
    KeyNo int NOT NULL,
    PluNo int NOT NULL,
    [TimeStamp] timestamp NOT NULL,
    CONSTRAINT PK_Demo_KeyAssignment PRIMARY KEY CLUSTERED (ItemClassCode, PageNo, KeyNo),
    CONSTRAINT FK_Demo_KeyAssignment_Group FOREIGN KEY(ItemClassCode) REFERENCES dbo.SADR_ItemClass(ItemClassCode)
);

CREATE TABLE dbo.SADR_Total
(
    TotalID int IDENTITY(1,1) NOT NULL,
    ScaleID int NOT NULL,
    SaleDateTime smalldatetime NULL,
    LableStatus int NULL CONSTRAINT DF_Demo_Total_Status DEFAULT(0),
    ReceiptNo int NOT NULL,
    TotalBarcode varchar(50) NULL,
    ItemBarcode varchar(50) NOT NULL,
    NTrans int NULL,
    SubDiscAmt int NULL CONSTRAINT DF_Demo_Total_SubDisc DEFAULT(0),
    DiscAmt int NULL CONSTRAINT DF_Demo_Total_Disc DEFAULT(0),
    AmtOfATax int NULL CONSTRAINT DF_Demo_Total_ATax DEFAULT(0),
    AmtOfVTax int NULL CONSTRAINT DF_Demo_Total_VTax DEFAULT(0),
    PriceWTax int NULL CONSTRAINT DF_Demo_Total_Price DEFAULT(0),
    ClerkNo int NULL,
    CONSTRAINT PK_Demo_Total_ItemBarcode PRIMARY KEY CLUSTERED (ItemBarcode ASC),
    CONSTRAINT UX_Demo_Total_TotalID UNIQUE NONCLUSTERED (TotalID),
    CONSTRAINT FK_Demo_Total_Scale FOREIGN KEY(ScaleID) REFERENCES dbo.SADR_Scale(ScaleID)
);

CREATE TABLE dbo.SADR_Detail
(
    DetailID int IDENTITY(1,1) NOT NULL,
    TotalID int NOT NULL,
    TotalBarcode varchar(50) NULL,
    ItemBarcode varchar(50) NOT NULL,
    TransNo int NULL,
    PluNo int NOT NULL,
    Weight float NULL CONSTRAINT DF_Demo_Detail_Weight DEFAULT(0),
    QTY float NULL CONSTRAINT DF_Demo_Detail_Qty DEFAULT(0),
    Uprice int NULL,
    UpriceAfDisc int NULL CONSTRAINT DF_Demo_Detail_UpriceAfter DEFAULT(0),
    StPointDiscStat int NULL CONSTRAINT DF_Demo_Detail_Stat DEFAULT(0),
    TTLPriceDiscAmt int NULL CONSTRAINT DF_Demo_Detail_Disc DEFAULT(0),
    ActPrice int NULL,
    TaxRtNo int NULL CONSTRAINT DF_Demo_Detail_Tax DEFAULT(0),
    ItemStatus int NULL,
    CONSTRAINT PK_Demo_Detail PRIMARY KEY CLUSTERED (DetailID ASC),
    CONSTRAINT FK_Demo_Detail_Total FOREIGN KEY(TotalID) REFERENCES dbo.SADR_Total(TotalID)
);
CREATE INDEX IX_Demo_Detail_Total ON dbo.SADR_Detail(TotalID);

CREATE TABLE dbo.SADR_PriceLog
(
    ID int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PluNo int NOT NULL,
    IndexBarcode varchar(50) NULL,
    PluName nvarchar(100) NULL,
    LastPrice int NOT NULL CONSTRAINT DF_Demo_PriceLog_Last DEFAULT(0),
    NewPrice int NOT NULL CONSTRAINT DF_Demo_PriceLog_New DEFAULT(0),
    [DateTime] datetime NOT NULL CONSTRAINT DF_Demo_PriceLog_Date DEFAULT(GETDATE()),
    [User] nvarchar(100) NULL
);
");
        }

        #endregion
    }
}
