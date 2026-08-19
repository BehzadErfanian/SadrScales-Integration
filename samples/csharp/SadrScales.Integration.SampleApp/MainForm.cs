using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SadrScales.Integration.Invoices;
using SadrScales.Integration.Items;
using SadrScales.Integration.Scales;
using SadrScales.Integration.Stores;

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

            dgvInvoiceItems.DataSource = invoice.Items
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
            if (!chkEnableWrites.Checked ||
                !TryGetInvoiceInputs(out string connectionString, out string totalBarcode))
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

                dgvScales.DataSource = scales.Select(scale => new
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
                }).ToList();

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
                scaleId >= nudScaleId.Minimum && scaleId <= nudScaleId.Maximum)
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

        #region Store Catalog

        private async void btnRefreshStores_Click(object sender, EventArgs e)
        {
            await RefreshStoresAsync().ConfigureAwait(true);
        }

        private async Task RefreshStoresAsync()
        {
            if (!TryGetConnectionString(out string connectionString))
            {
                return;
            }

            SetBusy(true);
            try
            {
                var stores = await new SadrScalesClient(connectionString).Stores.GetAllAsync().ConfigureAwait(true);
                dgvStores.DataSource = stores.ToList();
                lblStoreStatus.Text = "Loaded " + stores.Count + " store(s).";
            }
            catch (Exception exception)
            {
                dgvStores.DataSource = null;
                lblStoreStatus.Text = "Store read failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void dgvStores_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStores.CurrentRow?.DataBoundItem is SadrStore store)
            {
                txtStoreCode.Text = store.StoreCode;
                txtStoreName.Text = store.StoreName ?? string.Empty;
                txtStoreDescription.Text = store.Descriptions ?? string.Empty;
            }
        }

        private async void btnUpsertStore_Click(object sender, EventArgs e)
        {
            if (!chkEnableCatalogWrites.Checked || !TryGetConnectionString(out string connectionString))
            {
                return;
            }

            var store = new SadrStore
            {
                StoreCode = txtStoreCode.Text.Trim(),
                StoreName = EmptyToNull(txtStoreName.Text),
                Descriptions = EmptyToNull(txtStoreDescription.Text)
            };

            SetBusy(true);
            try
            {
                SadrStoreUpsertResult result =
                    await new SadrScalesClient(connectionString).Stores.UpsertAsync(store).ConfigureAwait(true);
                lblStoreStatus.Text = "Store upsert: " + result;
                await RefreshStoresAsync().ConfigureAwait(true);
            }
            catch (Exception exception)
            {
                lblStoreStatus.Text = "Store upsert failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        #endregion

        #region Item Group Catalog

        private async void btnRefreshGroups_Click(object sender, EventArgs e)
        {
            await RefreshGroupsAsync().ConfigureAwait(true);
        }

        private async Task RefreshGroupsAsync()
        {
            if (!TryGetConnectionString(out string connectionString))
            {
                return;
            }

            SetBusy(true);
            try
            {
                var groups = await new SadrScalesClient(connectionString).ItemGroups.GetAllAsync().ConfigureAwait(true);
                dgvGroups.DataSource = groups.ToList();
                lblGroupStatus.Text = "Loaded " + groups.Count + " item group(s).";
            }
            catch (Exception exception)
            {
                dgvGroups.DataSource = null;
                lblGroupStatus.Text = "Group read failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void dgvGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvGroups.CurrentRow?.DataBoundItem is SadrItemGroup group)
            {
                txtGroupCode.Text = group.ItemClassCode;
                txtGroupName.Text = group.ItemClassName ?? string.Empty;
                txtGroupDescription.Text = group.Descriptions ?? string.Empty;
            }
        }

        private async void btnUpsertGroup_Click(object sender, EventArgs e)
        {
            if (!chkEnableCatalogWrites.Checked || !TryGetConnectionString(out string connectionString))
            {
                return;
            }

            var group = new SadrItemGroup
            {
                ItemClassCode = txtGroupCode.Text.Trim(),
                ItemClassName = EmptyToNull(txtGroupName.Text),
                Descriptions = EmptyToNull(txtGroupDescription.Text)
            };

            SetBusy(true);
            try
            {
                SadrWriteResult result =
                    await new SadrScalesClient(connectionString).ItemGroups.UpsertAsync(group).ConfigureAwait(true);
                lblGroupStatus.Text = "Group upsert: " + result.Operation;
                await RefreshGroupsAsync().ConfigureAwait(true);
            }
            catch (Exception exception)
            {
                lblGroupStatus.Text = "Group upsert failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        #endregion

        #region Item Catalog

        private async void btnRefreshCatalogItems_Click(object sender, EventArgs e)
        {
            await RefreshCatalogItemsAsync().ConfigureAwait(true);
        }

        private async Task RefreshCatalogItemsAsync()
        {
            if (!TryGetConnectionString(out string connectionString))
            {
                return;
            }

            SetBusy(true);
            try
            {
                var items = await new SadrScalesClient(connectionString)
                    .Items.GetAllAsync(chkIncludeDeletedItems.Checked)
                    .ConfigureAwait(true);

                dgvCatalogItems.DataSource = items.Select(item => new
                {
                    item.PluNo,
                    item.ItemClassCode,
                    item.PluName,
                    item.UnitPrice,
                    item.IndexBarcode,
                    item.PluUnit,
                    item.DeleteFlag
                }).ToList();

                lblItemStatus.Text = "Loaded " + items.Count + " item(s).";
            }
            catch (Exception exception)
            {
                dgvCatalogItems.DataSource = null;
                lblItemStatus.Text = "Item read failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void dgvCatalogItems_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCatalogItems.CurrentRow?.Cells["PluNo"].Value is int pluNo)
            {
                nudItemPlu.Value = ClampDecimal(pluNo, nudItemPlu.Minimum, nudItemPlu.Maximum);
                txtItemGroup.Text = Convert.ToString(dgvCatalogItems.CurrentRow.Cells["ItemClassCode"].Value) ?? "0";
                txtItemName.Text = Convert.ToString(dgvCatalogItems.CurrentRow.Cells["PluName"].Value) ?? string.Empty;
                if (dgvCatalogItems.CurrentRow.Cells["UnitPrice"].Value is int price)
                {
                    nudItemPrice.Value = ClampDecimal(price, nudItemPrice.Minimum, nudItemPrice.Maximum);
                }
            }
        }

        private async void btnUpsertItem_Click(object sender, EventArgs e)
        {
            if (!chkEnableCatalogWrites.Checked || !TryGetConnectionString(out string connectionString))
            {
                return;
            }

            int pluNo = Decimal.ToInt32(nudItemPlu.Value);
            var client = new SadrScalesClient(connectionString);

            SetBusy(true);
            try
            {
                // Preserve non-edited settings on an existing PLU instead of overwriting them with defaults.
                SadrItem item = await client.Items.GetAsync(pluNo).ConfigureAwait(true) ?? new SadrItem { PluNo = pluNo };
                item.ItemClassCode = txtItemGroup.Text.Trim();
                item.PluName = EmptyToNull(txtItemName.Text);
                item.UnitPrice = Decimal.ToInt32(nudItemPrice.Value);
                item.DeleteFlag = 0;

                SadrWriteResult result = await client.Items.UpsertAsync(item).ConfigureAwait(true);
                lblItemStatus.Text = "Item upsert: " + result.Operation;
                await RefreshCatalogItemsAsync().ConfigureAwait(true);
            }
            catch (Exception exception)
            {
                lblItemStatus.Text = "Item upsert failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void btnSoftDeleteItem_Click(object sender, EventArgs e)
        {
            if (!chkEnableCatalogWrites.Checked || !TryGetConnectionString(out string connectionString))
            {
                return;
            }

            int pluNo = Decimal.ToInt32(nudItemPlu.Value);
            DialogResult confirmation = MessageBox.Show(
                this,
                "Soft-delete PLU " + pluNo + "?\r\n\r\nThe row is not physically removed. DeleteFlag becomes 1 and remains inspectable/recoverable.",
                "Confirm logical delete",
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
                SadrItemSoftDeleteResult result =
                    await new SadrScalesClient(connectionString).Items.SoftDeleteAsync(pluNo).ConfigureAwait(true);
                lblItemStatus.Text = "Soft delete: " + result;
                await RefreshCatalogItemsAsync().ConfigureAwait(true);
            }
            catch (Exception exception)
            {
                lblItemStatus.Text = "Soft delete failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void btnLoadPriceHistory_Click(object sender, EventArgs e)
        {
            if (!TryGetConnectionString(out string connectionString))
            {
                return;
            }

            int pluNo = Decimal.ToInt32(nudItemPlu.Value);
            SetBusy(true);
            try
            {
                var history = await new SadrScalesClient(connectionString)
                    .Items.GetPriceHistoryAsync(pluNo, 100)
                    .ConfigureAwait(true);
                dgvPriceHistory.DataSource = history.ToList();
                lblItemStatus.Text = "Loaded " + history.Count + " price-history row(s) for PLU " + pluNo + ".";
            }
            catch (Exception exception)
            {
                dgvPriceHistory.DataSource = null;
                lblItemStatus.Text = "Price-history read failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        #endregion

        #region Catalog Write Guard

        private void chkEnableCatalogWrites_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCatalogWriteButtons();
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

            MessageBox.Show(this, "Enter a Sadr Scales SQL connection string.", "Missing connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            btnRefreshScales.Enabled = !busy;
            chkEnableScaleWrites.Enabled = !busy;
            nudScaleId.Enabled = !busy;
            btnRefreshStores.Enabled = !busy;
            btnRefreshGroups.Enabled = !busy;
            btnRefreshCatalogItems.Enabled = !busy;
            btnLoadPriceHistory.Enabled = !busy;
            chkEnableCatalogWrites.Enabled = !busy;
            UpdateScaleWriteButtons();
            UpdateCatalogWriteButtons();
        }

        private void UpdateScaleWriteButtons()
        {
            bool enabled = !UseWaitCursor && chkEnableScaleWrites.Checked;
            btnRequestItemResend.Enabled = enabled;
            btnRequestHotKeyResend.Enabled = enabled;
        }

        private void UpdateCatalogWriteButtons()
        {
            bool enabled = !UseWaitCursor && chkEnableCatalogWrites.Checked;
            btnUpsertStore.Enabled = enabled;
            btnUpsertGroup.Enabled = enabled;
            btnUpsertItem.Enabled = enabled;
            btnSoftDeleteItem.Enabled = enabled;
        }

        private void ClearInvoiceGrids()
        {
            dgvInvoice.DataSource = null;
            dgvInvoiceItems.DataSource = null;
        }

        private static string? EmptyToNull(string value)
        {
            string trimmed = value.Trim();
            return trimmed.Length == 0 ? null : trimmed;
        }

        private static decimal ClampDecimal(int value, decimal minimum, decimal maximum)
        {
            decimal result = value;
            if (result < minimum)
            {
                return minimum;
            }

            return result > maximum ? maximum : result;
        }

        #endregion
    }
}
