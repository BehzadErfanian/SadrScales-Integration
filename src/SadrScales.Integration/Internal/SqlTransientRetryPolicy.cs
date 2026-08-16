using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SadrScales.Integration.Internal
{
    internal sealed class SqlTransientRetryPolicy
    {
        internal const int MaximumDelayMilliseconds = 5000;

        private static readonly HashSet<int> TransientErrorNumbers = new HashSet<int>
        {
            -2,    // SQL command/login timeout.
            64,    // Transport-level connection failure.
            233,   // Connection/login pipe failure.
            1205,  // Deadlock victim.
            10053, // Connection aborted by local host software.
            10054, // Connection reset by peer.
            10060, // Network connection timeout.
            10928, // Resource limit reached.
            10929, // Resource limit reached.
            40197, // Service error processing request.
            40501, // Service busy.
            40613, // Database temporarily unavailable.
            49918, // Insufficient resources.
            49919, // Too many create/update operations.
            49920  // Too many operations.
        };

        private readonly SadrScalesClientOptions _options;

        public SqlTransientRetryPolicy(SadrScalesClientOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<T> ExecuteAsync<T>(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken)
        {
            return ExecuteAsync(
                operation,
                exception => exception is SqlException sqlException && IsTransient(sqlException),
                cancellationToken);
        }

        internal async Task<T> ExecuteAsync<T>(
            Func<CancellationToken, Task<T>> operation,
            Func<Exception, bool> transientClassifier,
            CancellationToken cancellationToken)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (transientClassifier == null)
            {
                throw new ArgumentNullException(nameof(transientClassifier));
            }

            var retryNumber = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    return await operation(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception) when (
                    retryNumber < _options.TransientRetryCount &&
                    transientClassifier(exception))
                {
                    retryNumber++;
                    var delay = CalculateDelayMilliseconds(
                        _options.TransientRetryBaseDelayMilliseconds,
                        retryNumber);

                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        internal static bool IsTransient(SqlException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            foreach (SqlError error in exception.Errors)
            {
                if (IsTransientErrorNumber(error.Number))
                {
                    return true;
                }
            }

            return IsTransientErrorNumber(exception.Number);
        }

        internal static bool IsTransientErrorNumber(int errorNumber)
        {
            return TransientErrorNumbers.Contains(errorNumber);
        }

        internal static int CalculateDelayMilliseconds(int baseDelayMilliseconds, int retryNumber)
        {
            if (baseDelayMilliseconds < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(baseDelayMilliseconds));
            }

            if (retryNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(retryNumber));
            }

            var multiplier = 1L << Math.Min(retryNumber - 1, 20);
            var calculated = baseDelayMilliseconds * multiplier;
            return (int)Math.Min(calculated, MaximumDelayMilliseconds);
        }
    }
}
