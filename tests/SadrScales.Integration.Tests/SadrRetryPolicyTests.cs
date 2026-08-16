using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadrScales.Integration.Internal;

namespace SadrScales.Integration.Tests
{
    [TestClass]
    public sealed class SadrRetryPolicyTests
    {
        [TestMethod]
        public async Task RetryPolicy_Should_Retry_Transient_Failure_Until_Success()
        {
            var options = CreateFastOptions(retryCount: 2);
            var policy = new SqlTransientRetryPolicy(options);
            var attempts = 0;

            var result = await policy.ExecuteAsync(
                token =>
                {
                    attempts++;
                    if (attempts < 3)
                    {
                        return Task.FromException<int>(new SyntheticTransientException());
                    }

                    return Task.FromResult(42);
                },
                exception => exception is SyntheticTransientException,
                CancellationToken.None);

            Assert.AreEqual(42, result);
            Assert.AreEqual(3, attempts);
        }

        [TestMethod]
        public async Task RetryPolicy_Should_Stop_After_Configured_Retries()
        {
            var options = CreateFastOptions(retryCount: 2);
            var policy = new SqlTransientRetryPolicy(options);
            var attempts = 0;

            try
            {
                await policy.ExecuteAsync<int>(
                    token =>
                    {
                        attempts++;
                        return Task.FromException<int>(new SyntheticTransientException());
                    },
                    exception => exception is SyntheticTransientException,
                    CancellationToken.None);

                Assert.Fail("Expected the final transient exception.");
            }
            catch (SyntheticTransientException)
            {
                Assert.AreEqual(3, attempts, "Initial attempt plus exactly two retries were expected.");
            }
        }

        [TestMethod]
        public async Task RetryPolicy_Should_Not_Retry_NonTransient_Failure()
        {
            var options = CreateFastOptions(retryCount: 5);
            var policy = new SqlTransientRetryPolicy(options);
            var attempts = 0;

            try
            {
                await policy.ExecuteAsync<int>(
                    token =>
                    {
                        attempts++;
                        return Task.FromException<int>(new InvalidOperationException("not transient"));
                    },
                    exception => exception is SyntheticTransientException,
                    CancellationToken.None);

                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (InvalidOperationException)
            {
                Assert.AreEqual(1, attempts);
            }
        }

        [TestMethod]
        public async Task RetryPolicy_Should_Honor_Cancellation_During_Delay()
        {
            var options = new SadrScalesClientOptions("Server=localhost;Database=Unused;Integrated Security=true;Encrypt=Optional")
            {
                TransientRetryCount = 5,
                TransientRetryBaseDelayMilliseconds = 5000
            };
            var policy = new SqlTransientRetryPolicy(options);
            using (var cancellation = new CancellationTokenSource())
            {
                cancellation.CancelAfter(25);

                try
                {
                    await policy.ExecuteAsync<int>(
                        token => Task.FromException<int>(new SyntheticTransientException()),
                        exception => exception is SyntheticTransientException,
                        cancellation.Token);

                    Assert.Fail("Expected cancellation.");
                }
                catch (OperationCanceledException)
                {
                    // Expected: the retry delay is cancellation-aware.
                }
            }
        }

        [TestMethod]
        public void Transient_Error_Number_List_Should_Be_Conservative_And_Explicit()
        {
            Assert.IsTrue(SqlTransientRetryPolicy.IsTransientErrorNumber(-2));
            Assert.IsTrue(SqlTransientRetryPolicy.IsTransientErrorNumber(64));
            Assert.IsTrue(SqlTransientRetryPolicy.IsTransientErrorNumber(1205));
            Assert.IsTrue(SqlTransientRetryPolicy.IsTransientErrorNumber(40501));
            Assert.IsTrue(SqlTransientRetryPolicy.IsTransientErrorNumber(40613));
            Assert.IsFalse(SqlTransientRetryPolicy.IsTransientErrorNumber(18456), "Login failure must not be blindly retried.");
            Assert.IsFalse(SqlTransientRetryPolicy.IsTransientErrorNumber(4060), "Invalid/unavailable database configuration is not blindly retried by this SDK policy.");
        }

        [TestMethod]
        public void Retry_Delay_Should_Use_Bounded_Exponential_Backoff()
        {
            Assert.AreEqual(250, SqlTransientRetryPolicy.CalculateDelayMilliseconds(250, 1));
            Assert.AreEqual(500, SqlTransientRetryPolicy.CalculateDelayMilliseconds(250, 2));
            Assert.AreEqual(1000, SqlTransientRetryPolicy.CalculateDelayMilliseconds(250, 3));
            Assert.AreEqual(5000, SqlTransientRetryPolicy.CalculateDelayMilliseconds(5000, 2));
        }

        private static SadrScalesClientOptions CreateFastOptions(int retryCount)
        {
            return new SadrScalesClientOptions("Server=localhost;Database=Unused;Integrated Security=true;Encrypt=Optional")
            {
                TransientRetryCount = retryCount,
                TransientRetryBaseDelayMilliseconds = 1
            };
        }

        private sealed class SyntheticTransientException : Exception
        {
        }
    }
}
