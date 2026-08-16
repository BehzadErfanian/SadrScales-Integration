using System;
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
    }
}
