using System;
using System.Linq;
using System.Reflection;
using SadrScales.Integration;
using SadrScales.Integration.Items;
using SadrScales.Integration.Sales;

namespace SadrScales.Integration.Net48Consumer
{
    internal static class Program
    {
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
                Assert(client.ItemGroups != null, "ItemGroups API was not created.");
                Assert(client.Items != null, "Items API was not created.");
                Assert(client.Sales != null, "Sales API was not created.");

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

                Assert(group.ItemClassCode == "NET48", "Item-group public model failed.");
                Assert(item.PluNo == 480001, "Item public model failed.");
                Assert(typeof(SadrSalesBatch).IsPublic, "Sales batch public type is unavailable.");

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

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
