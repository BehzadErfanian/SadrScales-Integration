using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadrScales.Integration.Exceptions;
using SadrScales.Integration.Items;

namespace SadrScales.Integration.SqlTests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class SdkSqlIntegrationTests
    {
        private static SqlTestDatabase? _database;

        private static SqlTestDatabase Database =>
            _database ?? throw new InvalidOperationException("SQL test database is not initialized.");

        private static SadrScalesClient CreateClient() => new SadrScalesClient(Database.ConnectionString);

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _database = SqlTestDatabase.Create();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _database?.Dispose();
            _database = null;
        }

        [TestMethod]
        public async Task Contract_Validation_Should_Pass_Against_Frozen_Synthetic_Schema()
        {
            await CreateClient().ValidateAsync();
        }

        [TestMethod]
        public async Task ItemGroup_Upsert_Should_Report_Inserted_Unchanged_And_Updated()
        {
            var client = CreateClient();
            var code = "G" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var group = new SadrItemGroup
            {
                ItemClassCode = code,
                ItemClassName = "Initial",
                Descriptions = "Synthetic SQL integration test"
            };

            var inserted = await client.ItemGroups.UpsertAsync(group);
            var unchanged = await client.ItemGroups.UpsertAsync(group);

            group.ItemClassName = "Updated";
            var updated = await client.ItemGroups.UpsertAsync(group);

            Assert.AreEqual(SadrWriteOperation.Inserted, inserted.Operation);
            Assert.AreEqual(SadrWriteOperation.Unchanged, unchanged.Operation);
            Assert.AreEqual(SadrWriteOperation.Updated, updated.Operation);
        }

        [TestMethod]
        public async Task Item_Upsert_Should_Preserve_RowVersion_When_Semantically_Unchanged()
        {
            var client = CreateClient();
            var groupCode = "P" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var pluNo = 900000 + Math.Abs(Guid.NewGuid().GetHashCode() % 90000) + 1;

            await client.ItemGroups.UpsertAsync(new SadrItemGroup
            {
                ItemClassCode = groupCode,
                ItemClassName = "PLU Test"
            });

            var item = new SadrItem
            {
                ItemClassCode = groupCode,
                PluNo = pluNo,
                PluUnit = 3,
                UnitPrice = 125000,
                PluName = "Synthetic Apple",
                Text1 = "Integration Test"
            };

            var inserted = await client.Items.UpsertAsync(item);
            var rowVersionAfterInsert = Database.ReadItemRowVersion(pluNo);

            var unchanged = await client.Items.UpsertAsync(item);
            var rowVersionAfterUnchanged = Database.ReadItemRowVersion(pluNo);

            item.UnitPrice += 1000;
            var updated = await client.Items.UpsertAsync(item);
            var rowVersionAfterUpdate = Database.ReadItemRowVersion(pluNo);

            Assert.AreEqual(SadrWriteOperation.Inserted, inserted.Operation);
            Assert.AreEqual(SadrWriteOperation.Unchanged, unchanged.Operation);
            CollectionAssert.AreEqual(rowVersionAfterInsert, rowVersionAfterUnchanged);
            Assert.AreEqual(SadrWriteOperation.Updated, updated.Operation);
            Assert.IsFalse(rowVersionAfterUnchanged.SequenceEqual(rowVersionAfterUpdate));
        }

        [TestMethod]
        public async Task Sales_Read_Should_Handle_Identity_Gaps_And_Not_Mutate_Source_Rows()
        {
            Database.ExecuteNonQuery("TRUNCATE TABLE dbo.SADR_Logs;");
            Database.ExecuteNonQuery(@"
INSERT INTO dbo.SADR_Logs
(DeviceNo, Identify, [DateTime], FID, SID, Salesman, SubID, TotalPrice, PLU, Class, Dept,
 Amount, Unit, LogType, Tax, Text1, Text2, Text3, Text4, UnitPrice, CoFID, PLUName)
VALUES
(1, N'CI', GETDATE(), 101, 1, 1, 1, 1000, 10, 0, 0, 1.25, 3, 0, 0, NULL, NULL, NULL, NULL, 800, 0, N'A'),
(1, N'CI', GETDATE(), 102, 1, 1, 1, 2000, 11, 0, 0, 2.00, 3, 0, 0, NULL, NULL, NULL, NULL, 1000, 0, N'B'),
(1, N'CI', GETDATE(), 103, 1, 1, 1, 3000, 12, 0, 0, 3.00, 3, 0, 0, NULL, NULL, NULL, NULL, 1000, 0, N'C');
DELETE FROM dbo.SADR_Logs WHERE FID = 102;");

            var countBefore = Database.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.SADR_Logs;");
            var batch = await CreateClient().Sales.ReadAfterAsync(0, 10);
            var countAfter = Database.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.SADR_Logs;");

            Assert.AreEqual(2, batch.Rows.Count);
            Assert.AreEqual(101, batch.Rows[0].Fid);
            Assert.AreEqual(103, batch.Rows[1].Fid);
            Assert.IsTrue(batch.Rows[1].Id > batch.Rows[0].Id + 1, "The synthetic feed should contain an ID gap.");
            Assert.AreEqual(batch.Rows[1].Id, batch.LastReadId);
            Assert.AreEqual(countBefore, countAfter, "ReadAfterAsync must not mutate SADR_Logs.");
        }

        [TestMethod]
        public async Task Contract_Validation_Should_Use_Dedicated_Exception_For_Schema_Mismatch()
        {
            Database.ExecuteNonQuery("DROP INDEX UX_SADR_Item_PluNo ON dbo.SADR_Item;");

            try
            {
                await CreateClient().ValidateAsync();
                Assert.Fail("Expected SadrContractMismatchException.");
            }
            catch (SadrContractMismatchException exception)
            {
                Assert.AreEqual(51006, exception.SqlErrorNumber);
            }
            finally
            {
                Database.ExecuteNonQuery(
                    "CREATE UNIQUE NONCLUSTERED INDEX UX_SADR_Item_PluNo ON dbo.SADR_Item(PluNo);");
            }
        }
    }
}
