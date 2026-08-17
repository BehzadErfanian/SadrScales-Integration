using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadrScales.Integration.Items;

namespace SadrScales.Integration.Tests
{
    [TestClass]
    public sealed class SadrItemValidationTests
    {
        private const string UnusedConnectionString = "Server=localhost;Database=Unused;Integrated Security=true;Encrypt=Optional";

        [TestMethod]
        public async Task Item_Upsert_Should_Reject_Zero_Plu_Before_Opening_SQL_Connection()
        {
            var client = new SadrScalesClient(UnusedConnectionString);
            var item = new SadrItem { PluNo = 0, ItemClassCode = "0" };

            try
            {
                await client.Items.UpsertAsync(item);
                Assert.Fail("Expected ArgumentOutOfRangeException.");
            }
            catch (ArgumentOutOfRangeException)
            {
                // Expected: input validation occurs before SQL access.
            }
        }

        [TestMethod]
        public async Task Item_Upsert_Should_Reject_Oversized_Name_Before_Opening_SQL_Connection()
        {
            var client = new SadrScalesClient(UnusedConnectionString);
            var item = new SadrItem
            {
                PluNo = 1,
                ItemClassCode = "0",
                PluName = new string('X', 101)
            };

            try
            {
                await client.Items.UpsertAsync(item);
                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException)
            {
                // Expected.
            }
        }

        [TestMethod]
        public async Task Group_Upsert_Should_Reject_Blank_Code_Before_Opening_SQL_Connection()
        {
            var client = new SadrScalesClient(UnusedConnectionString);
            var group = new SadrItemGroup { ItemClassCode = "" };

            try
            {
                await client.ItemGroups.UpsertAsync(group);
                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException)
            {
                // Expected.
            }
        }

        [TestMethod]
        public async Task Item_Batch_Should_Reject_Duplicate_Plu_Before_Opening_SQL_Connection()
        {
            var client = new SadrScalesClient(UnusedConnectionString);
            var items = new[]
            {
                new SadrItem { PluNo = 10, ItemClassCode = "0" },
                new SadrItem { PluNo = 10, ItemClassCode = "0" }
            };

            try
            {
                await client.Items.UpsertBatchAsync(items);
                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException)
            {
                // Expected before any SQL connection is opened.
            }
        }

        [TestMethod]
        public async Task Item_Batch_Should_Reject_More_Than_Maximum_Before_Opening_SQL_Connection()
        {
            var client = new SadrScalesClient(UnusedConnectionString);
            var items = new List<SadrItem>();
            for (var i = 1; i <= SadrItemClient.MaxBatchSize + 1; i++)
            {
                items.Add(new SadrItem { PluNo = i, ItemClassCode = "0" });
            }

            try
            {
                await client.Items.UpsertBatchAsync(items);
                Assert.Fail("Expected ArgumentOutOfRangeException.");
            }
            catch (ArgumentOutOfRangeException)
            {
                // Expected before any SQL connection is opened.
            }
        }

        [TestMethod]
        public async Task Empty_Item_Batch_Should_Return_Zero_Result_Without_SQL_Access()
        {
            var client = new SadrScalesClient(UnusedConnectionString);
            var result = await client.Items.UpsertBatchAsync(Array.Empty<SadrItem>());

            Assert.AreEqual(0, result.Total);
            Assert.AreEqual(0, result.Inserted);
            Assert.AreEqual(0, result.Updated);
            Assert.AreEqual(0, result.Unchanged);
            Assert.IsFalse(result.Changed);
        }
    }
}
