using System;
using System.Linq;
using System.Reflection;
using SadrScales.Integration;
using SadrScales.Integration.Assignments;
using SadrScales.Integration.HotKeys;
using SadrScales.Integration.Invoices;
using SadrScales.Integration.Items;
using SadrScales.Integration.Sales;
using SadrScales.Integration.Scales;
using SadrScales.Integration.Stores;

namespace SadrScales.Integration.Net48Consumer
{
    internal static class Program
    {
        #region Entry Point

        private static int Main()
        {
            try
            {
                var options = new SadrScalesClientOptions(
                    "Server=localhost;Database=CompatibilitySmoke;Integrated Security=true;Encrypt=Optional")
                {
                    CommandTimeoutSeconds = 15,
                    TransientRetryCount = 1,
                    TransientRetryBaseDelayMilliseconds = 100
                };

                Assert(options.CommandTimeoutSeconds == 15, "Command timeout option did not round-trip.");
                Assert(options.TransientRetryCount == 1, "Retry count option did not round-trip.");
                Assert(options.TransientRetryBaseDelayMilliseconds == 100, "Retry delay option did not round-trip.");

                var client = new SadrScalesClient(options);
                Assert(client.Stores != null, "Stores API was not created.");
                Assert(client.ItemGroups != null, "ItemGroups API was not created.");
                Assert(client.Items != null, "Items API was not created.");
                Assert(client.Sales != null, "Sales API was not created.");
                Assert(client.Invoices != null, "Invoices API was not created.");
                Assert(client.Scales != null, "Scales API was not created.");
                Assert(client.ScaleAssignments != null, "ScaleAssignments API was not created.");
                Assert(client.ScaleMappings != null, "ScaleMappings API was not created.");
                Assert(client.HotKeys != null, "HotKeys API was not created.");

                var store = new SadrStore
                {
                    StoreCode = "NET48",
                    StoreName = "Compatibility Store"
                };

                var group = new SadrItemGroup
                {
                    ItemClassCode = "NET48",
                    ItemClassName = "Compatibility Smoke"
                };

                var item = new SadrItem
                {
                    ItemClassCode = group.ItemClassCode,
                    PluNo = 480001,
                    PluName = "NET48 Smoke",
                    UnitPrice = 1000
                };

                var mapping = new SadrScaleItemMap
                {
                    ScaleId = 48,
                    PluNo = item.PluNo,
                    ItemCode = 1,
                    PageNo = 0,
                    KeyNo = 1
                };

                var hotKey = new SadrHotKey
                {
                    PageNo = 0,
                    KeyNo = 1,
                    PluNo = item.PluNo
                };

                Assert(store.StoreCode == "NET48", "Store public model failed.");
                Assert(group.ItemClassCode == "NET48", "Item-group public model failed.");
                Assert(item.PluNo == 480001, "Item public model failed.");
                Assert(mapping.ItemCode == 1, "Scale mapping public model failed.");
                Assert(hotKey.PluNo == item.PluNo, "HotKey public model failed.");
                Assert(typeof(SadrStoreUpsertResult).IsPublic, "Store upsert result public enum is unavailable.");
                Assert(typeof(SadrItemSoftDeleteResult).IsPublic, "Item soft-delete result public enum is unavailable.");
                Assert(typeof(SadrPriceHistoryEntry).IsPublic, "Price-history public model is unavailable.");
                Assert(typeof(SadrReplaceResult).IsPublic, "Replace result public enum is unavailable.");
                Assert(typeof(SadrScaleItemMap).IsPublic, "Scale mapping public model is unavailable.");
                Assert(typeof(SadrHotKey).IsPublic, "HotKey public model is unavailable.");
                Assert(typeof(SadrSalesBatch).IsPublic, "Sales batch public type is unavailable.");

                string barcode = SadrInvoiceClient.BuildTotalBarcode(12, 3456);
                Assert(barcode == "25012000003456", "Structured invoice TotalBarcode generation failed.");
                Assert(typeof(SadrInvoiceLookupResult).IsPublic, "Invoice lookup result public type is unavailable.");
                Assert(typeof(SadrInvoice).IsPublic, "Structured invoice public type is unavailable.");
                Assert(typeof(SadrInvoiceItem).IsPublic, "Structured invoice item public type is unavailable.");

                Assert(typeof(SadrScale).IsPublic, "Scale public model is unavailable.");
                Assert(typeof(SadrScaleStatus).IsPublic, "Scale status public enum is unavailable.");
                Assert(typeof(SadrResendRequestResult).IsPublic, "Resend request result public enum is unavailable.");

                // The SDK package depends on Microsoft.Data.SqlClient. Loading the referenced assembly proves
                // that the restored net48 application has a compatible provider asset/dependency graph.
                var sqlClientReference = typeof(SadrScalesClient)
                    .Assembly
                    .GetReferencedAssemblies()
                    .FirstOrDefault(reference =>
                        string.Equals(reference.Name, "Microsoft.Data.SqlClient", StringComparison.Ordinal));

                if (sqlClientReference == null)
                {
                    throw new InvalidOperationException("SDK assembly does not reference Microsoft.Data.SqlClient.");
                }

                var sqlClientAssembly = Assembly.Load(sqlClientReference);
                if (sqlClientAssembly == null)
                {
                    throw new InvalidOperationException("Microsoft.Data.SqlClient could not be loaded by the net48 consumer.");
                }

                Console.WriteLine("PASS - SadrScales.Integration package loaded and executed from .NET Framework 4.8.");
                Console.WriteLine("SDK assembly: " + typeof(SadrScalesClient).Assembly.FullName);
                Console.WriteLine("SqlClient assembly: " + sqlClientAssembly.FullName);
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("FAIL - .NET Framework 4.8 consumer compatibility smoke test.");
                Console.Error.WriteLine(exception);
                return 1;
            }
        }

        #endregion

        #region Test Helper

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        #endregion
    }
}
