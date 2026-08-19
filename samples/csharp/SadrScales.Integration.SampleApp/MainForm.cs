using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SadrScales.Integration.Invoices;

namespace SadrScales.Integration.SampleApp
{
    /// <summary>
    /// Executable developer reference UI. Each Vendor-Ready capability is added here as a visible, testable flow.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Construction

        public MainForm()
        {
            InitializeComponent();

            string? connectionString = Environment.GetEnvironmentVariable("SADR_SCALES_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                txtConnectionString.Text = connectionString;
            }
        }

        #endregion

        #region Invoice Lookup

        private async void btnLookup_Click(object sender, EventArgs e)
        {
            await LookupInvoiceAsync().ConfigureAwait(true);
        }

        private async Task LookupInvoiceAsync()
        {
            if (!TryGetInputs(out string connectionString, out string totalBarcode))
            {
                return;
            }

            SetBusy(true);

            try
            {
                var client = new SadrScalesClient(connectionString);
                SadrInvoiceLookupResult result =
                    await client.Invoices.GetByBarcodeAsync(totalBarcode).ConfigureAwait(true);

                DisplayLookup(result);
            }
            catch (Exception exception)
            {
                ClearInvoiceGrids();
                lblStatus.Text = "Lookup failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void DisplayLookup(SadrInvoiceLookupResult result)
        {
            lblStatus.Text = "Lookup: " + result.Status;

            if (result.Invoice == null)
            {
                ClearInvoiceGrids();
                return;
            }

            SadrInvoice invoice = result.Invoice;

            dgvInvoice.DataSource = new[]
            {
                new
                {
                    invoice.ScaleId,
                    invoice.ReceiptNo,
                    invoice.TotalBarcode,
                    invoice.SaleDateTime,
                    invoice.TransactionCount,
                    invoice.PriceWithTax,
                    invoice.DiscountAmount,
                    invoice.ATaxAmount,
                    invoice.VTaxAmount,
                    invoice.ClerkNumber,
                    invoice.IsAcknowledged
                }
            };

            dgvItems.DataSource = invoice.Items
                .Select(item => new
                {
                    item.TransactionNo,
                    item.PluNo,
                    item.Weight,
                    item.Quantity,
                    item.UnitPrice,
                    item.UnitPriceAfterDiscount,
                    item.TotalPriceDiscountAmount,
                    item.ActualPrice,
                    item.TaxRateNo,
                    item.ItemBarcode
                })
                .ToList();
        }

        #endregion

        #region Invoice ACK

        private void chkEnableWrites_CheckedChanged(object sender, EventArgs e)
        {
            btnAck.Enabled = chkEnableWrites.Checked;
        }

        private async void btnAck_Click(object sender, EventArgs e)
        {
            if (!chkEnableWrites.Checked)
            {
                return;
            }

            if (!TryGetInputs(out string connectionString, out string totalBarcode))
            {
                return;
            }

            DialogResult confirmation = MessageBox.Show(
                this,
                "ACK changes SADR_Total.LableStatus to 1.\r\n\r\n" +
                "In a real POS/ERP flow, ACK only after your destination transaction has committed successfully.\r\n\r\n" +
                "Continue with this manual test?",
                "Confirm invoice ACK",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            SetBusy(true);

            try
            {
                var client = new SadrScalesClient(connectionString);
                SadrInvoiceAckStatus result =
                    await client.Invoices.AcknowledgeAsync(totalBarcode).ConfigureAwait(true);

                lblStatus.Text = "ACK: " + result;

                // Re-read after the explicit ACK so the developer can see AlreadyRead while the full data remains visible.
                SadrInvoiceLookupResult lookup =
                    await client.Invoices.GetByBarcodeAsync(totalBarcode).ConfigureAwait(true);
                DisplayLookup(lookup);
            }
            catch (Exception exception)
            {
                lblStatus.Text = "ACK failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        #endregion

        #region UI Helpers

        private bool TryGetInputs(out string connectionString, out string totalBarcode)
        {
            connectionString = txtConnectionString.Text.Trim();
            totalBarcode = txtBarcode.Text.Trim();

            if (connectionString.Length == 0)
            {
                MessageBox.Show(this, "Enter a Sadr Scales SQL connection string.", "Missing connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (totalBarcode.Length == 0)
            {
                MessageBox.Show(this, "Enter the 14-digit structured TotalBarcode.", "Missing barcode", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            return true;
        }

        private void SetBusy(bool busy)
        {
            UseWaitCursor = busy;
            btnLookup.Enabled = !busy;
            chkEnableWrites.Enabled = !busy;
            btnAck.Enabled = !busy && chkEnableWrites.Checked;
        }

        private void ClearInvoiceGrids()
        {
            dgvInvoice.DataSource = null;
            dgvItems.DataSource = null;
        }

        #endregion
    }
}
