using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadrScales.Integration.DemoLab;

namespace SadrScales.Integration.Tests
{
    [TestClass]
    public sealed class DemoScenarioTests
    {
        #region Determinism

        [TestMethod]
        public void Same_Seed_Should_Create_Exactly_The_Same_Demo_Scenario()
        {
            var options = new SadrDemoScenarioOptions { Seed = 12345 };

            SadrDemoScenario first = SadrDemoScenarioFactory.Create(options);
            SadrDemoScenario second = SadrDemoScenarioFactory.Create(options);

            Assert.AreEqual(Fingerprint(first), Fingerprint(second));
            Assert.AreEqual(new DateTime(2026, 1, 1).AddDays(12345 % 300), first.ReferenceDate);
        }

        [TestMethod]
        public void Different_Seed_Should_Change_The_Demo_Scenario()
        {
            string first = Fingerprint(SadrDemoScenarioFactory.Create(
                new SadrDemoScenarioOptions { Seed = 12345 }));
            string second = Fingerprint(SadrDemoScenarioFactory.Create(
                new SadrDemoScenarioOptions { Seed = 12346 }));

            Assert.AreNotEqual(first, second);
        }

        #endregion

        #region Safe Synthetic Shape

        [TestMethod]
        public void Default_Scenario_Should_Stay_Inside_Demo_Ranges_And_Be_Internally_Consistent()
        {
            SadrDemoScenario scenario = SadrDemoScenarioFactory.Create(
                new SadrDemoScenarioOptions { Seed = 12345 });

            Assert.AreEqual(3, scenario.Stores.Count);
            Assert.AreEqual(5, scenario.Groups.Count);
            Assert.AreEqual(100, scenario.Items.Count);
            Assert.AreEqual(10, scenario.Scales.Count);
            Assert.AreEqual(20, scenario.Invoices.Count);

            CollectionAssert.AreEqual(
                Enumerable.Range(81, 10).ToArray(),
                scenario.Scales.Select(scale => scale.ScaleId).ToArray());

            Assert.IsTrue(scenario.Scales.All(scale => scale.IpAddress.StartsWith("192.0.2.", StringComparison.Ordinal)));
            Assert.IsTrue(scenario.Items.All(item => item.PluNo >= 900001 && item.PluNo <= 900100));
            Assert.IsTrue(scenario.ScaleAssignments.Values.All(groups => groups.Count >= 1));
            Assert.IsTrue(scenario.ScaleMappings.Values.All(mappings => mappings.Count > 0));
            Assert.IsTrue(scenario.GroupHotKeys.Values.All(keys => keys.All(key => key.PluNo > 0)));

            foreach (SadrDemoInvoice invoice in scenario.Invoices)
            {
                Assert.AreEqual(14, invoice.TotalBarcode.Length);
                Assert.IsTrue(invoice.TotalBarcode.StartsWith("25", StringComparison.Ordinal));
                Assert.IsTrue(invoice.Lines.Count >= 1);
                Assert.IsTrue(invoice.Lines.All(line => line.TotalPrice > 0));
            }
        }

        #endregion

        #region Fingerprint Helper

        private static string Fingerprint(SadrDemoScenario scenario)
        {
            var builder = new StringBuilder();
            builder.Append(scenario.Seed).Append('|')
                .Append(scenario.ReferenceDate.ToString("O", CultureInfo.InvariantCulture)).Append('|');

            foreach (var store in scenario.Stores)
                builder.Append(store.StoreCode).Append(':').Append(store.StoreName).Append('|');
            foreach (var group in scenario.Groups)
                builder.Append(group.ItemClassCode).Append(':').Append(group.ItemClassName).Append('|');
            foreach (var item in scenario.Items)
                builder.Append(item.PluNo).Append(':').Append(item.ItemClassCode).Append(':')
                    .Append(item.PluUnit).Append(':').Append(item.UnitPrice).Append(':')
                    .Append(item.PluName).Append('|');
            foreach (var scale in scenario.Scales)
                builder.Append(scale.ScaleId).Append(':').Append(scale.Model).Append(':')
                    .Append(scale.IpAddress).Append(':').Append(scale.StoreCode).Append(':')
                    .Append(scale.PrimaryItemGroupCode).Append('|');
            foreach (var assignment in scenario.ScaleAssignments.OrderBy(pair => pair.Key))
                builder.Append('A').Append(assignment.Key).Append(':')
                    .Append(string.Join(",", assignment.Value)).Append('|');
            foreach (var mapping in scenario.ScaleMappings.OrderBy(pair => pair.Key))
            {
                foreach (var row in mapping.Value)
                    builder.Append('M').Append(mapping.Key).Append(':').Append(row.PluNo).Append(':')
                        .Append(row.ItemCode).Append(':').Append(row.PageNo).Append(':').Append(row.KeyNo).Append('|');
            }
            foreach (var group in scenario.GroupHotKeys.OrderBy(pair => pair.Key, StringComparer.Ordinal))
            {
                foreach (var row in group.Value)
                    builder.Append('K').Append(group.Key).Append(':').Append(row.PageNo).Append(':')
                        .Append(row.KeyNo).Append(':').Append(row.PluNo).Append('|');
            }
            foreach (SadrDemoInvoice invoice in scenario.Invoices)
            {
                builder.Append('I').Append(invoice.ScaleId).Append(':').Append(invoice.Fid).Append(':')
                    .Append(invoice.SaleDateTime.ToString("O", CultureInfo.InvariantCulture)).Append(':')
                    .Append(invoice.TotalBarcode).Append(':').Append(invoice.IsAcknowledged).Append('|');
                foreach (SadrDemoInvoiceLine line in invoice.Lines)
                    builder.Append(line.SubId).Append(':').Append(line.PluNo).Append(':')
                        .Append(line.Unit).Append(':').Append(line.Amount.ToString(CultureInfo.InvariantCulture)).Append(':')
                        .Append(line.UnitPrice).Append(':').Append(line.TotalPrice).Append('|');
            }

            return builder.ToString();
        }

        #endregion
    }
}
