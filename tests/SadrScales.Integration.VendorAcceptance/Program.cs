using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration;
using SadrScales.Integration.Assignments;
using SadrScales.Integration.HotKeys;
using SadrScales.Integration.Invoices;
using SadrScales.Integration.Items;
using SadrScales.Integration.Sales;
using SadrScales.Integration.Stores;

namespace SadrScales.Integration.VendorAcceptance
{
    internal static class Program
    {
        #region Entry Point

        private static async Task<int> Main()
        {
            string? masterConnectionString = Environment.GetEnvironmentVariable("SADR_VENDOR_ACCEPTANCE_SQL");
            if (string.IsNullOrWhiteSpace(masterConnectionString))
            {
                Console.Error.WriteLine("SADR_VENDOR_ACCEPTANCE_SQL is required.");
                return 2;
            }

            string databaseName = "SadrScalesVendorAcceptance_" + Guid.NewGuid().ToString("N").Substring(0, 10);

            try
            {
                await CreateDatabaseAsync(masterConnectionString, databaseName).ConfigureAwait(false);
                string databaseConnectionString = BuildDatabaseConnectionString(masterConnectionString, databaseName);

                await ExecuteScriptAsync(databaseConnectionString, "VendorFixture.sql").ConfigureAwait(false);
                await ExecuteScriptAsync(databaseConnectionString, "VendorContractRequirements.sql").ConfigureAwait(false);

                await RunVendorFlowAsync(databaseConnectionString).ConfigureAwait(false);

                Console.WriteLine("PASS - package-only Vendor Acceptance flow completed successfully.");
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("FAIL - Vendor Acceptance flow.");
                Console.Error.WriteLine(exception);
                return 1;
            }
            finally
            {
                try
                {
                    SqlConnection.ClearAllPools();
                    await DropDatabaseAsync(masterConnectionString, databaseName).ConfigureAwait(false);
                }
                catch (Exception cleanupException)
                {
                    Console.Error.WriteLine("WARN - acceptance database cleanup failed: " + cleanupException.Message);
                }
            }
        }

        #endregion

        #region External Developer Flow

        private static async Task RunVendorFlowAsync(string connectionString)
        {
            var client = new SadrScalesClient(connectionString);

            // Start exactly where an external developer starts: validate the installed SQL Contract.
            await client.ValidateAsync().ConfigureAwait(false);

            await client.Stores.UpsertAsync(new SadrStore
            {
                StoreCode = "VA",
                StoreName = "Vendor Acceptance Store",
                Descriptions = "Synthetic acceptance data"
            }).ConfigureAwait(false);

            await client.ItemGroups.UpsertAsync(new SadrItemGroup
            {
                ItemClassCode = "VA",
                ItemClassName = "Vendor Acceptance Group",
                Descriptions = "Synthetic acceptance group"
            }).ConfigureAwait(false);

            await client.Items.UpsertAsync(new SadrItem
            {
                ItemClassCode = "VA",
                PluNo = 900001,
                PluName = "Vendor Acceptance Item",
                ItemCode = "900001",
                IndexBarcode = "900001",
                UnitPrice = 12500,
                PluUnit = 3
            }).ConfigureAwait(false);

            // Scale lifecycle is intentionally NOT a 5.2.1 production SQL API. The acceptance harness creates
            // one disabled synthetic registered scale only as fixture state, then all tested operations use the package.
            await InsertFixtureScaleAsync(connectionString).ConfigureAwait(false);

            var assignmentResult = await client.ScaleAssignments.ReplaceGroupsAsync(
                81,
                new[] { "VA" }).ConfigureAwait(false);
            Assert(assignmentResult.ToString() == "Replaced" || assignmentResult.ToString() == "Unchanged",
                "Scale assignment did not complete.");

            var mappingResult = await client.ScaleMappings.ReplaceAsync(
                81,
                new[]
                {
                    new SadrScaleItemMap
                    {
                        ScaleId = 81,
                        PluNo = 900001,
                        ItemCode = 1,
                        PageNo = 0,
                        KeyNo = 1
                    }
                }).ConfigureAwait(false);
            Assert(mappingResult.ToString() == "Replaced" || mappingResult.ToString() == "Unchanged",
                "Scale mapping did not complete.");

            var hotKeyResult = await client.HotKeys.ReplaceGroupAsync(
                "VA",
                new[]
                {
                    new SadrHotKey { PageNo = 0, KeyNo = 1, PluNo = 900001 }
                }).ConfigureAwait(false);
            Assert(hotKeyResult.ToString() == "Replaced" || hotKeyResult.ToString() == "Unchanged",
                "Group HotKey replacement did not complete.");

            var scale = await client.Scales.GetAsync(81).ConfigureAwait(false);
            Assert(scale != null, "Registered scale was not readable through the package.");
            Assert(scale!.Status.ToString() == "Offline", "Expected synthetic scale to be Offline.");

            var itemResend = await client.Scales.RequestItemResendAsync(81).ConfigureAwait(false);
            Assert(itemResend.ToString() == "Requested", "Item resend request was not recorded.");

            var keyResend = await client.Scales.RequestHotKeyResendAsync(81).ConfigureAwait(false);
            Assert(keyResend.ToString() == "Requested", "HotKey resend request was not recorded for LSG fixture scale.");

            string totalBarcode = SadrInvoiceClient.BuildTotalBarcode(81, 1001);
            await InsertFixtureSaleAsync(connectionString, totalBarcode).ConfigureAwait(false);

            SadrSalesBatch feed = await client.Sales.ReadAfterAsync(0, 100).ConfigureAwait(false);
            Assert(feed.Rows.Count == 1, "Incremental Sales Feed did not return the fixture sale.");

            SadrSalesPage query = await client.Sales.QueryAsync(new SadrSalesQueryFilter
            {
                ScaleId = 81,
                Plu = 900001,
                Fid = 1001,
                PageNumber = 1,
                PageSize = 100
            }).ConfigureAwait(false);
            Assert(query.Rows.Count == 1, "Sales Query did not return the fixture sale.");
            Assert(query.Summary.InvoiceCount == 1, "Sales Query invoice summary is incorrect.");

            var daily = await client.Reports.GetDailyAsync(new SadrSalesQueryFilter { ScaleId = 81 }).ConfigureAwait(false);
            var byScale = await client.Reports.GetByScaleAsync(new SadrSalesQueryFilter { ScaleId = 81 }).ConfigureAwait(false);
            var byItem = await client.Reports.GetByItemAsync(new SadrSalesQueryFilter { Plu = 900001 }).ConfigureAwait(false);
            Assert(daily.Count == 1 && byScale.Count == 1 && byItem.Count == 1,
                "Typed report surfaces did not return the fixture aggregates.");

            SadrInvoiceLookupResult lookup = await client.Invoices.GetByBarcodeAsync(totalBarcode).ConfigureAwait(false);
            Assert(lookup.Status == SadrInvoiceLookupStatus.FoundUnread,
                "Fresh structured invoice should be FoundUnread.");
            Assert(lookup.Invoice != null && lookup.Invoice.Items.Count == 1,
                "Structured invoice did not return complete detail data.");

            // Simulate the vendor's own successful destination transaction BEFORE source ACK.
            await SaveDestinationInvoiceAsync(connectionString, totalBarcode).ConfigureAwait(false);

            SadrInvoiceAckStatus ack = await client.Invoices.AcknowledgeAsync(totalBarcode).ConfigureAwait(false);
            Assert(ack == SadrInvoiceAckStatus.Acknowledged, "First invoice ACK did not acknowledge the invoice.");

            SadrInvoiceLookupResult reread = await client.Invoices.GetByBarcodeAsync(totalBarcode).ConfigureAwait(false);
            Assert(reread.Status == SadrInvoiceLookupStatus.AlreadyRead,
                "ACKed invoice did not report AlreadyRead.");
            Assert(reread.Invoice != null && reread.Invoice.Items.Count == 1,
                "AlreadyRead invoice was incorrectly blocked or lost its details.");

            SadrInvoiceAckStatus repeatedAck = await client.Invoices.AcknowledgeAsync(totalBarcode).ConfigureAwait(false);
            Assert(repeatedAck == SadrInvoiceAckStatus.AlreadyAcknowledged,
                "Repeated invoice ACK is not idempotent.");

            var deleteResult = await client.Items.SoftDeleteAsync(900001).ConfigureAwait(false);
            Assert(deleteResult.ToString() == "Deleted", "Logical PLU delete did not succeed.");
            SadrItem? deletedItem = await client.Items.GetAsync(900001).ConfigureAwait(false);
            Assert(deletedItem != null && deletedItem.DeleteFlag == 1,
                "Logically deleted PLU was not recoverable through individual lookup.");
        }

        #endregion

        #region Fixture Setup

        private static async Task InsertFixtureScaleAsync(string connectionString)
        {
            const string sql = @"
INSERT INTO dbo.SADR_Scale
(
    ScaleID, Port, Mac, StoreCode, ItemClassCode, GroupName, Category,
    Version, DeviceName, StoreName, ScaleIP, AutoSendItems, Status,
    LastSendItem, LastSendKey, LastReceiveFID, AutoGetInvoice,
    SendScaleDetail, GetScaleDetail, HotKeyCountPerPage, HotKeyPageCount, Used
)
VALUES
(
    81, 5000, 'VA-MAC-81', 'VA', 'VA', N'VA', N'LSG',
    'VA-5.2.1', N'VA Scale 81', N'Vendor Acceptance Store', '192.0.2.81',
    1, N'Offline', 123, 456, 0, 0, 0, 0, 40, 3, 0
);";

            await ExecuteNonQueryAsync(connectionString, sql).ConfigureAwait(false);
        }

        private static async Task InsertFixtureSaleAsync(string connectionString, string totalBarcode)
        {
            const string sql = @"
DECLARE @TotalID int;

INSERT INTO dbo.SADR_Total
(
    ScaleID, SaleDateTime, LableStatus, ReceiptNo, TotalBarcode,
    ItemBarcode, NTrans, SubDiscAmt, DiscAmt, AmtOfATax, AmtOfVTax, PriceWTax, ClerkNo
)
VALUES
(
    81, '2026-08-19T12:00:00', 0, 1001, @TotalBarcode,
    'VA-INVOICE-1001', 1, 0, 0, 0, 0, 25000, 1
);
SET @TotalID = SCOPE_IDENTITY();

INSERT INTO dbo.SADR_Detail
(
    TotalID, TotalBarcode, ItemBarcode, TransNo, PluNo,
    Weight, QTY, Uprice, UpriceAfDisc, StPointDiscStat,
    TTLPriceDiscAmt, ActPrice, TaxRtNo, ItemStatus
)
VALUES
(
    @TotalID, @TotalBarcode, 'VA-INVOICE-1001-1', 1, 900001,
    2.0, 0, 12500, 12500, 0, 0, 25000, 0, 0
);

INSERT INTO dbo.SADR_Logs
(
    DeviceNo, Identify, [DateTime], FID, SID, Salesman, SubID,
    TotalPrice, PLU, Class, Dept, Amount, Unit, LogType, Tax,
    Text1, Text2, Text3, Text4, UnitPrice, CoFID, PLUName
)
VALUES
(
    81, N'192.0.2.81', '2026-08-19T12:00:00', 1001, 1, 1, 1,
    25000, 900001, 0, 0, 2.0, 3, 0, 0,
    NULL, NULL, NULL, NULL, 12500, 0, N'Vendor Acceptance Item'
);";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@TotalBarcode", SqlDbType.VarChar, 50).Value = totalBarcode;
                await connection.OpenAsync().ConfigureAwait(false);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        private static async Task SaveDestinationInvoiceAsync(string connectionString, string totalBarcode)
        {
            const string sql = @"
BEGIN TRANSACTION;
INSERT INTO dbo.VendorDestinationInvoice(TotalBarcode, SavedAtUtc)
VALUES(@TotalBarcode, SYSUTCDATETIME());
COMMIT TRANSACTION;";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@TotalBarcode", SqlDbType.VarChar, 50).Value = totalBarcode;
                await connection.OpenAsync().ConfigureAwait(false);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region Database Lifecycle

        private static async Task CreateDatabaseAsync(string masterConnectionString, string databaseName)
        {
            string sql = "CREATE DATABASE [" + databaseName.Replace("]", "]]" ) + "];";
            await ExecuteNonQueryAsync(masterConnectionString, sql).ConfigureAwait(false);
        }

        private static async Task DropDatabaseAsync(string masterConnectionString, string databaseName)
        {
            string safeName = "[" + databaseName.Replace("]", "]]" ) + "]";
            string sql = "IF DB_ID(@DatabaseName) IS NOT NULL BEGIN ALTER DATABASE " + safeName +
                         " SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE " + safeName + "; END;";

            using (var connection = new SqlConnection(masterConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@DatabaseName", SqlDbType.NVarChar, 128).Value = databaseName;
                await connection.OpenAsync().ConfigureAwait(false);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        private static string BuildDatabaseConnectionString(string masterConnectionString, string databaseName)
        {
            var builder = new SqlConnectionStringBuilder(masterConnectionString)
            {
                InitialCatalog = databaseName
            };
            return builder.ConnectionString;
        }

        private static async Task ExecuteScriptAsync(string connectionString, string fileName)
        {
            string path = Path.Combine(AppContext.BaseDirectory, fileName);
            string sql = await File.ReadAllTextAsync(path).ConfigureAwait(false);
            await ExecuteNonQueryAsync(connectionString, sql).ConfigureAwait(false);
        }

        private static async Task ExecuteNonQueryAsync(string connectionString, string sql)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandTimeout = 120;
                await connection.OpenAsync().ConfigureAwait(false);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region Assertion Helper

        private static void Assert(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }

        #endregion
    }
}
