using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadrScales.Integration.DemoLab;

namespace SadrScales.Integration.SqlTests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class DemoDatabaseGuardSqlTests
    {
        #region Marker Eligibility

        [TestMethod]
        public async Task Empty_CI_Database_Should_Be_Eligible_For_Demo_Marker_And_Require_Exact_Name()
        {
            using (SqlTestDatabase database = SqlTestDatabase.Create())
            {
                EnsureGuardSchema(database);
                var guard = new SadrDemoDatabaseGuard(database.ConnectionString);

                SadrDemoDatabaseSafety safety = await guard.InspectAsync();

                Assert.IsTrue(safety.HasSafeDemoName, database.DatabaseName);
                Assert.IsTrue(safety.HasRequiredSchema);
                Assert.IsTrue(safety.IsBusinessDataEmpty);
                Assert.IsFalse(safety.HasDemoMarker);
                Assert.IsTrue(safety.CanInitializeMarker);
                Assert.IsFalse(safety.CanWriteDemoData);

                await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                    () => guard.InitializeMarkerAsync(database.DatabaseName + "_WRONG"));

                await guard.InitializeMarkerAsync(database.DatabaseName);
                SadrDemoDatabaseSafety marked = await guard.InspectAsync();

                Assert.IsTrue(marked.HasDemoMarker);
                Assert.IsTrue(marked.CanWriteDemoData);
            }
        }

        [TestMethod]
        public async Task Unmarked_Database_With_Business_Data_Must_Not_Become_Demo_Database()
        {
            using (SqlTestDatabase database = SqlTestDatabase.Create())
            {
                EnsureGuardSchema(database);
                database.ExecuteNonQuery(@"
INSERT INTO dbo.SADR_Item(ItemClassCode, PluNo, PluName)
VALUES('0', 700001, N'Real-looking row');");

                var guard = new SadrDemoDatabaseGuard(database.ConnectionString);
                SadrDemoDatabaseSafety safety = await guard.InspectAsync();

                Assert.IsFalse(safety.IsBusinessDataEmpty);
                Assert.IsFalse(safety.CanInitializeMarker);
                Assert.IsFalse(safety.CanWriteDemoData);
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                    () => guard.InitializeMarkerAsync(database.DatabaseName));
            }
        }

        #endregion

        #region Marked Reset

        [TestMethod]
        public async Task Marked_Demo_Database_Should_Allow_Data_Then_Reset_It_Without_Removing_Marker_Or_Defaults()
        {
            using (SqlTestDatabase database = SqlTestDatabase.Create())
            {
                EnsureGuardSchema(database);
                var guard = new SadrDemoDatabaseGuard(database.ConnectionString);
                await guard.InitializeMarkerAsync(database.DatabaseName);

                database.ExecuteNonQuery(@"
INSERT INTO dbo.SADR_Store(StoreCode, StoreName) VALUES('DEMO-S01', N'Demo');
INSERT INTO dbo.SADR_ItemClass(ItemClassCode, ItemClassName) VALUES('DEMO-G01', N'Demo');
INSERT INTO dbo.SADR_Item(ItemClassCode, PluNo, PluName) VALUES('DEMO-G01', 900001, N'Demo Item');
INSERT INTO dbo.SADR_Scale(ScaleID) VALUES(81);
INSERT INTO dbo.SADR_ScaleItemClass(ScaleID, ItemClassCode) VALUES(81, 'DEMO-G01');
INSERT INTO dbo.SADR_ScaleItemMap(ScaleID, PluNo, ItemCode) VALUES(81, 900001, 1);
INSERT INTO dbo.SADR_KeyAssignment(ItemClassCode, PageNo, KeyNo, PluNo) VALUES('DEMO-G01', 0, 1, 900001);
INSERT INTO dbo.SADR_PriceLog(PluNo) VALUES(900001);
");

                SadrDemoDatabaseSafety populated = await guard.InspectAsync();
                Assert.IsTrue(populated.HasDemoMarker);
                Assert.IsTrue(populated.CanWriteDemoData,
                    "A valid marker, not emptiness, authorizes later DemoLab reset/generation.");
                Assert.IsFalse(populated.IsBusinessDataEmpty);

                await guard.ResetDemoDataAsync();

                SadrDemoDatabaseSafety reset = await guard.InspectAsync();
                Assert.IsTrue(reset.HasDemoMarker);
                Assert.IsTrue(reset.IsBusinessDataEmpty);
                Assert.IsTrue(reset.CanWriteDemoData);
                Assert.AreEqual(1, database.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM dbo.SADR_ItemClass WHERE ItemClassCode = '0';"));
                Assert.AreEqual(1, database.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM dbo.SADR_Store WHERE StoreCode = '0';"));
                Assert.AreEqual(1, database.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM dbo.SADR_IntegrationDemoMarker WHERE MarkerId = 1;"));
            }
        }

        #endregion

        #region Schema Helper

        private static void EnsureGuardSchema(SqlTestDatabase database)
        {
            database.ExecuteNonQuery(@"
CREATE TABLE dbo.SADR_Store
(
    StoreCode varchar(50) NOT NULL PRIMARY KEY,
    StoreName nvarchar(100) NULL,
    Descriptions nvarchar(150) NULL
);
INSERT INTO dbo.SADR_Store(StoreCode, StoreName) VALUES('0', N'Default');

CREATE TABLE dbo.SADR_Scale
(
    ScaleID int NOT NULL PRIMARY KEY
);

CREATE TABLE dbo.SADR_ScaleItemClass
(
    ScaleID int NOT NULL,
    ItemClassCode varchar(50) NOT NULL,
    PRIMARY KEY(ScaleID, ItemClassCode)
);

CREATE TABLE dbo.SADR_ScaleItemMap
(
    ScaleID int NOT NULL,
    PluNo int NOT NULL,
    ItemCode int NOT NULL,
    PageNo int NULL,
    KeyNo int NULL,
    PRIMARY KEY(ScaleID, PluNo)
);

CREATE TABLE dbo.SADR_KeyAssignment
(
    ItemClassCode varchar(50) NOT NULL,
    PageNo int NOT NULL,
    KeyNo int NOT NULL,
    PluNo int NOT NULL,
    PRIMARY KEY(ItemClassCode, PageNo, KeyNo)
);

CREATE TABLE dbo.SADR_Total
(
    TotalID int IDENTITY(1,1) NOT NULL PRIMARY KEY
);

CREATE TABLE dbo.SADR_Detail
(
    DetailID int IDENTITY(1,1) NOT NULL PRIMARY KEY
);

CREATE TABLE dbo.SADR_PriceLog
(
    ID int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PluNo int NOT NULL
);
");
        }

        #endregion
    }
}
