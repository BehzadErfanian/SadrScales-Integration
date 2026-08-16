using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SadrScales.Integration.Tests
{
    [TestClass]
    public sealed class SadrScalesClientOptionsTests
    {
        [TestMethod]
        public void Constructor_Should_Reject_Blank_Connection_String()
        {
            try
            {
                _ = new SadrScalesClientOptions("   ");
                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException)
            {
                // Expected.
            }
        }

        [TestMethod]
        public void CommandTimeout_Should_Default_To_Thirty_Seconds()
        {
            var options = new SadrScalesClientOptions("Server=localhost;Database=Test;Integrated Security=true;Encrypt=Optional");

            Assert.AreEqual(30, options.CommandTimeoutSeconds);
        }

        [TestMethod]
        public void CommandTimeout_Should_Reject_Out_Of_Range_Values()
        {
            var options = new SadrScalesClientOptions("Server=localhost;Database=Test;Integrated Security=true;Encrypt=Optional");

            AssertTimeoutRejected(options, 0);
            AssertTimeoutRejected(options, 301);
        }

        private static void AssertTimeoutRejected(SadrScalesClientOptions options, int value)
        {
            try
            {
                options.CommandTimeoutSeconds = value;
                Assert.Fail("Expected ArgumentOutOfRangeException.");
            }
            catch (ArgumentOutOfRangeException)
            {
                // Expected.
            }
        }
    }
}
