using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SadrScales.Integration.Tests
{
    [TestClass]
    public sealed class SadrSalesValidationTests
    {
        private const string UnusedConnectionString = "Server=localhost;Database=Unused;Integrated Security=true;Encrypt=Optional";

        [TestMethod]
        public async Task Sales_Read_Should_Reject_Negative_Cursor_Before_SQL_Access()
        {
            var client = new SadrScalesClient(UnusedConnectionString);

            try
            {
                await client.Sales.ReadAfterAsync(-1, 100);
                Assert.Fail("Expected ArgumentOutOfRangeException.");
            }
            catch (ArgumentOutOfRangeException)
            {
                // Expected.
            }
        }

        [TestMethod]
        public async Task Sales_Read_Should_Reject_Invalid_Batch_Size_Before_SQL_Access()
        {
            var client = new SadrScalesClient(UnusedConnectionString);

            try
            {
                await client.Sales.ReadAfterAsync(0, 5001);
                Assert.Fail("Expected ArgumentOutOfRangeException.");
            }
            catch (ArgumentOutOfRangeException)
            {
                // Expected.
            }
        }
    }
}
