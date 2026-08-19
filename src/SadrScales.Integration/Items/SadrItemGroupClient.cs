using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration.Internal;

namespace SadrScales.Integration.Items
{
    /// <summary>
    /// Public item-group catalog operations.
    /// </summary>
    public sealed class SadrItemGroupClient
    {
        #region SQL

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

        #endregion

        #region Dependencies

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly SadrScalesClientOptions _options;

        #endregion

        #region Construction

        internal SadrItemGroupClient(SqlConnectionFactory connectionFactory, SadrScalesClientOptions options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
        }

        #endregion

        #region Read API

        /// <summary>
        /// Reads all item groups ordered by ItemClassCode.
        /// </summary>
        public Task<IReadOnlyList<SadrItemGroup>> GetAllAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _connectionFactory.ExecuteReadAsync<IReadOnlyList<SadrItemGroup>>(
                async (connection, token) =>
                {
                    var result = new List<SadrItemGroup>();
                    const string sql = @"
SELECT ItemClassCode, ItemClassName, Descriptions
FROM dbo.SADR_ItemClass
ORDER BY ItemClassCode ASC;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(token).ConfigureAwait(false))
                            {
                                result.Add(Map(reader));
                            }
                        }
                    }

                    return result;
                },
                cancellationToken);
        }

        /// <summary>
        /// Reads one item group by ItemClassCode, or returns null when it does not exist.
        /// </summary>
        public Task<SadrItemGroup?> GetAsync(
            string itemClassCode,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateCode(itemClassCode);

            return _connectionFactory.ExecuteReadAsync<SadrItemGroup?>(
                async (connection, token) =>
                {
                    const string sql = @"
SELECT ItemClassCode, ItemClassName, Descriptions
FROM dbo.SADR_ItemClass
WHERE ItemClassCode = @Code;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _options.CommandTimeoutSeconds;
                        command.Parameters.Add("@Code", SqlDbType.VarChar, 50).Value = itemClassCode;

                        using (var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false))
                        {
                            return await reader.ReadAsync(token).ConfigureAwait(false)
                                ? Map(reader)
                                : null;
                        }
                    }
                },
                cancellationToken);
        }

        #endregion

        #region Write API

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

        #endregion

        #region Mapping

        private static SadrItemGroup Map(SqlDataReader reader)
        {
            return new SadrItemGroup
            {
                ItemClassCode = reader.GetString(0),
                ItemClassName = reader.IsDBNull(1) ? null : reader.GetString(1),
                Descriptions = reader.IsDBNull(2) ? null : reader.GetString(2)
            };
        }

        #endregion

        #region Validation

        private static void Validate(SadrItemGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            ValidateCode(group.ItemClassCode);
            ValidateLength(group.ItemClassName, 100, nameof(group.ItemClassName));
            ValidateLength(group.Descriptions, 150, nameof(group.Descriptions));
        }

        private static void ValidateCode(string itemClassCode)
        {
            if (string.IsNullOrWhiteSpace(itemClassCode))
            {
                throw new ArgumentException("ItemClassCode is required.", nameof(itemClassCode));
            }

            ValidateLength(itemClassCode, 50, nameof(itemClassCode));
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

        #endregion
    }
}
