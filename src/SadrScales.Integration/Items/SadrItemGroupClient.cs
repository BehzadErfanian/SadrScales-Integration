using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration.Internal;

namespace SadrScales.Integration.Items
{
    /// <summary>
    /// Basic Contract v1 operations for item groups.
    /// </summary>
    public sealed class SadrItemGroupClient
    {
        private const string UpsertSql = @"
IF NOT EXISTS (
    SELECT 1
    FROM dbo.SADR_ItemClass WITH (UPDLOCK, HOLDLOCK)
    WHERE ItemClassCode = @Code)
BEGIN
    INSERT INTO dbo.SADR_ItemClass (ItemClassCode, ItemClassName, Descriptions)
    VALUES (@Code, @Name, @Descriptions);
    SELECT CAST(1 AS int);
END
ELSE
BEGIN
    UPDATE dbo.SADR_ItemClass
    SET ItemClassName = @Name,
        Descriptions = @Descriptions
    WHERE ItemClassCode = @Code
      AND EXISTS (
          SELECT ItemClassName, Descriptions
          EXCEPT
          SELECT @Name, @Descriptions);

    DECLARE @Rows int = @@ROWCOUNT;
    SELECT CASE WHEN @Rows = 0 THEN 0 ELSE 2 END;
END;";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly SadrScalesClientOptions _options;

        internal SadrItemGroupClient(SqlConnectionFactory connectionFactory, SadrScalesClientOptions options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
        }

        /// <summary>
        /// Inserts or semantically updates an item group.
        /// </summary>
        public async Task<SadrWriteResult> UpsertAsync(
            SadrItemGroup group,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Validate(group);

            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            using (var command = new SqlCommand(UpsertSql, connection, transaction))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                command.Parameters.Add("@Code", SqlDbType.VarChar, 50).Value = group.ItemClassCode;
                command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = (object?)group.ItemClassName ?? DBNull.Value;
                command.Parameters.Add("@Descriptions", SqlDbType.NVarChar, 150).Value = (object?)group.Descriptions ?? DBNull.Value;

                try
                {
                    var scalar = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                    return new SadrWriteResult((SadrWriteOperation)Convert.ToInt32(scalar));
                }
                catch
                {
                    TryRollback(transaction);
                    throw;
                }
            }
        }

        private static void Validate(SadrItemGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (string.IsNullOrWhiteSpace(group.ItemClassCode))
            {
                throw new ArgumentException("ItemClassCode is required.", nameof(group));
            }

            ValidateLength(group.ItemClassCode, 50, nameof(group.ItemClassCode));
            ValidateLength(group.ItemClassName, 100, nameof(group.ItemClassName));
            ValidateLength(group.Descriptions, 150, nameof(group.Descriptions));
        }

        private static void ValidateLength(string? value, int maximumLength, string name)
        {
            if (value != null && value.Length > maximumLength)
            {
                throw new ArgumentException(name + " exceeds the SQL Contract v1 maximum length of " + maximumLength + ".", name);
            }
        }

        private static void TryRollback(SqlTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // Preserve the original operation exception.
            }
        }
    }
}
