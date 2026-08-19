using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SadrScales.Integration.Assignments;
using SadrScales.Integration.HotKeys;

namespace SadrScales.Integration.SampleApp
{
    /// <summary>
    /// Designer-based reference UI for scale assignments, per-scale mapping and group HotKey templates.
    /// </summary>
    internal sealed partial class ScaleConfigurationControl : UserControl
    {
        #region State

        private BindingList<SadrScaleItemMap> _mappingRows = new BindingList<SadrScaleItemMap>();
        private BindingList<SadrHotKey> _hotKeyRows = new BindingList<SadrHotKey>();
        private bool _busy;

        /// <summary>
        /// Gets or sets the shared connection-string provider supplied by the host form.
        /// </summary>
        public Func<string?>? ConnectionStringProvider { get; set; }

        #endregion

        #region Construction

        public ScaleConfigurationControl()
        {
            InitializeComponent();
            BindMappingRows(_mappingRows);
            BindHotKeyRows(_hotKeyRows);
            UpdateWriteButtons();
        }

        #endregion

        #region Scale Assignments

        private async void btnLoadAssignments_Click(object sender, EventArgs e)
        {
            await LoadAssignmentsAsync().ConfigureAwait(true);
        }

        private async Task LoadAssignmentsAsync()
        {
            if (!TryGetConnectionString(out string connectionString))
            {
                return;
            }

            int scaleId = Decimal.ToInt32(nudAssignmentScale.Value);
            SetBusy(true);
            try
            {
                var client = new SadrScalesClient(connectionString);
                IReadOnlyList<string> groups =
                    await client.ScaleAssignments.GetGroupsAsync(scaleId).ConfigureAwait(true);

                dgvAssignments.DataSource = groups
                    .Select(code => new AssignmentRow { ItemClassCode = code })
                    .ToList();
                txtAssignmentGroups.Text = string.Join(", ", groups);
                lblAssignmentStatus.Text = "Loaded " + groups.Count + " assigned group(s).";
            }
            catch (Exception exception)
            {
                dgvAssignments.DataSource = null;
                lblAssignmentStatus.Text = "Assignment read failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void btnReplaceAssignments_Click(object sender, EventArgs e)
        {
            if (!CanWrite() || !TryGetConnectionString(out string connectionString))
            {
                return;
            }

            int scaleId = Decimal.ToInt32(nudAssignmentScale.Value);
            string[] groups = txtAssignmentGroups.Text
                .Split(new[] { ',', ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .Where(value => value.Length > 0)
                .ToArray();

            if (!Confirm(
                "Replace the complete item-group assignment set for Scale " + scaleId + "?\r\n\r\n" +
                "A real change records an Item AutoSend resend request."))
            {
                return;
            }

            SetBusy(true);
            try
            {
                SadrReplaceResult result = await new SadrScalesClient(connectionString)
                    .ScaleAssignments.ReplaceGroupsAsync(scaleId, groups)
                    .ConfigureAwait(true);
                lblAssignmentStatus.Text = "Replace groups: " + result;
            }
            catch (Exception exception)
            {
                lblAssignmentStatus.Text = "Assignment replace failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }

            await LoadAssignmentsAsync().ConfigureAwait(true);
        }

        #endregion

        #region Scale Mapping

        private async void btnLoadMappings_Click(object sender, EventArgs e)
        {
            await LoadMappingsAsync().ConfigureAwait(true);
        }

        private async Task LoadMappingsAsync()
        {
            if (!TryGetConnectionString(out string connectionString))
            {
                return;
            }

            int scaleId = Decimal.ToInt32(nudMappingScale.Value);
            SetBusy(true);
            try
            {
                var client = new SadrScalesClient(connectionString);
                IReadOnlyList<SadrScaleItemMap> mappings =
                    await client.ScaleMappings.GetAsync(scaleId).ConfigureAwait(true);

                BindMappingRows(new BindingList<SadrScaleItemMap>(mappings
                    .Select(item => new SadrScaleItemMap
                    {
                        ScaleId = item.ScaleId,
                        PluNo = item.PluNo,
                        ItemCode = item.ItemCode,
                        PageNo = item.PageNo,
                        KeyNo = item.KeyNo
                    })
                    .ToList()));

                lblMappingStatus.Text = "Loaded " + mappings.Count + " mapping row(s).";
            }
            catch (Exception exception)
            {
                BindMappingRows(new BindingList<SadrScaleItemMap>());
                lblMappingStatus.Text = "Mapping read failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void btnAddMapping_Click(object sender, EventArgs e)
        {
            int nextItemCode = _mappingRows.Count == 0
                ? 1
                : Math.Max(1, _mappingRows.Max(item => item.ItemCode) + 1);

            _mappingRows.Add(new SadrScaleItemMap
            {
                PluNo = 1,
                ItemCode = nextItemCode
            });
        }

        private void btnRemoveMapping_Click(object sender, EventArgs e)
        {
            if (dgvMappings.CurrentRow?.DataBoundItem is SadrScaleItemMap item)
            {
                _mappingRows.Remove(item);
            }
        }

        private async void btnReplaceMappings_Click(object sender, EventArgs e)
        {
            if (!CanWrite() || !TryGetConnectionString(out string connectionString))
            {
                return;
            }

            dgvMappings.EndEdit();
            int scaleId = Decimal.ToInt32(nudMappingScale.Value);

            if (!Confirm(
                "Replace the COMPLETE item mapping for Scale " + scaleId + " with the rows currently shown?\r\n\r\n" +
                "A real change resets Item and HotKey AutoSend state."))
            {
                return;
            }

            SetBusy(true);
            try
            {
                SadrReplaceResult result = await new SadrScalesClient(connectionString)
                    .ScaleMappings.ReplaceAsync(scaleId, _mappingRows.ToList())
                    .ConfigureAwait(true);
                lblMappingStatus.Text = "Replace mapping: " + result;
            }
            catch (Exception exception)
            {
                lblMappingStatus.Text = "Mapping replace failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }

            await LoadMappingsAsync().ConfigureAwait(true);
        }

        private async void btnCopyMappings_Click(object sender, EventArgs e)
        {
            if (!CanWrite() || !TryGetConnectionString(out string connectionString))
            {
                return;
            }

            int sourceScaleId = Decimal.ToInt32(nudMappingScale.Value);
            int destinationScaleId = Decimal.ToInt32(nudMappingDestination.Value);

            if (!Confirm(
                "Copy the COMPLETE mapping from Scale " + sourceScaleId + " to Scale " + destinationScaleId + "?\r\n\r\n" +
                "The destination is replaced only if the source layout is compatible."))
            {
                return;
            }

            SetBusy(true);
            try
            {
                SadrReplaceResult result = await new SadrScalesClient(connectionString)
                    .ScaleMappings.CopyAsync(sourceScaleId, destinationScaleId)
                    .ConfigureAwait(true);
                lblMappingStatus.Text = "Copy mapping: " + result;

                if (result != SadrReplaceResult.NotFound)
                {
                    nudMappingScale.Value = destinationScaleId;
                }
            }
            catch (Exception exception)
            {
                lblMappingStatus.Text = "Mapping copy failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }

            await LoadMappingsAsync().ConfigureAwait(true);
        }

        private void BindMappingRows(BindingList<SadrScaleItemMap> rows)
        {
            _mappingRows = rows;
            dgvMappings.DataSource = _mappingRows;
        }

        #endregion

        #region Group HotKeys

        private async void btnLoadHotKeys_Click(object sender, EventArgs e)
        {
            await LoadHotKeysAsync().ConfigureAwait(true);
        }

        private async Task LoadHotKeysAsync()
        {
            if (!TryGetConnectionString(out string connectionString))
            {
                return;
            }

            string groupCode = txtHotKeyGroup.Text.Trim();
            SetBusy(true);
            try
            {
                var client = new SadrScalesClient(connectionString);
                IReadOnlyList<SadrHotKey> hotKeys =
                    await client.HotKeys.GetGroupAsync(groupCode).ConfigureAwait(true);

                BindHotKeyRows(new BindingList<SadrHotKey>(hotKeys
                    .Select(item => new SadrHotKey
                    {
                        PageNo = item.PageNo,
                        KeyNo = item.KeyNo,
                        PluNo = item.PluNo
                    })
                    .ToList()));

                lblHotKeyStatus.Text = "Loaded " + hotKeys.Count +
                    " user HotKey(s). Internal/system rows are intentionally hidden.";
            }
            catch (Exception exception)
            {
                BindHotKeyRows(new BindingList<SadrHotKey>());
                lblHotKeyStatus.Text = "HotKey read failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void btnAddHotKey_Click(object sender, EventArgs e)
        {
            int nextKey = _hotKeyRows.Count == 0
                ? 1
                : Math.Max(1, _hotKeyRows.Max(item => item.KeyNo) + 1);

            _hotKeyRows.Add(new SadrHotKey
            {
                PageNo = 0,
                KeyNo = nextKey,
                PluNo = 1
            });
        }

        private void btnRemoveHotKey_Click(object sender, EventArgs e)
        {
            if (dgvHotKeys.CurrentRow?.DataBoundItem is SadrHotKey item)
            {
                _hotKeyRows.Remove(item);
            }
        }

        private async void btnReplaceHotKeys_Click(object sender, EventArgs e)
        {
            if (!CanWrite() || !TryGetConnectionString(out string connectionString))
            {
                return;
            }

            dgvHotKeys.EndEdit();
            string groupCode = txtHotKeyGroup.Text.Trim();

            if (!Confirm(
                "Replace all USER HotKeys for group '" + groupCode + "' with the rows currently shown?\r\n\r\n" +
                "Internal/system rows with zero or negative PLUs are preserved."))
            {
                return;
            }

            SetBusy(true);
            try
            {
                SadrReplaceResult result = await new SadrScalesClient(connectionString)
                    .HotKeys.ReplaceGroupAsync(groupCode, _hotKeyRows.ToList())
                    .ConfigureAwait(true);
                lblHotKeyStatus.Text = "Replace HotKeys: " + result;
            }
            catch (Exception exception)
            {
                lblHotKeyStatus.Text = "HotKey replace failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }

            await LoadHotKeysAsync().ConfigureAwait(true);
        }

        private void BindHotKeyRows(BindingList<SadrHotKey> rows)
        {
            _hotKeyRows = rows;
            dgvHotKeys.DataSource = _hotKeyRows;
        }

        #endregion

        #region Write Guard

        private void chkEnableConfigurationWrites_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWriteButtons();
        }

        private bool CanWrite()
        {
            if (chkEnableConfigurationWrites.Checked)
            {
                return true;
            }

            MessageBox.Show(
                this,
                "Enable configuration writes before running a Replace or Copy operation.",
                "Writes disabled",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return false;
        }

        private bool Confirm(string message)
        {
            return MessageBox.Show(
                this,
                message,
                "Confirm configuration write",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        private void UpdateWriteButtons()
        {
            bool enabled = !_busy && chkEnableConfigurationWrites.Checked;
            btnReplaceAssignments.Enabled = enabled;
            btnReplaceMappings.Enabled = enabled;
            btnCopyMappings.Enabled = enabled;
            btnReplaceHotKeys.Enabled = enabled;
        }

        #endregion

        #region UI Helpers

        private bool TryGetConnectionString(out string connectionString)
        {
            connectionString = ConnectionStringProvider?.Invoke()?.Trim()
                ?? Environment.GetEnvironmentVariable("SADR_SCALES_CONNECTION_STRING")?.Trim()
                ?? string.Empty;

            if (connectionString.Length > 0)
            {
                return true;
            }

            MessageBox.Show(
                this,
                "Enter the Sadr Scales SQL connection string in the main Sample window.",
                "Missing connection",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return false;
        }

        private void SetBusy(bool busy)
        {
            _busy = busy;
            UseWaitCursor = busy;

            btnLoadAssignments.Enabled = !busy;
            btnLoadMappings.Enabled = !busy;
            btnLoadHotKeys.Enabled = !busy;
            btnAddMapping.Enabled = !busy;
            btnRemoveMapping.Enabled = !busy;
            btnAddHotKey.Enabled = !busy;
            btnRemoveHotKey.Enabled = !busy;
            chkEnableConfigurationWrites.Enabled = !busy;
            nudAssignmentScale.Enabled = !busy;
            nudMappingScale.Enabled = !busy;
            nudMappingDestination.Enabled = !busy;
            txtAssignmentGroups.Enabled = !busy;
            txtHotKeyGroup.Enabled = !busy;
            dgvMappings.Enabled = !busy;
            dgvHotKeys.Enabled = !busy;

            UpdateWriteButtons();
        }

        private sealed class AssignmentRow
        {
            public string ItemClassCode { get; set; } = string.Empty;
        }

        #endregion
    }
}
