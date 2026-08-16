using System;

namespace SadrScales.Integration
{
    /// <summary>
    /// Configuration for <see cref="SadrScalesClient"/>.
    /// </summary>
    public sealed class SadrScalesClientOptions
    {
        private int _commandTimeoutSeconds = 30;
        private int _transientRetryCount = 2;
        private int _transientRetryBaseDelayMilliseconds = 250;

        /// <summary>
        /// Creates options with the caller-owned SQL Server connection string.
        /// </summary>
        /// <param name="connectionString">Connection string for the Sadr Scales database.</param>
        public SadrScalesClientOptions(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("A non-empty SQL Server connection string is required.", nameof(connectionString));
            }

            ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets the caller-supplied SQL Server connection string.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Gets or sets the command timeout in seconds. Default is 30 seconds.
        /// </summary>
        public int CommandTimeoutSeconds
        {
            get => _commandTimeoutSeconds;
            set
            {
                if (value < 1 || value > 300)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Command timeout must be between 1 and 300 seconds.");
                }

                _commandTimeoutSeconds = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of retries after the initial attempt for transient connection/read failures.
        /// Default is 2. Transactional write commands are not automatically re-executed.
        /// </summary>
        public int TransientRetryCount
        {
            get => _transientRetryCount;
            set
            {
                if (value < 0 || value > 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Transient retry count must be between 0 and 5.");
                }

                _transientRetryCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the first retry delay in milliseconds. Later retries use bounded exponential backoff.
        /// Default is 250 ms.
        /// </summary>
        public int TransientRetryBaseDelayMilliseconds
        {
            get => _transientRetryBaseDelayMilliseconds;
            set
            {
                if (value < 1 || value > 5000)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Transient retry base delay must be between 1 and 5000 milliseconds.");
                }

                _transientRetryBaseDelayMilliseconds = value;
            }
        }
    }
}
