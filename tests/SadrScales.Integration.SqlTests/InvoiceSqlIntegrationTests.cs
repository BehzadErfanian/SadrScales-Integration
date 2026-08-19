using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadrScales.Integration.Invoices;

namespace SadrScales.Integration.SqlTests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class InvoiceSqlIntegrationTests
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
            Database.ExecuteNonQuery(InvoiceSchemaSql);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _database?.Dispose();
            _database = null;
        }

        #endregion

        #region Lookup Tests

        [TestMethod]
        public async Task Invoice_Lookup_Should_Return_Full_Unread_Data_Without_AutoAck()
        {
            ResetInvoices();

            const int scaleId = 3;
            const int fid = 12345;
            string barcode = SadrInvoiceClient.BuildTotalBarcode(scaleId, fid);
            int totalId = InsertInvoice(barcode, scaleId, 88, acknowledged: false);
            InsertDetails(totalId, barcode);

            var result = await CreateClient().Invoices.GetByBarcodeAsync(barcode);

            Assert.AreEqual(SadrInvoiceLookupStatus.FoundUnread, result.Status);
            Assert.IsNotNull(result.Invoice);
            Assert.AreEqual(totalId, result.Invoice!.TotalId);
            Assert.AreEqual(scaleId, result.Invoice.ScaleId);
            Assert.AreEqual(88, result.Invoice.ReceiptNo);
            Assert.AreEqual(barcode, result.Invoice.TotalBarcode);
            Assert.AreEqual(barcode, result.Invoice.ItemBarcode);
            Assert.AreEqual(2, result.Invoice.TransactionCount);
            Assert.AreEqual(10, result.Invoice.SubDiscountAmount);
            Assert.AreEqual(20, result.Invoice.DiscountAmount);
            Assert.AreEqual(30, result.Invoice.ATaxAmount);
            Assert.AreEqual(40, result.Invoice.VTaxAmount);
            Assert.AreEqual(150000, result.Invoice.PriceWithTax);
            Assert.AreEqual(7, result.Invoice.ClerkNumber);
            Assert.IsFalse(result.Invoice.IsAcknowledged);
            Assert.AreEqual(2, result.Invoice.Items.Count);

            Assert.AreEqual(501, result.Invoice.Items[0].PluNo);
            Assert.AreEqual(1.250d, result.Invoice.Items[0].Weight);
            Assert.AreEqual(0d, result.Invoice.Items[0].Quantity);
            Assert.AreEqual(100000, result.Invoice.Items[0].UnitPrice);
            Assert.AreEqual(95000, result.Invoice.Items[0].UnitPriceAfterDiscount);
            Assert.AreEqual(118750, result.Invoice.Items[0].ActualPrice);

            Assert.AreEqual(502, result.Invoice.Items[1].PluNo);
            Assert.AreEqual(0d, result.Invoice.Items[1].Weight);
            Assert.AreEqual(2d, result.Invoice.Items[1].Quantity);

            int labelStatus = Database.ExecuteScalar<int>(
                "SELECT ISNULL(LableStatus, 0) FROM dbo.SADR_Total WHERE TotalID = " + totalId + ";");
            Assert.AreEqual(0, labelStatus, "Lookup must never acknowledge the source invoice automatically.");
        }

        [TestMethod]
        public async Task Invoice_GetByScaleAndFid_Should_Use_TotalBarcode_Identity_And_Report_NotFound()
        {
            ResetInvoices();

            const int scaleId = 12;
            const int fid = 654321;
            string barcode = SadrInvoiceClient.BuildTotalBarcode(scaleId, fid);
            int totalId = InsertInvoice(barcode, scaleId, 99, acknowledged: false);
            InsertDetails(totalId, barcode);

            var found = await CreateClient().Invoices.GetAsync(scaleId, fid);
            var missing = await CreateClient().Invoices.GetAsync(scaleId, fid + 1);

            Assert.AreEqual(SadrInvoiceLookupStatus.FoundUnread, found.Status);
            Assert.IsNotNull(found.Invoice);
            Assert.AreEqual(barcode, found.Invoice!.TotalBarcode);
            Assert.AreEqual(SadrInvoiceLookupStatus.NotFound, missing.Status);
            Assert.IsNull(missing.Invoice);
        }

        #endregion

        #region ACK Tests

        [TestMethod]
        public async Task Invoice_Ack_Should_Be_Idempotent_And_AlreadyRead_Should_Still_Return_Full_Data()
        {
            ResetInvoices();

            const int scaleId = 9;
            const int fid = 43210;
            string barcode = SadrInvoiceClient.BuildTotalBarcode(scaleId, fid);
            int totalId = InsertInvoice(barcode, scaleId, 77, acknowledged: false);
            InsertDetails(totalId, barcode);

            var client = CreateClient();
            var firstAck = await client.Invoices.AcknowledgeAsync(barcode);
            var secondAck = await client.Invoices.AcknowledgeAsync(barcode);
            var afterAck = await client.Invoices.GetByBarcodeAsync(barcode);
            var missingAck = await client.Invoices.AcknowledgeAsync(
                SadrInvoiceClient.BuildTotalBarcode(scaleId, fid + 1));

            Assert.AreEqual(SadrInvoiceAckStatus.Acknowledged, firstAck);
            Assert.AreEqual(SadrInvoiceAckStatus.AlreadyAcknowledged, secondAck);
            Assert.AreEqual(SadrInvoiceAckStatus.NotFound, missingAck);

            Assert.AreEqual(SadrInvoiceLookupStatus.AlreadyRead, afterAck.Status);
            Assert.IsNotNull(afterAck.Invoice);
            Assert.IsTrue(afterAck.Invoice!.IsAcknowledged);
            Assert.AreEqual(2, afterAck.Invoice.Items.Count,
                "AlreadyRead must return the complete invoice for recovery/re-import scenarios.");

            int labelStatus = Database.ExecuteScalar<int>(
                "SELECT ISNULL(LableStatus, 0) FROM dbo.SADR_Total WHERE TotalID = " + totalId + ";");
            Assert.AreEqual(1, labelStatus);
        }

        #endregion

        #region Test Data Helpers

        private static void ResetInvoices()
        {
            Database.ExecuteNonQuery("DELETE FROM dbo.SADR_Detail; DELETE FROM dbo.SADR_Total;");
        }

        private static int InsertInvoice(string barcode, int scaleId, int receiptNo, bool acknowledged)
        {
            string acknowledgedValue = acknowledged ? "1" : "0";

            Database.ExecuteNonQuery(@"
INSERT INTO dbo.SADR_Total
(
    ScaleID, SaleDateTime, LableStatus, ReceiptNo, TotalBarcode, ItemBarcode,
    NTrans, SubDiscAmt, DiscAmt, AmtOfATax, AmtOfVTax, PriceWTax, ClerkNo
)
VALUES
(" + scaleId + @", '2026-08-19T12:30:00', " + acknowledgedValue + ", " + receiptNo + @",
 '" + barcode + "', '" + barcode + @"', 2, 10, 20, 30, 40, 150000, 7);");

            return Database.ExecuteScalar<int>("SELECT MAX(TotalID) FROM dbo.SADR_Total;");
        }

        private static void InsertDetails(int totalId, string barcode)
        {
            Database.ExecuteNonQuery(@"
INSERT INTO dbo.SADR_Detail
(
    TotalID, TotalBarcode, ItemBarcode, TransNo, PluNo, Weight, QTY,
    Uprice, UpriceAfDisc, StPointDiscStat, TTLPriceDiscAmt, ActPrice, TaxRtNo, ItemStatus
)
VALUES
(" + totalId + @", '" + barcode + "', '2501" + barcode.Substring(2) + @"', 1, 501,
 1.250, 0, 100000, 95000, 0, 5000, 118750, 1, 0),
(" + totalId + @", '" + barcode + "', '2502" + barcode.Substring(2) + @"', 2, 502,
 0, 2, 15000, 15000, 0, 0, 30000, 0, 0);");
        }

        #endregion

        #region Synthetic 5.2.1 Structured Invoice Schema

        private const string InvoiceSchemaSql = @"
CREATE TABLE dbo.SADR_Total
(
    TotalID int IDENTITY(1,1) NOT NULL,
    ScaleID int NOT NULL,
    SaleDateTime smalldatetime NULL,
    LableStatus int NULL CONSTRAINT DF_SADR_Total_LableStatus DEFAULT((0)),
    ReceiptNo int NOT NULL,
    TotalBarcode varchar(50) NULL,
    ItemBarcode varchar(50) NOT NULL,
    NTrans int NULL,
    SubDiscAmt int NULL CONSTRAINT DF_SADR_Total_SubDiscAmt DEFAULT((0)),
    DiscAmt int NULL CONSTRAINT DF_SADR_Total_DiscAmt DEFAULT((0)),
    AmtOfATax int NULL CONSTRAINT DF_SADR_Total_AmtOfATax DEFAULT((0)),
    AmtOfVTax int NULL CONSTRAINT DF_SADR_Total_AmtOfVTax DEFAULT((0)),
    PriceWTax int NULL CONSTRAINT DF_SADR_Total_PriceWTax DEFAULT((0)),
    ClerkNo int NULL,
    CONSTRAINT PK_SADR_Total PRIMARY KEY CLUSTERED (ItemBarcode ASC),
    CONSTRAINT UX_SADR_Total_TotalID UNIQUE NONCLUSTERED (TotalID),
    CONSTRAINT CK_SADR_Total_TotalID_NotZero CHECK (TotalID <> 0),
    CONSTRAINT CK_SADR_Total_ReceiptNo_NotZero CHECK (ReceiptNo <> 0)
);

CREATE TABLE dbo.SADR_Detail
(
    DetailID int IDENTITY(1,1) NOT NULL,
    TotalID int NOT NULL,
    TotalBarcode varchar(50) NULL,
    ItemBarcode varchar(50) NOT NULL,
    TransNo int NULL,
    PluNo int NOT NULL,
    Weight float NULL CONSTRAINT DF_SADR_Detail_Weight DEFAULT((0)),
    QTY float NULL CONSTRAINT DF_SADR_Detail_QTY DEFAULT((0)),
    Uprice int NULL,
    UpriceAfDisc int NULL CONSTRAINT DF_SADR_Detail_UpriceAfDisc DEFAULT((0)),
    StPointDiscStat int NULL CONSTRAINT DF_SADR_Detail_StPointDiscStat DEFAULT((0)),
    TTLPriceDiscAmt int NULL CONSTRAINT DF_SADR_Detail_TTLPriceDiscAmt DEFAULT((0)),
    ActPrice int NULL,
    TaxRtNo int NULL CONSTRAINT DF_SADR_Detail_TaxRtNo DEFAULT((0)),
    ItemStatus int NULL,
    CONSTRAINT PK_SADR_Detail PRIMARY KEY CLUSTERED (ItemBarcode ASC),
    CONSTRAINT FK_SADR_Detail_SADR_Total FOREIGN KEY(TotalID) REFERENCES dbo.SADR_Total(TotalID)
);
CREATE NONCLUSTERED INDEX IX_SADR_Detail_TotalID ON dbo.SADR_Detail(TotalID);
";

        #endregion
    }
}
