using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SadrScales.Integration.DemoLab
{
    /// <summary>Safety result returned before any DemoLab database write is allowed.</summary>
    public sealed class SadrDemoDatabaseSafety
    {
        public string DatabaseName { get; internal set; } = string.Empty;
        public bool HasRequiredSchema { get; internal set; }
        public bool HasDemoMarker { get; internal set; }
        public bool IsBusinessDataEmpty { get; internal set; }
        public bool HasSafeDemoName { get; internal set; }
        public bool CanInitializeMarker { get; internal set; }
        public bool CanWriteDemoData { get; internal set; }
        public string Message { get; internal set; } = string.Empty;
    }

    /// <summary>
    /// Protects Demo Data operations from customer/production databases.
    /// </summary>
    /// <remarks>
    /// A database can be marked for DemoLab use only when it has a clearly non-production name, contains the
    /// required migrated 5.2.1 schema and contains no business data beyond the default Store/Group rows.
    /// Every later generate/reset operation requires the private DemoLab marker token.
    /// </remarks>
    public sealed class SadrDemoDatabaseGuard
    {
        #region Constants

        private const string MarkerTable = "SADR_IntegrationDemoMarker";
        private const string MarkerToken = "SADR-INTEGRATION-DEMO-V1";

        private static readonly string[] RequiredTables =
        {
            "SADR_Store",
            "SADR_ItemClass",
            "SADR_Item",
            "SADR_Scale",
            "SADR_ScaleItemClass",
            "SADR_ScaleItemMap",
            "SADR_KeyAssignment",
            "SADR_Logs",
            "SADR_Total",
            "SADR_Detail",
            "SADR_PriceLog"
        };

        private readonly string _connectionString;

        #endregion

        #region Construction

        public SadrDemoDatabaseGuard(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is required.", nameof(connectionString));

            _connectionString = connectionString;
        }

        #endregion

        #region Public API

        /// <summary>Inspects the target database without modifying it.</summary>
        public async Task<SadrDemoDatabaseSafety> InspectAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                string databaseName = Convert.ToString(
                    await ExecuteScalarAsync(connection, null, "SELECT DB_NAME();", cancellationToken)
                        .ConfigureAwait(false)) ?? string.Empty;

                bool safeName = IsSafeDemoDatabaseName(databaseName);
                bool schema = await HasRequiredSchemaAsync(connection, cancellationToken).ConfigureAwait(false);
                bool marker = schema && await HasValidMarkerAsync(connection, cancellationToken).ConfigureAwait(false);
                bool empty = schema && await IsBusinessDataEmptyAsync(connection, cancellationToken).ConfigureAwait(false);

                var result = new SadrDemoDatabaseSafety
                {
                    DatabaseName = databaseName,
                    HasSafeDemoName = safeName,
                    HasRequiredSchema = schema,
                    HasDemoMarker = marker,
                    IsBusinessDataEmpty = empty,
                    CanInitializeMarker = safeName && schema && empty && !marker,
                    CanWriteDemoData = safeName && schema && marker
                };

                result.Message = BuildMessage(result);
                return result;
            }
        }

        /// <summary>
        /// Marks an empty, clearly named Demo/Test database for DemoLab use.
        /// </summary>
        /// <remarks>
        /// The exact current database name must be supplied again by the caller. This deliberate second factor
        /// prevents a stale connection string or accidental button press from marking a different database.
        /// </remarks>
        public async Task InitializeMarkerAsync(
            string confirmedDatabaseName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SadrDemoDatabaseSafety safety = await InspectAsync(cancellationToken).ConfigureAwait(false);

            if (!safety.CanInitializeMarker)
                throw new InvalidOperationException("Demo marker cannot be initialized: " + safety.Message);

            if (!string.Equals(
                    safety.DatabaseName,
                    (confirmedDatabaseName ?? string.Empty).Trim(),
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Typed database confirmation does not exactly match DB_NAME().");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                const string sql = @"
IF OBJECT_ID(N'dbo.SADR_IntegrationDemoMarker', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SADR_IntegrationDemoMarker
    (
        MarkerId int NOT NULL,
        MarkerToken varchar(64) NOT NULL,
        CreatedAtUtc datetime2(0) NOT NULL,
        LastSeed int NULL,
        LastGeneratedAtUtc datetime2(0) NULL,
        CONSTRAINT PK_SADR_IntegrationDemoMarker PRIMARY KEY CLUSTERED (MarkerId),
        CONSTRAINT CK_SADR_IntegrationDemoMarker_OneRow CHECK (MarkerId = 1)
    );
END;

DELETE FROM dbo.SADR_IntegrationDemoMarker;
INSERT INTO dbo.SADR_IntegrationDemoMarker
    (MarkerId, MarkerToken, CreatedAtUtc, LastSeed, LastGeneratedAtUtc)
VALUES
    (1, @MarkerToken, SYSUTCDATETIME(), NULL, NULL);";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@MarkerToken", SqlDbType.VarChar, 64).Value = MarkerToken;
                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Deletes business rows only from a valid marked DemoLab database while preserving schema/default rows/marker.
        /// </summary>
        public async Task ResetDemoDataAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SadrDemoDatabaseSafety safety = await InspectAsync(cancellationToken).ConfigureAwait(false);
            if (!safety.CanWriteDemoData)
                throw new InvalidOperationException("Demo reset refused: " + safety.Message);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        const string sql = @"
DELETE FROM dbo.SADR_Detail;
DELETE FROM dbo.SADR_Logs;
DELETE FROM dbo.SADR_Total;
DELETE FROM dbo.SADR_PriceLog;
DELETE FROM dbo.SADR_ScaleItemMap;
DELETE FROM dbo.SADR_KeyAssignment;
DELETE FROM dbo.SADR_ScaleItemClass;
IF OBJECT_ID(N'dbo.SADR_ItemSyncState', N'U') IS NOT NULL
    DELETE FROM dbo.SADR_ItemSyncState;
DELETE FROM dbo.SADR_Scale;
DELETE FROM dbo.SADR_Item;
DELETE FROM dbo.SADR_ItemClass WHERE ItemClassCode <> '0';
DELETE FROM dbo.SADR_Store WHERE StoreCode <> '0';
UPDATE dbo.SADR_IntegrationDemoMarker
SET LastSeed = NULL,
    LastGeneratedAtUtc = NULL
WHERE MarkerId = 1 AND MarkerToken = @MarkerToken;";

                        using (var command = new SqlCommand(sql, connection, transaction))
                        {
                            command.Parameters.Add("@MarkerToken", SqlDbType.VarChar, 64).Value = MarkerToken;
                            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        TryRollback(transaction);
                        throw;
                    }
                }
            }
        }

        internal async Task EnsureDemoWriteAllowedAsync(CancellationToken cancellationToken)
        {
            SadrDemoDatabaseSafety safety = await InspectAsync(cancellationToken).ConfigureAwait(false);
            if (!safety.CanWriteDemoData)
                throw new InvalidOperationException("Demo write refused: " + safety.Message);
        }

        internal async Task RecordGeneratedSeedAsync(
            int seed,
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                const string sql = @"
UPDATE dbo.SADR_IntegrationDemoMarker
SET LastSeed = @Seed,
    LastGeneratedAtUtc = SYSUTCDATETIME()
WHERE MarkerId = 1 AND MarkerToken = @MarkerToken;";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Seed", SqlDbType.Int).Value = seed;
                    command.Parameters.Add("@MarkerToken", SqlDbType.VarChar, 64).Value = MarkerToken;
                    int affected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    if (affected != 1)
                        throw new InvalidOperationException("Demo marker disappeared during generation.");
                }
            }
        }

        #endregion

        #region Inspection

        private static bool IsSafeDemoDatabaseName(string databaseName)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
                return false;

            string normalized = databaseName.ToLowerInvariant();
            if (normalized == "master" || normalized == "model" || normalized == "msdb" || normalized == "tempdb")
                return false;

            return normalized.Contains("demo") ||
                   normalized.Contains("test") ||
                   normalized.Contains("sample") ||
                   normalized.Contains("sandbox") ||
                   normalized.Contains("dev") ||
                   normalized.Contains("ci");
        }

        private static async Task<bool> HasRequiredSchemaAsync(
            SqlConnection connection,
            CancellationToken cancellationToken)
        {
            foreach (string table in RequiredTables)
            {
                using (var command = new SqlCommand(
                    "SELECT CASE WHEN OBJECT_ID(@ObjectName, N'U') IS NULL THEN 0 ELSE 1 END;",
                    connection))
                {
                    command.Parameters.Add("@ObjectName", SqlDbType.NVarChar, 256).Value = "dbo." + table;
                    int exists = Convert.ToInt32(
                        await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
                    if (exists != 1)
                        return false;
                }
            }

            return true;
        }

        private static async Task<bool> HasValidMarkerAsync(
            SqlConnection connection,
            CancellationToken cancellationToken)
        {
            using (var exists = new SqlCommand(
                "SELECT CASE WHEN OBJECT_ID(N'dbo.SADR_IntegrationDemoMarker', N'U') IS NULL THEN 0 ELSE 1 END;",
                connection))
            {
                if (Convert.ToInt32(await exists.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false)) != 1)
                    return false;
            }

            using (var command = new SqlCommand(@"
SELECT COUNT(*)
FROM dbo.SADR_IntegrationDemoMarker
WHERE MarkerId = 1 AND MarkerToken = @MarkerToken;", connection))
            {
                command.Parameters.Add("@MarkerToken", SqlDbType.VarChar, 64).Value = MarkerToken;
                return Convert.ToInt32(
                    await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false)) == 1;
            }
        }

        private static async Task<bool> IsBusinessDataEmptyAsync(
            SqlConnection connection,
            CancellationToken cancellationToken)
        {
            var checks = new Dictionary<string, string>
            {
                { "Scale", "SELECT COUNT_BIG(*) FROM dbo.SADR_Scale;" },
                { "Item", "SELECT COUNT_BIG(*) FROM dbo.SADR_Item;" },
                { "Logs", "SELECT COUNT_BIG(*) FROM dbo.SADR_Logs;" },
                { "Total", "SELECT COUNT_BIG(*) FROM dbo.SADR_Total;" },
                { "Detail", "SELECT COUNT_BIG(*) FROM dbo.SADR_Detail;" },
                { "PriceLog", "SELECT COUNT_BIG(*) FROM dbo.SADR_PriceLog;" },
                { "KeyAssignment", "SELECT COUNT_BIG(*) FROM dbo.SADR_KeyAssignment;" },
                { "ScaleItemClass", "SELECT COUNT_BIG(*) FROM dbo.SADR_ScaleItemClass;" },
                { "ScaleItemMap", "SELECT COUNT_BIG(*) FROM dbo.SADR_ScaleItemMap;" },
                { "Groups", "SELECT COUNT_BIG(*) FROM dbo.SADR_ItemClass WHERE ItemClassCode <> '0';" },
                { "Stores", "SELECT COUNT_BIG(*) FROM dbo.SADR_Store WHERE StoreCode <> '0';" }
            };

            foreach (KeyValuePair<string, string> check in checks)
            {
                object value = await ExecuteScalarAsync(
                    connection,
                    null,
                    check.Value,
                    cancellationToken).ConfigureAwait(false);
                if (Convert.ToInt64(value) != 0L)
                    return false;
            }

            using (var syncExists = new SqlCommand(
                "SELECT CASE WHEN OBJECT_ID(N'dbo.SADR_ItemSyncState', N'U') IS NULL THEN 0 ELSE 1 END;",
                connection))
            {
                if (Convert.ToInt32(await syncExists.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false)) == 1)
                {
                    object count = await ExecuteScalarAsync(
                        connection,
                        null,
                        "SELECT COUNT_BIG(*) FROM dbo.SADR_ItemSyncState;",
                        cancellationToken).ConfigureAwait(false);
                    if (Convert.ToInt64(count) != 0L)
                        return false;
                }
            }

            return true;
        }

        private static string BuildMessage(SadrDemoDatabaseSafety safety)
        {
            if (!safety.HasSafeDemoName)
                return "Database name is not clearly Demo/Test/Sample/Sandbox/Dev/CI.";
            if (!safety.HasRequiredSchema)
                return "Required Sadr Scales 5.2.1 tables are missing. Run normal schema creation/migration first.";
            if (safety.HasDemoMarker)
                return "Valid DemoLab marker found. Demo generation/reset is allowed.";
            if (!safety.IsBusinessDataEmpty)
                return "Database contains business data and cannot be marked for DemoLab use.";
            return "Empty non-production database is eligible for DemoLab initialization.";
        }

        #endregion

        #region SQL Helpers

        private static async Task<object> ExecuteScalarAsync(
            SqlConnection connection,
            SqlTransaction? transaction,
            string sql,
            CancellationToken cancellationToken)
        {
            using (var command = new SqlCommand(sql, connection, transaction))
            {
                return await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false)
                    ?? DBNull.Value;
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
                // Preserve the original failure.
            }
        }

        #endregion
    }
}
