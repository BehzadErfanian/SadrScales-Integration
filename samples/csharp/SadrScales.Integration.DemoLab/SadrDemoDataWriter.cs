using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration.Items;

namespace SadrScales.Integration.DemoLab
{
    /// <summary>Result returned after a complete guarded DemoLab generation.</summary>
    public sealed class SadrDemoGenerationResult
    {
        public int Seed { get; internal set; }
        public int StoreCount { get; internal set; }
        public int GroupCount { get; internal set; }
        public int ItemCount { get; internal set; }
        public int ScaleCount { get; internal set; }
        public int InvoiceCount { get; internal set; }
        public int SalesRowCount { get; internal set; }
    }

    /// <summary>
    /// Writes one deterministic DemoLab scenario into a database that passed <see cref="SadrDemoDatabaseGuard"/>.
    /// </summary>
    /// <remarks>
    /// Production-contract entities are written through the real public SDK wherever Sadr Scales 5.2.1 supports it.
    /// Synthetic Scale registration and synthetic Sales/Invoice persistence are explicit Demo-only bootstrap operations
    /// because 5.2.1 intentionally does not expose production Scale lifecycle or fake-sale creation APIs.
    /// </remarks>
    public sealed class SadrDemoDataWriter
    {
        #region Dependencies

        private readonly string _connectionString;
        private readonly SadrDemoDatabaseGuard _guard;

        #endregion

        #region Construction

        public SadrDemoDataWriter(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is required.", nameof(connectionString));

            _connectionString = connectionString;
            _guard = new SadrDemoDatabaseGuard(connectionString);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Generates a complete deterministic DemoLab dataset in an empty, already-marked demo database.
        /// </summary>
        /// <remarks>
        /// Generation refuses to merge with existing business/demo rows. Use ResetDemoDataAsync first when regenerating.
        /// If any stage fails, a best-effort Demo reset removes partial data before the original exception is rethrown.
        /// </remarks>
        public async Task<SadrDemoGenerationResult> GenerateAsync(
            SadrDemoScenarioOptions? options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SadrDemoDatabaseSafety safety = await _guard.InspectAsync(cancellationToken).ConfigureAwait(false);
            if (!safety.CanWriteDemoData)
                throw new InvalidOperationException("Demo generation refused: " + safety.Message);
            if (!safety.IsBusinessDataEmpty)
                throw new InvalidOperationException(
                    "Demo generation requires an empty marked database. Run Demo reset before generating again.");

            SadrDemoScenario scenario = SadrDemoScenarioFactory.Create(options);

            try
            {
                var client = new SadrScalesClient(_connectionString);

                await WriteStoresAsync(client, scenario, cancellationToken).ConfigureAwait(false);
                await WriteGroupsAsync(client, scenario, cancellationToken).ConfigureAwait(false);
                await WriteItemsAsync(client, scenario, cancellationToken).ConfigureAwait(false);

                await InsertDemoScalesAsync(scenario, cancellationToken).ConfigureAwait(false);

                foreach (SadrDemoScale scale in scenario.Scales)
                {
                    await client.ScaleAssignments.ReplaceGroupsAsync(
                        scale.ScaleId,
                        scenario.ScaleAssignments[scale.ScaleId],
                        cancellationToken).ConfigureAwait(false);

                    await client.ScaleMappings.ReplaceAsync(
                        scale.ScaleId,
                        scenario.ScaleMappings[scale.ScaleId],
                        cancellationToken).ConfigureAwait(false);
                }

                foreach (KeyValuePair<string, List<HotKeys.SadrHotKey>> entry in scenario.GroupHotKeys)
                {
                    await client.HotKeys.ReplaceGroupAsync(
                        entry.Key,
                        entry.Value,
                        cancellationToken).ConfigureAwait(false);
                }

                await InsertSyntheticSalesAsync(scenario, cancellationToken).ConfigureAwait(false);
                await _guard.RecordGeneratedSeedAsync(scenario.Seed, cancellationToken).ConfigureAwait(false);

                return new SadrDemoGenerationResult
                {
                    Seed = scenario.Seed,
                    StoreCount = scenario.Stores.Count,
                    GroupCount = scenario.Groups.Count,
                    ItemCount = scenario.Items.Count,
                    ScaleCount = scenario.Scales.Count,
                    InvoiceCount = scenario.Invoices.Count,
                    SalesRowCount = scenario.Invoices.Sum(invoice => invoice.Lines.Count)
                };
            }
            catch
            {
                try
                {
                    await _guard.ResetDemoDataAsync(cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    // Preserve the original generation failure. The caller can inspect/reset the marked demo DB manually.
                }

                throw;
            }
        }

        #endregion

        #region Public SDK Stages

        private static async Task WriteStoresAsync(
            SadrScalesClient client,
            SadrDemoScenario scenario,
            CancellationToken cancellationToken)
        {
            foreach (var store in scenario.Stores)
                await client.Stores.UpsertAsync(store, cancellationToken).ConfigureAwait(false);
        }

        private static async Task WriteGroupsAsync(
            SadrScalesClient client,
            SadrDemoScenario scenario,
            CancellationToken cancellationToken)
        {
            foreach (SadrItemGroup group in scenario.Groups)
                await client.ItemGroups.UpsertAsync(group, cancellationToken).ConfigureAwait(false);
        }

        private static async Task WriteItemsAsync(
            SadrScalesClient client,
            SadrDemoScenario scenario,
            CancellationToken cancellationToken)
        {
            // Demo scenario validation caps this at the SDK's atomic 200-item batch limit.
            await client.Items.UpsertBatchAsync(scenario.Items, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Demo-only Scale Bootstrap

        private async Task InsertDemoScalesAsync(
            SadrDemoScenario scenario,
            CancellationToken cancellationToken)
        {
            const string sql = @"
INSERT INTO dbo.SADR_Scale
(
    ScaleID, Port, Mac, StoreCode, ItemClassCode, Category,
    Version, DeviceName, StoreName, ScaleIP, Status,
    LastSendItem, LastSendKey, LastReceiveFID,
    HotKeyCountPerPage, HotKeyPageCount,
    AutoSendItems, AutoGetInvoice, SendScaleDetail, GetScaleDetail, Used
)
VALUES
(
    @ScaleID, @Port, @Mac, @StoreCode, @ItemClassCode, @Category,
    @Version, @DeviceName, @StoreName, @ScaleIP, 'Offline',
    0, 0, 0,
    @HotKeyCountPerPage, @HotKeyPageCount,
    0, 0, 0, 0, 0
);";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        foreach (SadrDemoScale scale in scenario.Scales)
                        {
                            using (var command = new SqlCommand(sql, connection, transaction))
                            {
                                command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = scale.ScaleId;
                                command.Parameters.Add("@Port", SqlDbType.Int).Value = scale.Port;
                                command.Parameters.Add("@Mac", SqlDbType.VarChar, 50).Value = scale.Mac;
                                command.Parameters.Add("@StoreCode", SqlDbType.VarChar, 50).Value = scale.StoreCode;
                                command.Parameters.Add("@ItemClassCode", SqlDbType.VarChar, 50).Value = scale.PrimaryItemGroupCode;
                                command.Parameters.Add("@Category", SqlDbType.NVarChar, 50).Value = scale.Model;
                                command.Parameters.Add("@Version", SqlDbType.VarChar, 50).Value = "DEMO-5.2.1";
                                command.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 50).Value = scale.DeviceName;
                                command.Parameters.Add("@StoreName", SqlDbType.NVarChar, 50).Value = scale.StoreName;
                                command.Parameters.Add("@ScaleIP", SqlDbType.VarChar, 20).Value = scale.IpAddress;
                                command.Parameters.Add("@HotKeyCountPerPage", SqlDbType.SmallInt).Value = scale.HotKeyCountPerPage;
                                command.Parameters.Add("@HotKeyPageCount", SqlDbType.TinyInt).Value = scale.HotKeyPageCount;
                                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                            }
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

        #endregion

        #region Demo-only Sales and Invoice Bootstrap

        private async Task InsertSyntheticSalesAsync(
            SadrDemoScenario scenario,
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        foreach (SadrDemoInvoice invoice in scenario.Invoices)
                        {
                            int totalId = await InsertInvoiceHeaderAsync(
                                connection,
                                transaction,
                                invoice,
                                cancellationToken).ConfigureAwait(false);

                            foreach (SadrDemoInvoiceLine line in invoice.Lines)
                            {
                                await InsertInvoiceLineAsync(
                                    connection,
                                    transaction,
                                    totalId,
                                    invoice,
                                    line,
                                    cancellationToken).ConfigureAwait(false);
                                await InsertSalesLogAsync(
                                    connection,
                                    transaction,
                                    scenario,
                                    invoice,
                                    line,
                                    cancellationToken).ConfigureAwait(false);
                            }
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

        private static async Task<int> InsertInvoiceHeaderAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            SadrDemoInvoice invoice,
            CancellationToken cancellationToken)
        {
            const string sql = @"
INSERT INTO dbo.SADR_Total
(
    ScaleID, SaleDateTime, LableStatus, ReceiptNo, TotalBarcode,
    ItemBarcode, NTrans, SubDiscAmt, DiscAmt, AmtOfATax, AmtOfVTax,
    PriceWTax, ClerkNo
)
OUTPUT INSERTED.TotalID
VALUES
(
    @ScaleID, @SaleDateTime, @LableStatus, @ReceiptNo, @TotalBarcode,
    @ItemBarcode, @NTrans, 0, 0, 0, 0,
    @PriceWTax, 1
);";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@ScaleID", SqlDbType.Int).Value = invoice.ScaleId;
                command.Parameters.Add("@SaleDateTime", SqlDbType.SmallDateTime).Value = invoice.SaleDateTime;
                command.Parameters.Add("@LableStatus", SqlDbType.Int).Value = invoice.IsAcknowledged ? 1 : 0;
                command.Parameters.Add("@ReceiptNo", SqlDbType.Int).Value = invoice.Fid;
                command.Parameters.Add("@TotalBarcode", SqlDbType.VarChar, 50).Value = invoice.TotalBarcode;
                command.Parameters.Add("@ItemBarcode", SqlDbType.VarChar, 50).Value = invoice.ItemBarcode;
                command.Parameters.Add("@NTrans", SqlDbType.Int).Value = invoice.Lines.Count;
                command.Parameters.Add("@PriceWTax", SqlDbType.Int).Value = invoice.Lines.Sum(line => line.TotalPrice);
                return Convert.ToInt32(
                    await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
            }
        }

        private static async Task InsertInvoiceLineAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            int totalId,
            SadrDemoInvoice invoice,
            SadrDemoInvoiceLine line,
            CancellationToken cancellationToken)
        {
            const string sql = @"
INSERT INTO dbo.SADR_Detail
(
    TotalID, TotalBarcode, ItemBarcode, TransNo, PluNo,
    Weight, QTY, Uprice, UpriceAfDisc, StPointDiscStat,
    TTLPriceDiscAmt, ActPrice, TaxRtNo, ItemStatus
)
VALUES
(
    @TotalID, @TotalBarcode, @ItemBarcode, @TransNo, @PluNo,
    @Weight, @QTY, @Uprice, @Uprice, 0,
    0, @ActPrice, 0, 0
);";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@TotalID", SqlDbType.Int).Value = totalId;
                command.Parameters.Add("@TotalBarcode", SqlDbType.VarChar, 50).Value = invoice.TotalBarcode;
                command.Parameters.Add("@ItemBarcode", SqlDbType.VarChar, 50).Value =
                    invoice.ItemBarcode + "-" + line.SubId;
                command.Parameters.Add("@TransNo", SqlDbType.Int).Value = line.SubId;
                command.Parameters.Add("@PluNo", SqlDbType.Int).Value = line.PluNo;
                command.Parameters.Add("@Weight", SqlDbType.Float).Value = line.Unit == 2
                    ? 0d
                    : Convert.ToDouble(line.Amount);
                command.Parameters.Add("@QTY", SqlDbType.Float).Value = line.Unit == 2
                    ? Convert.ToDouble(line.Amount)
                    : 0d;
                command.Parameters.Add("@Uprice", SqlDbType.Int).Value = line.UnitPrice;
                command.Parameters.Add("@ActPrice", SqlDbType.Int).Value = line.TotalPrice;
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task InsertSalesLogAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            SadrDemoScenario scenario,
            SadrDemoInvoice invoice,
            SadrDemoInvoiceLine line,
            CancellationToken cancellationToken)
        {
            SadrDemoScale scale = scenario.Scales.First(candidate => candidate.ScaleId == invoice.ScaleId);
            const string sql = @"
INSERT INTO dbo.SADR_Logs
(
    DeviceNo, Identify, [DateTime], FID, SID, Salesman, SubID,
    TotalPrice, PLU, Class, Dept, Amount, Unit, LogType, Tax,
    Text1, Text2, Text3, Text4, UnitPrice, CoFID, PLUName
)
VALUES
(
    @DeviceNo, @Identify, @DateTime, @FID, 1, 1, @SubID,
    @TotalPrice, @PLU, 0, 0, @Amount, @Unit, 0, 0,
    NULL, NULL, NULL, NULL, @UnitPrice, 0, @PLUName
);";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@DeviceNo", SqlDbType.Int).Value = invoice.ScaleId;
                command.Parameters.Add("@Identify", SqlDbType.NVarChar, 50).Value = scale.IpAddress;
                command.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = invoice.SaleDateTime;
                command.Parameters.Add("@FID", SqlDbType.Int).Value = invoice.Fid;
                command.Parameters.Add("@SubID", SqlDbType.Int).Value = line.SubId;
                command.Parameters.Add("@TotalPrice", SqlDbType.Float).Value = line.TotalPrice;
                command.Parameters.Add("@PLU", SqlDbType.Int).Value = line.PluNo;
                command.Parameters.Add("@Amount", SqlDbType.Float).Value = Convert.ToDouble(line.Amount);
                command.Parameters.Add("@Unit", SqlDbType.Int).Value = line.Unit;
                command.Parameters.Add("@UnitPrice", SqlDbType.Float).Value = line.UnitPrice;
                command.Parameters.Add("@PLUName", SqlDbType.NVarChar, 50).Value =
                    Truncate(line.PluName, 50);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        #endregion

        #region Helpers

        private static string Truncate(string value, int maximumLength)
        {
            string safe = value ?? string.Empty;
            return safe.Length <= maximumLength ? safe : safe.Substring(0, maximumLength);
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
