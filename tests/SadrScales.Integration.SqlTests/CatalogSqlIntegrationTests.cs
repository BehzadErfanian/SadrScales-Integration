using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadrScales.Integration.Items;
using SadrScales.Integration.Stores;

namespace SadrScales.Integration.SqlTests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class CatalogSqlIntegrationTests
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
            Database.ExecuteNonQuery(CatalogExtensionSchemaSql);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _database?.Dispose();
            _database = null;
        }

        #endregion

        #region Store Tests

        [TestMethod]
        public async Task Store_Upsert_And_Read_Should_Use_StoreCode_As_Stable_Identity()
        {
            Database.ExecuteNonQuery("DELETE FROM dbo.SADR_Store WHERE StoreCode <> '0';");

            var client = CreateClient();
            var store = new SadrStore
            {
                StoreCode = "S10",
                StoreName = "North Branch",
                Descriptions = "Initial"
            };

            var inserted = await client.Stores.UpsertAsync(store);
            var unchanged = await client.Stores.UpsertAsync(store);

            store.StoreName = "North Branch Updated";
            var updated = await client.Stores.UpsertAsync(store);

            var loaded = await client.Stores.GetAsync("S10");
            var all = await client.Stores.GetAllAsync();

            Assert.AreEqual(SadrStoreUpsertResult.Inserted, inserted);
            Assert.AreEqual(SadrStoreUpsertResult.Unchanged, unchanged);
            Assert.AreEqual(SadrStoreUpsertResult.Updated, updated);
            Assert.IsNotNull(loaded);
            Assert.AreEqual("S10", loaded!.StoreCode);
            Assert.AreEqual("North Branch Updated", loaded.StoreName);
            Assert.IsTrue(all.Any(x => x.StoreCode == "0"), "The default store must remain visible.");
            Assert.IsTrue(all.Any(x => x.StoreCode == "S10"));
        }

        #endregion

        #region Group And Item Read Tests

        [TestMethod]
        public async Task Group_And_Item_Reads_Should_Expose_Active_And_Optional_Deleted_Rows()
        {
            ResetCatalogItems();

            var client = CreateClient();
            const string groupCode = "CATALOG";

            await client.ItemGroups.UpsertAsync(new SadrItemGroup
            {
                ItemClassCode = groupCode,
                ItemClassName = "Catalog Test"
            });

            await client.Items.UpsertAsync(new SadrItem
            {
                ItemClassCode = groupCode,
                PluNo = 810001,
                PluName = "Active Item",
                UnitPrice = 1000,
                DeleteFlag = 0
            });

            await client.Items.UpsertAsync(new SadrItem
            {
                ItemClassCode = groupCode,
                PluNo = 810002,
                PluName = "Deleted Item",
                UnitPrice = 2000,
                DeleteFlag = 1
            });

            var group = await client.ItemGroups.GetAsync(groupCode);
            var groups = await client.ItemGroups.GetAllAsync();
            var active = await client.Items.GetAllAsync();
            var all = await client.Items.GetAllAsync(includeDeleted: true);
            var deleted = await client.Items.GetAsync(810002);

            Assert.IsNotNull(group);
            Assert.AreEqual("Catalog Test", group!.ItemClassName);
            Assert.IsTrue(groups.Any(x => x.ItemClassCode == groupCode));

            Assert.AreEqual(1, active.Count(x => x.ItemClassCode == groupCode));
            Assert.AreEqual(2, all.Count(x => x.ItemClassCode == groupCode));
            Assert.IsNotNull(deleted);
            Assert.AreEqual(1, deleted!.DeleteFlag);
            Assert.AreEqual("Deleted Item", deleted.PluName);
        }

        #endregion

        #region Soft Delete Tests

        [TestMethod]
        public async Task Item_SoftDelete_Should_Be_Idempotent_And_Keep_Row_Inspectable()
        {
            ResetCatalogItems();

            var client = CreateClient();
            const string groupCode = "DELETE";

            await client.ItemGroups.UpsertAsync(new SadrItemGroup
            {
                ItemClassCode = groupCode,
                ItemClassName = "Delete Test"
            });

            await client.Items.UpsertAsync(new SadrItem
            {
                ItemClassCode = groupCode,
                PluNo = 820001,
                PluName = "Logical Delete",
                UnitPrice = 5000
            });

            var first = await client.Items.SoftDeleteAsync(820001);
            var second = await client.Items.SoftDeleteAsync(820001);
            var missing = await client.Items.SoftDeleteAsync(829999);
            var row = await client.Items.GetAsync(820001);
            var active = await client.Items.GetAllAsync();

            Assert.AreEqual(SadrItemSoftDeleteResult.Deleted, first);
            Assert.AreEqual(SadrItemSoftDeleteResult.AlreadyDeleted, second);
            Assert.AreEqual(SadrItemSoftDeleteResult.NotFound, missing);
            Assert.IsNotNull(row, "Logical delete must not physically remove the PLU row.");
            Assert.AreEqual(1, row!.DeleteFlag);
            Assert.IsFalse(active.Any(x => x.PluNo == 820001), "Default active catalog read must hide deleted PLUs.");
        }

        #endregion

        #region Price History Tests

        [TestMethod]
        public async Task PriceHistory_Read_Should_Filter_And_Order_Without_Mutating_Source()
        {
            Database.ExecuteNonQuery("DELETE FROM dbo.SADR_PriceLog;");
            Database.ExecuteNonQuery(@"
INSERT INTO dbo.SADR_PriceLog(PluNo, IndexBarcode, PluName, LastPrice, NewPrice, [DateTime], [User])
VALUES
(830001, 'B1', N'Apple', 1000, 1200, '2026-08-17T09:00:00', N'UserA'),
(830002, 'B2', N'Orange', 2000, 2200, '2026-08-18T10:00:00', N'UserB'),
(830001, 'B1', N'Apple', 1200, 1500, '2026-08-19T11:00:00', N'UserC');");

            int before = Database.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.SADR_PriceLog;");
            var client = CreateClient();
            var apple = await client.Items.GetPriceHistoryAsync(830001, 10);
            var recent = await client.Items.GetRecentPriceHistoryAsync(2);
            int after = Database.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.SADR_PriceLog;");

            Assert.AreEqual(2, apple.Count);
            Assert.AreEqual(1500, apple[0].NewPrice);
            Assert.AreEqual(1200, apple[1].NewPrice);
            Assert.AreEqual(2, recent.Count);
            Assert.AreEqual(830001, recent[0].PluNo);
            Assert.AreEqual(830002, recent[1].PluNo);
            Assert.AreEqual(before, after, "Price-history API is read-only in the 1.1.0 Vendor-Ready contract.");
        }

        #endregion

        #region Helpers

        private static void ResetCatalogItems()
        {
            Database.ExecuteNonQuery(@"
DELETE FROM dbo.SADR_Item;
DELETE FROM dbo.SADR_ItemClass WHERE ItemClassCode <> '0';");
        }

        #endregion

        #region Synthetic Schema Extension

        private const string CatalogExtensionSchemaSql = @"
CREATE TABLE dbo.SADR_Store
(
    StoreCode varchar(50) NOT NULL,
    StoreName nvarchar(100) NULL,
    Descriptions nvarchar(150) NULL,
    CONSTRAINT PK_SADR_Store PRIMARY KEY CLUSTERED (StoreCode ASC)
);
INSERT INTO dbo.SADR_Store(StoreCode, StoreName, Descriptions)
VALUES('0', N'پیشفرض', N'شعبه پیشفرض');

CREATE TABLE dbo.SADR_PriceLog
(
    ID int IDENTITY(1,1) NOT NULL,
    PluNo int NOT NULL,
    IndexBarcode varchar(50) NULL,
    PluName nvarchar(100) NULL,
    LastPrice int NOT NULL,
    NewPrice int NOT NULL,
    [DateTime] datetime NOT NULL,
    [User] nvarchar(100) NULL CONSTRAINT DF_SADR_PriceLog_User DEFAULT('Admin')
);
";

        #endregion
    }
}
