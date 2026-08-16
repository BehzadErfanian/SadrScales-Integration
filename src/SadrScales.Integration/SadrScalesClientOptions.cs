using System;

namespace SadrScales.Integration
{
    /// <summary>
    /// Configuration for <see cref="SadrScalesClient"/>.
    /// </summary>
    public sealed class SadrScalesClientOptions
    {
        private int _commandTimeoutSeconds = 30;

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
    }
}
