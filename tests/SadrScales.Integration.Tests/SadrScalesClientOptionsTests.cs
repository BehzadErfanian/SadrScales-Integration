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
            var options = CreateOptions();
            Assert.AreEqual(30, options.CommandTimeoutSeconds);
        }

        [TestMethod]
        public void CommandTimeout_Should_Reject_Out_Of_Range_Values()
        {
            var options = CreateOptions();
            AssertTimeoutRejected(options, 0);
            AssertTimeoutRejected(options, 301);
        }

        [TestMethod]
        public void Retry_Should_Have_Bounded_Defaults()
        {
            var options = CreateOptions();
            Assert.AreEqual(2, options.TransientRetryCount);
            Assert.AreEqual(250, options.TransientRetryBaseDelayMilliseconds);
        }

        [TestMethod]
        public void Retry_Count_Should_Reject_Out_Of_Range_Values()
        {
            var options = CreateOptions();
            AssertRetryCountRejected(options, -1);
            AssertRetryCountRejected(options, 6);
        }

        [TestMethod]
        public void Retry_Delay_Should_Reject_Out_Of_Range_Values()
        {
            var options = CreateOptions();
            AssertRetryDelayRejected(options, 0);
            AssertRetryDelayRejected(options, 5001);
        }

        private static SadrScalesClientOptions CreateOptions()
        {
            return new SadrScalesClientOptions("Server=localhost;Database=Test;Integrated Security=true;Encrypt=Optional");
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

        private static void AssertRetryCountRejected(SadrScalesClientOptions options, int value)
        {
            try
            {
                options.TransientRetryCount = value;
                Assert.Fail("Expected ArgumentOutOfRangeException.");
            }
            catch (ArgumentOutOfRangeException)
            {
                // Expected.
            }
        }

        private static void AssertRetryDelayRejected(SadrScalesClientOptions options, int value)
        {
            try
            {
                options.TransientRetryBaseDelayMilliseconds = value;
                Assert.Fail("Expected ArgumentOutOfRangeException.");
            }
            catch (ArgumentOutOfRangeException)
            {
                // Expected.
            }
        }
    }
}
