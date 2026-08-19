using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadrScales.Integration.Scales;

namespace SadrScales.Integration.SqlTests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class ScaleSqlIntegrationTests
    {
        #region Test Database

        private static SqlTestDatabase? _database;

        private static SqlTestDatabase Database =>
            _database ?? throw new InvalidOperationException("SQL test database is not initialized.");

        private static SadrScalesClient CreateClient() => new SadrScalesClient(Database.ConnectionString);

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _database = SqlTestDatabase.Create();
            Database.ExecuteNonQuery(ScaleSchemaSql);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _database?.Dispose();
            _database = null;
        }

        #endregion

        #region Read Tests

        [TestMethod]
        public async Task Scale_Read_Should_Map_Public_Metadata_And_Coarse_Status()
        {
            ResetScales();
            InsertScale(3, "LSG", "Online", 25, 35);
            InsertScale(4, "PLUS", "Offline", 40, 50);
            InsertScale(5, "LSG", "Connecting", 60, 70);

            var client = CreateClient();
            var scales = await client.Scales.GetAllAsync();
            var scale3 = await client.Scales.GetAsync(3);
            var status4 = await client.Scales.GetStatusAsync(4);
            var status5 = await client.Scales.GetStatusAsync(5);
            var missingStatus = await client.Scales.GetStatusAsync(99);

            Assert.AreEqual(3, scales.Count);
            Assert.IsNotNull(scale3);
            Assert.AreEqual(3, scale3!.ScaleId);
            Assert.AreEqual("192.168.10.3", scale3.IpAddress);
            Assert.AreEqual(5000, scale3.Port);
            Assert.AreEqual("LSG", scale3.Model);
            Assert.AreEqual("Scale 3", scale3.DeviceName);
            Assert.AreEqual("S1", scale3.StoreCode);
            Assert.AreEqual("Store 1", scale3.StoreName);
            Assert.AreEqual("G1", scale3.PrimaryItemGroupCode);
            Assert.IsTrue(scale3.Used);
            Assert.IsTrue(scale3.AutoSendItems);
            Assert.IsFalse(scale3.AutoGetInvoice);
            Assert.AreEqual(SadrScaleStatus.Online, scale3.Status);
            Assert.AreEqual(108, scale3.HotKeyCountPerPage);
            Assert.AreEqual(3, scale3.HotKeyPageCount);

            Assert.AreEqual(SadrScaleStatus.Offline, status4);
            Assert.AreEqual(SadrScaleStatus.Unknown, status5,
                "Transient/internal status text must not leak as a new public enum value.");
            Assert.AreEqual(SadrScaleStatus.Unknown, missingStatus);
        }

        #endregion

        #region Resend Tests

        [TestMethod]
        public async Task Item_Resend_Should_Reset_Item_Watermark_Only_And_Report_Missing()
        {
            ResetScales();
            InsertScale(10, "PLUS", "Online", 1234, 5678);

            var client = CreateClient();
            var requested = await client.Scales.RequestItemResendAsync(10);
            var missing = await client.Scales.RequestItemResendAsync(11);

            Assert.AreEqual(SadrResendRequestResult.Requested, requested);
            Assert.AreEqual(SadrResendRequestResult.NotFound, missing);
            Assert.AreEqual(0L, Database.ExecuteScalar<long>(
                "SELECT CONVERT(bigint, LastSendItem) FROM dbo.SADR_Scale WHERE ScaleID = 10;"));
            Assert.AreEqual(5678L, Database.ExecuteScalar<long>(
                "SELECT CONVERT(bigint, LastSendKey) FROM dbo.SADR_Scale WHERE ScaleID = 10;"));
        }

        [TestMethod]
        public async Task HotKey_Resend_Should_Reset_Supported_Model_And_Reject_Plus()
        {
            ResetScales();
            InsertScale(20, "LSG", "Online", 100, 200);
            InsertScale(21, "PLUS", "Online", 300, 400);

            var client = CreateClient();
            var lsg = await client.Scales.RequestHotKeyResendAsync(20);
            var plus = await client.Scales.RequestHotKeyResendAsync(21);
            var missing = await client.Scales.RequestHotKeyResendAsync(22);

            Assert.AreEqual(SadrResendRequestResult.Requested, lsg);
            Assert.AreEqual(SadrResendRequestResult.UnsupportedModel, plus);
            Assert.AreEqual(SadrResendRequestResult.NotFound, missing);

            Assert.AreEqual(0L, Database.ExecuteScalar<long>(
                "SELECT CONVERT(bigint, LastSendKey) FROM dbo.SADR_Scale WHERE ScaleID = 20;"));
            Assert.AreEqual(400L, Database.ExecuteScalar<long>(
                "SELECT CONVERT(bigint, LastSendKey) FROM dbo.SADR_Scale WHERE ScaleID = 21;"));
        }

        #endregion

        #region Test Data Helpers

        private static void ResetScales()
        {
            Database.ExecuteNonQuery("DELETE FROM dbo.SADR_Scale;");
        }

        private static void InsertScale(int scaleId, string model, string status, long lastSendItem, long lastSendKey)
        {
            Database.ExecuteNonQuery(@"
INSERT INTO dbo.SADR_Scale
(
    ScaleID, Port, Mac, ItemClassCode, Version, DeviceName, StoreName,
    ScaleIP, AutoSendItems, Status, AutoGetInvoice, Category, StoreCode,
    HotKeyCountPerPage, HotKeyPageCount, Used, LastSendItem, LastSendKey
)
VALUES
(" + scaleId + @", 5000, '00:11:22:33:44:55', 'G1', '5.2.1', N'Scale " + scaleId + @"', N'Store 1',
 '192.168.10." + scaleId + @"', 1, '" + status + "', 0, '" + model + @"', 'S1',
 108, 3, 1, " + lastSendItem + ", " + lastSendKey + ");");
        }

        #endregion

        #region Synthetic 5.2.1 Scale Schema

        private const string ScaleSchemaSql = @"
CREATE TABLE dbo.SADR_Scale
(
    ScaleID int NOT NULL,
    Port int NULL,
    Mac varchar(50) NULL,
    ItemClassCode varchar(50) NULL,
    Version varchar(50) NULL,
    DeviceName nvarchar(100) NULL,
    StoreName nvarchar(100) NULL,
    ScaleIP varchar(50) NULL,
    AutoSendItems bit NULL,
    Status varchar(50) NULL,
    AutoGetInvoice bit NULL,
    Category varchar(50) NULL,
    StoreCode varchar(50) NULL,
    HotKeyCountPerPage smallint NULL,
    HotKeyPageCount tinyint NULL,
    Used bit NULL,
    LastSendItem bigint NULL,
    LastSendKey bigint NULL,
    CONSTRAINT PK_SADR_Scale PRIMARY KEY CLUSTERED (ScaleID ASC)
);
";

        #endregion
    }
}
