using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadrScales.Integration.Sales;

namespace SadrScales.Integration.SqlTests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class SalesQueryReportSqlIntegrationTests
    {
        #region Test Database

        private static SqlTestDatabase? _database;

        private static SqlTestDatabase Database =>
            _database ?? throw new InvalidOperationException("SQL test database is not initialized.");

        private static SadrScalesClient CreateClient() => new SadrScalesClient(Database.ConnectionString);

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _database = SqlTestDatabase.Create();
            SeedSales();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _database?.Dispose();
            _database = null;
        }

        #endregion

        #region Query Tests

        [TestMethod]
        public async Task Sales_Query_Should_Return_Newest_First_With_Whole_Filter_Summary()
        {
            int before = Database.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.SADR_Logs;");

            SadrSalesPage page = await CreateClient().Sales.QueryAsync(new SadrSalesQueryFilter
            {
                PageNumber = 0,
                PageSize = 1
            });

            int after = Database.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.SADR_Logs;");

            Assert.AreEqual(1, page.PageNumber);
            Assert.AreEqual(50, page.PageSize, "5.2.1 query semantics clamp small pages to 50 rows.");
            Assert.AreEqual(1, page.PageCount);
            Assert.AreEqual(4, page.Rows.Count);
            Assert.AreEqual(4L, page.Summary.RecordCount);
            Assert.AreEqual(3L, page.Summary.InvoiceCount,
                "Two rows from Scale 1 / FID 100 belong to one invoice.");
            Assert.AreEqual(10000m, page.Summary.TotalPrice);
            Assert.AreEqual(2.250m, page.Summary.TotalWeight);
            Assert.AreEqual(5.000m, page.Summary.TotalQuantity);
            Assert.AreEqual(201, page.Rows[0].Fid);
            Assert.AreEqual(200, page.Rows[1].Fid);
            Assert.AreEqual(before, after, "QueryAsync must never mutate the sales source.");
        }

        [TestMethod]
        public async Task Sales_Query_Should_Apply_Date_Scale_Plu_Identify_And_Fid_Filters()
        {
            var client = CreateClient();

            SadrSalesPage scale = await client.Sales.QueryAsync(new SadrSalesQueryFilter { ScaleId = 1 });
            Assert.AreEqual(3L, scale.Summary.RecordCount);
            Assert.AreEqual(2L, scale.Summary.InvoiceCount);
            Assert.AreEqual(7000m, scale.Summary.TotalPrice);
            Assert.AreEqual(1.500m, scale.Summary.TotalWeight);
            Assert.AreEqual(5.000m, scale.Summary.TotalQuantity);

            SadrSalesPage item = await client.Sales.QueryAsync(new SadrSalesQueryFilter { Plu = 10 });
            Assert.AreEqual(3L, item.Summary.RecordCount);
            Assert.AreEqual(8000m, item.Summary.TotalPrice);
            Assert.AreEqual(2.250m, item.Summary.TotalWeight);
            Assert.AreEqual(3.000m, item.Summary.TotalQuantity);

            SadrSalesPage date = await client.Sales.QueryAsync(new SadrSalesQueryFilter
            {
                StartDateInclusive = new DateTime(2026, 8, 19),
                EndDateExclusive = new DateTime(2026, 8, 20)
            });
            Assert.AreEqual(2L, date.Summary.RecordCount);
            Assert.AreEqual(7000m, date.Summary.TotalPrice);

            SadrSalesPage identify = await client.Sales.QueryAsync(new SadrSalesQueryFilter
            {
                Identify = " 10.0.0.2 "
            });
            Assert.AreEqual(1, identify.Rows.Count);
            Assert.AreEqual(2, identify.Rows[0].DeviceNo);

            SadrSalesPage fid = await client.Sales.QueryAsync(new SadrSalesQueryFilter { Fid = 100 });
            Assert.AreEqual(2, fid.Rows.Count);
            Assert.IsTrue(fid.Rows.All(row => row.Fid == 100));
        }

        #endregion

        #region Report Tests

        [TestMethod]
        public async Task Sales_Reports_Should_Match_5_2_1_Daily_Scale_And_Item_Aggregates()
        {
            var client = CreateClient();

            var daily = await client.Reports.GetDailyAsync();
            Assert.AreEqual(2, daily.Count);
            Assert.AreEqual(new DateTime(2026, 8, 19), daily[0].SaleDate);
            Assert.AreEqual(2L, daily[0].Summary.RecordCount);
            Assert.AreEqual(2L, daily[0].Summary.InvoiceCount);
            Assert.AreEqual(7000m, daily[0].Summary.TotalPrice);
            Assert.AreEqual(0.750m, daily[0].Summary.TotalWeight);
            Assert.AreEqual(3.000m, daily[0].Summary.TotalQuantity);
            Assert.AreEqual(3000m, daily[1].Summary.TotalPrice);
            Assert.AreEqual(1L, daily[1].Summary.InvoiceCount);

            var byScale = await client.Reports.GetByScaleAsync();
            Assert.AreEqual(2, byScale.Count);
            Assert.AreEqual(1, byScale[0].ScaleId);
            Assert.AreEqual("10.0.0.1", byScale[0].Identify);
            Assert.AreEqual(7000m, byScale[0].Summary.TotalPrice);
            Assert.AreEqual(2, byScale[1].ScaleId);
            Assert.AreEqual(3000m, byScale[1].Summary.TotalPrice);

            var byItem = await client.Reports.GetByItemAsync();
            Assert.AreEqual(2, byItem.Count);
            Assert.AreEqual(10, byItem[0].Plu);
            Assert.AreEqual("Apple", byItem[0].PluName);
            Assert.AreEqual(8000m, byItem[0].Summary.TotalPrice);
            Assert.AreEqual(11, byItem[1].Plu);
            Assert.AreEqual(2000m, byItem[1].Summary.TotalPrice);
        }

        [TestMethod]
        public async Task Sales_Reports_Should_Use_The_Same_Filter_Semantics_As_Query()
        {
            var filter = new SadrSalesQueryFilter
            {
                ScaleId = 1,
                StartDateInclusive = new DateTime(2026, 8, 19),
                EndDateExclusive = new DateTime(2026, 8, 20)
            };

            SadrSalesPage query = await CreateClient().Sales.QueryAsync(filter);
            var daily = await CreateClient().Reports.GetDailyAsync(filter);
            var byScale = await CreateClient().Reports.GetByScaleAsync(filter);
            var byItem = await CreateClient().Reports.GetByItemAsync(filter);

            Assert.AreEqual(1L, query.Summary.RecordCount);
            Assert.AreEqual(4000m, query.Summary.TotalPrice);
            Assert.AreEqual(1, daily.Count);
            Assert.AreEqual(query.Summary.TotalPrice, daily[0].Summary.TotalPrice);
            Assert.AreEqual(1, byScale.Count);
            Assert.AreEqual(query.Summary.TotalPrice, byScale[0].Summary.TotalPrice);
            Assert.AreEqual(1, byItem.Count);
            Assert.AreEqual(query.Summary.TotalPrice, byItem[0].Summary.TotalPrice);
        }

        #endregion

        #region Period Tests

        [TestMethod]
        public void Sales_Period_Should_Match_Saturday_Week_And_Persian_Month_Semantics()
        {
            DateTime reference = new DateTime(2026, 8, 19, 15, 30, 0);

            SadrSalesDateRange today = SadrSalesPeriod.GetRange(SadrSalesPeriodPreset.Today, reference);
            Assert.AreEqual(new DateTime(2026, 8, 19), today.StartDateInclusive);
            Assert.AreEqual(new DateTime(2026, 8, 20), today.EndDateExclusive);

            SadrSalesDateRange week = SadrSalesPeriod.GetRange(SadrSalesPeriodPreset.CurrentWeek, reference);
            Assert.AreEqual(DayOfWeek.Saturday, week.StartDateInclusive.DayOfWeek);
            Assert.AreEqual(7d, (week.EndDateExclusive - week.StartDateInclusive).TotalDays);
            Assert.IsTrue(reference >= week.StartDateInclusive && reference < week.EndDateExclusive);

            SadrSalesDateRange month = SadrSalesPeriod.GetRange(SadrSalesPeriodPreset.CurrentMonth, reference);
            var calendar = new PersianCalendar();
            Assert.AreEqual(1, calendar.GetDayOfMonth(month.StartDateInclusive));
            Assert.IsTrue(reference >= month.StartDateInclusive && reference < month.EndDateExclusive);
        }

        #endregion

        #region Test Data

        private static void SeedSales()
        {
            Database.ExecuteNonQuery("TRUNCATE TABLE dbo.SADR_Logs;");
            Database.ExecuteNonQuery(@"
INSERT INTO dbo.SADR_Logs
(DeviceNo, Identify, [DateTime], FID, SID, Salesman, SubID, TotalPrice, PLU, Class, Dept,
 Amount, Unit, LogType, Tax, Text1, Text2, Text3, Text4, UnitPrice, CoFID, PLUName)
VALUES
(1, N'10.0.0.1', '2026-08-18T10:00:00', 100, 1, 1, 1, 1000, 10, 0, 0, 1.50, 3, 0, 0, NULL, NULL, NULL, NULL, 1000, 0, N'Apple'),
(1, N'10.0.0.1', '2026-08-18T10:01:00', 100, 1, 1, 2, 2000, 11, 0, 0, 2.00, 2, 0, 0, NULL, NULL, NULL, NULL, 1000, 0, N'Cake'),
(2, N'10.0.0.2', '2026-08-19T09:00:00', 200, 1, 1, 1, 3000, 10, 0, 0, 0.75, 3, 0, 0, NULL, NULL, NULL, NULL, 4000, 0, N'Apple'),
(1, N'10.0.0.1', '2026-08-19T11:00:00', 201, 1, 1, 1, 4000, 10, 0, 0, 3.00, 2, 0, 0, NULL, NULL, NULL, NULL, 1333, 0, N'Apple');
");
        }

        #endregion
    }
}
