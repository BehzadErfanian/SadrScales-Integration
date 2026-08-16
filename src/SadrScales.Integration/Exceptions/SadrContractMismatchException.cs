using System;

namespace SadrScales.Integration.Exceptions
{
    /// <summary>
    /// Indicates that the connected database does not satisfy the frozen Sadr Scales SQL Contract v1 schema.
    /// </summary>
    public sealed class SadrContractMismatchException : Exception
    {
        internal SadrContractMismatchException(string message, int sqlErrorNumber, Exception innerException)
            : base(message, innerException)
        {
            SqlErrorNumber = sqlErrorNumber;
        }

        /// <summary>
        /// Gets the SQL error number emitted by the contract validator.
        /// </summary>
        public int SqlErrorNumber { get; }
    }
}
