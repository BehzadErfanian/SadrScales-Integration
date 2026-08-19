using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SadrScales.Integration.Invoices;
using SadrScales.Integration.Scales;

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
            if (!TryGetInvoiceInputs(out string connectionString, out string totalBarcode))
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
                lblInvoiceStatus.Text = "Lookup failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void DisplayLookup(SadrInvoiceLookupResult result)
        {
            lblInvoiceStatus.Text = "Lookup: " + result.Status;

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
            btnAck.Enabled = chkEnableWrites.Checked && !UseWaitCursor;
        }

        private async void btnAck_Click(object sender, EventArgs e)
        {
            if (!chkEnableWrites.Checked)
            {
                return;
            }

            if (!TryGetInvoiceInputs(out string connectionString, out string totalBarcode))
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

                lblInvoiceStatus.Text = "ACK: " + result;

                // Re-read after ACK so the developer can see AlreadyRead while the full invoice remains available.
                SadrInvoiceLookupResult lookup =
                    await client.Invoices.GetByBarcodeAsync(totalBarcode).ConfigureAwait(true);
                DisplayLookup(lookup);
            }
            catch (Exception exception)
            {
                lblInvoiceStatus.Text = "ACK failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        #endregion

        #region Scale Read

        private async void btnRefreshScales_Click(object sender, EventArgs e)
        {
            await RefreshScalesAsync().ConfigureAwait(true);
        }

        private async Task RefreshScalesAsync()
        {
            if (!TryGetConnectionString(out string connectionString))
            {
                return;
            }

            SetBusy(true);

            try
            {
                var client = new SadrScalesClient(connectionString);
                var scales = await client.Scales.GetAllAsync().ConfigureAwait(true);

                dgvScales.DataSource = scales
                    .Select(scale => new
                    {
                        scale.ScaleId,
                        scale.DeviceName,
                        scale.IpAddress,
                        scale.Port,
                        scale.Model,
                        scale.StoreCode,
                        scale.StoreName,
                        scale.PrimaryItemGroupCode,
                        scale.Status,
                        scale.Used,
                        scale.AutoSendItems,
                        scale.AutoGetInvoice,
                        scale.Version,
                        scale.HotKeyCountPerPage,
                        scale.HotKeyPageCount
                    })
                    .ToList();

                lblScaleStatus.Text = "Loaded " + scales.Count + " registered scale(s).";
            }
            catch (Exception exception)
            {
                dgvScales.DataSource = null;
                lblScaleStatus.Text = "Scale read failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void dgvScales_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvScales.CurrentRow?.Cells["ScaleId"].Value is int scaleId &&
                scaleId >= nudScaleId.Minimum &&
                scaleId <= nudScaleId.Maximum)
            {
                nudScaleId.Value = scaleId;
            }
        }

        #endregion

        #region Scale Resend Requests

        private void chkEnableScaleWrites_CheckedChanged(object sender, EventArgs e)
        {
            UpdateScaleWriteButtons();
        }

        private async void btnRequestItemResend_Click(object sender, EventArgs e)
        {
            await RequestScaleResendAsync(hotKey: false).ConfigureAwait(true);
        }

        private async void btnRequestHotKeyResend_Click(object sender, EventArgs e)
        {
            await RequestScaleResendAsync(hotKey: true).ConfigureAwait(true);
        }

        private async Task RequestScaleResendAsync(bool hotKey)
        {
            if (!chkEnableScaleWrites.Checked || !TryGetConnectionString(out string connectionString))
            {
                return;
            }

            int scaleId = Decimal.ToInt32(nudScaleId.Value);
            string operationName = hotKey ? "HotKey" : "item";

            DialogResult confirmation = MessageBox.Show(
                this,
                "This writes a resend request for Scale " + scaleId + ".\r\n\r\n" +
                "Requested means the AutoSend watermark was reset in SQL. It does NOT mean the physical scale has already received the data.\r\n\r\n" +
                "Continue with the " + operationName + " resend request?",
                "Confirm resend request",
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
                SadrResendRequestResult result = hotKey
                    ? await client.Scales.RequestHotKeyResendAsync(scaleId).ConfigureAwait(true)
                    : await client.Scales.RequestItemResendAsync(scaleId).ConfigureAwait(true);

                lblScaleStatus.Text = operationName + " resend for Scale " + scaleId + ": " + result;

                if (result == SadrResendRequestResult.Requested)
                {
                    lblScaleStatus.Text += " — request recorded; wait for the next eligible AutoSend cycle.";
                }
                else if (result == SadrResendRequestResult.UnsupportedModel)
                {
                    lblScaleStatus.Text += " — this model has no 5.2.1 automatic HotKey-send path.";
                }
            }
            catch (Exception exception)
            {
                lblScaleStatus.Text = operationName + " resend failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        #endregion

        #region UI Helpers

        private bool TryGetConnectionString(out string connectionString)
        {
            connectionString = txtConnectionString.Text.Trim();
            if (connectionString.Length != 0)
            {
                return true;
            }

            MessageBox.Show(
                this,
                "Enter a Sadr Scales SQL connection string.",
                "Missing connection",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return false;
        }

        private bool TryGetInvoiceInputs(out string connectionString, out string totalBarcode)
        {
            totalBarcode = txtBarcode.Text.Trim();

            if (!TryGetConnectionString(out connectionString))
            {
                return false;
            }

            if (totalBarcode.Length == 0)
            {
                MessageBox.Show(
                    this,
                    "Enter the 14-digit structured TotalBarcode.",
                    "Missing barcode",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
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
            btnRefreshScales.Enabled = !busy;
            chkEnableScaleWrites.Enabled = !busy;
            nudScaleId.Enabled = !busy;
            UpdateScaleWriteButtons();
        }

        private void UpdateScaleWriteButtons()
        {
            bool enabled = !UseWaitCursor && chkEnableScaleWrites.Checked;
            btnRequestItemResend.Enabled = enabled;
            btnRequestHotKeyResend.Enabled = enabled;
        }

        private void ClearInvoiceGrids()
        {
            dgvInvoice.DataSource = null;
            dgvItems.DataSource = null;
        }

        #endregion
    }
}
