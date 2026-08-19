using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadrScales.Integration.Assignments;
using SadrScales.Integration.HotKeys;

namespace SadrScales.Integration.SqlTests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class AssignmentMappingHotKeySqlIntegrationTests
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
            Database.ExecuteNonQuery(ExtendedSchemaSql);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _database?.Dispose();
            _database = null;
        }

        #endregion

        #region Scale Assignment Tests

        [TestMethod]
        public async Task Scale_Assignment_Replace_Should_Be_Atomic_And_Reset_Item_State_Only_On_Change()
        {
            ResetData();
            SeedGroupsAndItems();
            InsertScale(1, 3, 2, 111, 222);
            Database.ExecuteNonQuery("INSERT INTO dbo.SADR_ScaleItemClass(ScaleID, ItemClassCode) VALUES(1, 'G1');");

            var client = CreateClient();
            SadrReplaceResult replaced = await client.ScaleAssignments.ReplaceGroupsAsync(1, new[] { "G2", "G1" });
            var groups = await client.ScaleAssignments.GetGroupsAsync(1);

            Assert.AreEqual(SadrReplaceResult.Replaced, replaced);
            CollectionAssert.AreEqual(new[] { "G1", "G2" }, groups.ToArray());
            Assert.AreEqual(0L, ReadScaleLong(1, "LastSendItem"));
            Assert.AreEqual(222L, ReadScaleLong(1, "LastSendKey"));

            Database.ExecuteNonQuery("UPDATE dbo.SADR_Scale SET LastSendItem = 999 WHERE ScaleID = 1;");
            SadrReplaceResult unchanged = await client.ScaleAssignments.ReplaceGroupsAsync(1, new[] { "G1", "G2" });

            Assert.AreEqual(SadrReplaceResult.Unchanged, unchanged);
            Assert.AreEqual(999L, ReadScaleLong(1, "LastSendItem"),
                "An unchanged assignment must not create a new resend request.");
            Assert.AreEqual(SadrReplaceResult.NotFound,
                await client.ScaleAssignments.ReplaceGroupsAsync(99, new[] { "G1" }));
        }

        #endregion

        #region Scale Mapping Tests

        [TestMethod]
        public async Task Scale_Mapping_Replace_Should_Validate_Layout_And_Reset_Both_Send_States()
        {
            ResetData();
            SeedGroupsAndItems();
            InsertScale(2, 3, 2, 100, 200);

            var client = CreateClient();
            var requested = new[]
            {
                new SadrScaleItemMap { PluNo = 1001, ItemCode = 1 },
                new SadrScaleItemMap { PluNo = 1002, ItemCode = 2, PageNo = 1, KeyNo = 2 }
            };

            SadrReplaceResult replaced = await client.ScaleMappings.ReplaceAsync(2, requested);
            var persisted = await client.ScaleMappings.GetAsync(2);

            Assert.AreEqual(SadrReplaceResult.Replaced, replaced);
            Assert.AreEqual(2, persisted.Count);
            Assert.AreEqual(2, persisted[1].PluNo);
            Assert.AreEqual(0L, ReadScaleLong(2, "LastSendItem"));
            Assert.AreEqual(0L, ReadScaleLong(2, "LastSendKey"));

            Database.ExecuteNonQuery("UPDATE dbo.SADR_Scale SET LastSendItem = 333, LastSendKey = 444 WHERE ScaleID = 2;");
            SadrReplaceResult unchanged = await client.ScaleMappings.ReplaceAsync(2, requested);

            Assert.AreEqual(SadrReplaceResult.Unchanged, unchanged);
            Assert.AreEqual(333L, ReadScaleLong(2, "LastSendItem"));
            Assert.AreEqual(444L, ReadScaleLong(2, "LastSendKey"));

            bool rejected = false;
            try
            {
                await client.ScaleMappings.ReplaceAsync(2, new[]
                {
                    new SadrScaleItemMap { PluNo = 1001, ItemCode = 10, PageNo = 2, KeyNo = 1 }
                });
            }
            catch (ArgumentOutOfRangeException)
            {
                rejected = true;
            }

            Assert.IsTrue(rejected, "A mapping outside the configured scale HotKey layout must be rejected.");
            Assert.AreEqual(2, (await client.ScaleMappings.GetAsync(2)).Count,
                "Rejected validation must not delete the existing mapping.");
        }

        [TestMethod]
        public async Task Scale_Mapping_Copy_Should_Reject_Incompatible_Destination_Without_Destroying_It()
        {
            ResetData();
            SeedGroupsAndItems();
            InsertScale(3, 3, 2, 1, 2);
            InsertScale(4, 1, 1, 3, 4);
            InsertScale(5, 3, 2, 5, 6);

            var client = CreateClient();
            await client.ScaleMappings.ReplaceAsync(3, new[]
            {
                new SadrScaleItemMap { PluNo = 1001, ItemCode = 11, PageNo = 1, KeyNo = 2 }
            });
            await client.ScaleMappings.ReplaceAsync(4, new[]
            {
                new SadrScaleItemMap { PluNo = 1002, ItemCode = 22, PageNo = 0, KeyNo = 1 }
            });

            bool rejected = false;
            try
            {
                await client.ScaleMappings.CopyAsync(3, 4);
            }
            catch (ArgumentOutOfRangeException)
            {
                rejected = true;
            }

            Assert.IsTrue(rejected);
            var destinationAfterReject = await client.ScaleMappings.GetAsync(4);
            Assert.AreEqual(1, destinationAfterReject.Count);
            Assert.AreEqual(1002, destinationAfterReject[0].PluNo,
                "Incompatible copy must leave destination mapping unchanged.");

            SadrReplaceResult copied = await client.ScaleMappings.CopyAsync(3, 5);
            var destination = await client.ScaleMappings.GetAsync(5);

            Assert.AreEqual(SadrReplaceResult.Replaced, copied);
            Assert.AreEqual(1, destination.Count);
            Assert.AreEqual(1001, destination[0].PluNo);
            Assert.AreEqual(11, destination[0].ItemCode);
            Assert.AreEqual(1, destination[0].PageNo);
            Assert.AreEqual(2, destination[0].KeyNo);
            Assert.AreEqual(0L, ReadScaleLong(5, "LastSendItem"));
            Assert.AreEqual(0L, ReadScaleLong(5, "LastSendKey"));
        }

        #endregion

        #region Group HotKey Tests

        [TestMethod]
        public async Task Group_HotKey_Replace_Should_Preserve_System_Rows_And_Reset_Only_Assigned_Scales()
        {
            ResetData();
            SeedGroupsAndItems();
            InsertScale(10, 108, 3, 100, 500);
            InsertScale(11, 108, 3, 200, 600);
            Database.ExecuteNonQuery(@"
INSERT INTO dbo.SADR_ScaleItemClass(ScaleID, ItemClassCode) VALUES(10, 'G1');
INSERT INTO dbo.SADR_ScaleItemClass(ScaleID, ItemClassCode) VALUES(11, 'G2');
INSERT INTO dbo.SADR_KeyAssignment(ItemClassCode, PageNo, KeyNo, PluNo) VALUES('G1', 0, 108, -32001);
INSERT INTO dbo.SADR_KeyAssignment(ItemClassCode, PageNo, KeyNo, PluNo) VALUES('G1', 0, 1, 1001);
");

            var client = CreateClient();
            SadrReplaceResult replaced = await client.HotKeys.ReplaceGroupAsync("G1", new[]
            {
                new SadrHotKey { PageNo = 0, KeyNo = 2, PluNo = 1002 }
            });
            var publicKeys = await client.HotKeys.GetGroupAsync("G1");

            Assert.AreEqual(SadrReplaceResult.Replaced, replaced);
            Assert.AreEqual(1, publicKeys.Count);
            Assert.AreEqual(2, publicKeys[0].KeyNo);
            Assert.AreEqual(1002, publicKeys[0].PluNo);
            Assert.AreEqual(1, Database.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM dbo.SADR_KeyAssignment WHERE ItemClassCode = 'G1' AND PluNo = -32001;"),
                "Public replace must preserve internal/system HotKey rows.");
            Assert.AreEqual(0L, ReadScaleLong(10, "LastSendKey"));
            Assert.AreEqual(600L, ReadScaleLong(11, "LastSendKey"));

            Database.ExecuteNonQuery("UPDATE dbo.SADR_Scale SET LastSendKey = 777 WHERE ScaleID = 10;");
            SadrReplaceResult unchanged = await client.HotKeys.ReplaceGroupAsync("G1", new[]
            {
                new SadrHotKey { PageNo = 0, KeyNo = 2, PluNo = 1002 }
            });

            Assert.AreEqual(SadrReplaceResult.Unchanged, unchanged);
            Assert.AreEqual(777L, ReadScaleLong(10, "LastSendKey"));

            SadrReplaceResult cleared = await client.HotKeys.ReplaceGroupAsync("G1", Array.Empty<SadrHotKey>());
            Assert.AreEqual(SadrReplaceResult.Replaced, cleared);
            Assert.AreEqual(0, (await client.HotKeys.GetGroupAsync("G1")).Count);
            Assert.AreEqual(1, Database.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM dbo.SADR_KeyAssignment WHERE ItemClassCode = 'G1' AND PluNo = -32001;"));
        }

        #endregion

        #region Test Data Helpers

        private static void ResetData()
        {
            Database.ExecuteNonQuery(@"
DELETE FROM dbo.SADR_ScaleItemMap;
DELETE FROM dbo.SADR_KeyAssignment;
DELETE FROM dbo.SADR_ScaleItemClass;
DELETE FROM dbo.SADR_Scale;
DELETE FROM dbo.SADR_Item;
DELETE FROM dbo.SADR_ItemClass WHERE ItemClassCode <> '0';
");
        }

        private static void SeedGroupsAndItems()
        {
            Database.ExecuteNonQuery(@"
INSERT INTO dbo.SADR_ItemClass(ItemClassCode, ItemClassName) VALUES('G1', N'Group 1');
INSERT INTO dbo.SADR_ItemClass(ItemClassCode, ItemClassName) VALUES('G2', N'Group 2');
INSERT INTO dbo.SADR_Item(ItemClassCode, PluNo, PluName, UnitPrice) VALUES('G1', 1001, N'Item 1001', 10000);
INSERT INTO dbo.SADR_Item(ItemClassCode, PluNo, PluName, UnitPrice) VALUES('G1', 1002, N'Item 1002', 20000);
INSERT INTO dbo.SADR_Item(ItemClassCode, PluNo, PluName, UnitPrice) VALUES('G2', 2001, N'Item 2001', 30000);
");
        }

        private static void InsertScale(
            int scaleId,
            int hotKeyCountPerPage,
            int hotKeyPageCount,
            long lastSendItem,
            long lastSendKey)
        {
            Database.ExecuteNonQuery(@"
INSERT INTO dbo.SADR_Scale
(
    ScaleID, Category, Status, HotKeyCountPerPage, HotKeyPageCount,
    LastSendItem, LastSendKey
)
VALUES
(" + scaleId + @", 'LSG', 'Online', " + hotKeyCountPerPage + ", " + hotKeyPageCount + @",
 " + lastSendItem + ", " + lastSendKey + ");");
        }

        private static long ReadScaleLong(int scaleId, string column)
        {
            return Database.ExecuteScalar<long>(
                "SELECT CONVERT(bigint, " + column + ") FROM dbo.SADR_Scale WHERE ScaleID = " + scaleId + ";");
        }

        #endregion

        #region Synthetic 5.2.1 Schema

        private const string ExtendedSchemaSql = @"
CREATE TABLE dbo.SADR_Scale
(
    ScaleID int NOT NULL,
    Category varchar(50) NULL,
    Status varchar(50) NULL,
    HotKeyCountPerPage smallint NULL,
    HotKeyPageCount tinyint NULL,
    LastSendItem bigint NULL,
    LastSendKey bigint NULL,
    CONSTRAINT PK_SADR_Scale_AssignmentTests PRIMARY KEY CLUSTERED (ScaleID ASC)
);

CREATE TABLE dbo.SADR_ScaleItemClass
(
    ScaleID int NOT NULL,
    ItemClassCode varchar(50) NOT NULL,
    CONSTRAINT PK_SADR_ScaleItemClass_AssignmentTests PRIMARY KEY CLUSTERED (ScaleID, ItemClassCode),
    CONSTRAINT FK_SADR_ScaleItemClass_Scale_AssignmentTests FOREIGN KEY(ScaleID)
        REFERENCES dbo.SADR_Scale(ScaleID) ON DELETE CASCADE,
    CONSTRAINT FK_SADR_ScaleItemClass_Group_AssignmentTests FOREIGN KEY(ItemClassCode)
        REFERENCES dbo.SADR_ItemClass(ItemClassCode)
);

CREATE TABLE dbo.SADR_ScaleItemMap
(
    ScaleID int NOT NULL,
    PluNo int NOT NULL,
    ItemCode int NOT NULL,
    PageNo int NULL,
    KeyNo int NULL,
    CONSTRAINT PK_SADR_ScaleItemMap_AssignmentTests PRIMARY KEY CLUSTERED (ScaleID, PluNo),
    CONSTRAINT FK_SADR_ScaleItemMap_Scale_AssignmentTests FOREIGN KEY(ScaleID)
        REFERENCES dbo.SADR_Scale(ScaleID) ON DELETE CASCADE,
    CONSTRAINT FK_SADR_ScaleItemMap_Item_AssignmentTests FOREIGN KEY(PluNo)
        REFERENCES dbo.SADR_Item(PluNo) ON DELETE CASCADE,
    CONSTRAINT CK_SADR_ScaleItemMap_ItemCode_AssignmentTests CHECK (ItemCode > 0),
    CONSTRAINT CK_SADR_ScaleItemMap_PageNo_AssignmentTests CHECK (PageNo IS NULL OR PageNo BETWEEN 0 AND 2),
    CONSTRAINT CK_SADR_ScaleItemMap_KeyNo_AssignmentTests CHECK (KeyNo IS NULL OR KeyNo > 0),
    CONSTRAINT CK_SADR_ScaleItemMap_HotKeyPair_AssignmentTests CHECK
    (
        (PageNo IS NULL AND KeyNo IS NULL)
        OR (PageNo IS NOT NULL AND KeyNo IS NOT NULL)
    )
);
CREATE UNIQUE NONCLUSTERED INDEX UX_SADR_ScaleItemMap_ItemCode_AssignmentTests
    ON dbo.SADR_ScaleItemMap(ScaleID, ItemCode);
CREATE UNIQUE NONCLUSTERED INDEX UX_SADR_ScaleItemMap_HotKey_AssignmentTests
    ON dbo.SADR_ScaleItemMap(ScaleID, PageNo, KeyNo)
    WHERE PageNo IS NOT NULL AND KeyNo IS NOT NULL;

CREATE TABLE dbo.SADR_KeyAssignment
(
    ItemClassCode varchar(50) NOT NULL,
    PageNo int NOT NULL,
    KeyNo int NOT NULL,
    PluNo int NOT NULL,
    [TimeStamp] timestamp NOT NULL,
    CONSTRAINT PK_SADR_KeyAssignment_AssignmentTests PRIMARY KEY CLUSTERED (ItemClassCode, PageNo, KeyNo),
    CONSTRAINT FK_SADR_KeyAssignment_Group_AssignmentTests FOREIGN KEY(ItemClassCode)
        REFERENCES dbo.SADR_ItemClass(ItemClassCode)
);
";

        #endregion
    }
}
