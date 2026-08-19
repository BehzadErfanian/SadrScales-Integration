using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SadrScales.Integration.DemoLab;

namespace SadrScales.Integration.SampleApp
{
    /// <summary>
    /// Designer-based DemoLab UI. Every database write is gated by the explicit Demo marker safety model.
    /// </summary>
    internal sealed partial class DemoDataControl : UserControl
    {
        #region State

        private bool _busy;
        private SadrDemoDatabaseSafety? _lastSafety;

        /// <summary>Gets or sets the shared connection-string provider supplied by the host form.</summary>
        public Func<string?>? ConnectionStringProvider { get; set; }

        #endregion

        #region Construction

        public DemoDataControl()
        {
            InitializeComponent();
            ShowPreview(CreateScenario());
        }

        #endregion

        #region Inspect / Marker

        private async void btnInspect_Click(object sender, EventArgs e)
        {
            await InspectAsync().ConfigureAwait(true);
        }

        private async Task InspectAsync()
        {
            if (!TryGetConnectionString(out string connectionString))
            {
                return;
            }

            SetBusy(true);
            try
            {
                var guard = new SadrDemoDatabaseGuard(connectionString);
                _lastSafety = await guard.InspectAsync().ConfigureAwait(true);
                lblSafety.Text = FormatSafety(_lastSafety);
            }
            catch (Exception exception)
            {
                _lastSafety = null;
                lblSafety.Text = "Inspection failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void btnInitializeMarker_Click(object sender, EventArgs e)
        {
            if (!TryGetConnectionString(out string connectionString))
            {
                return;
            }

            string confirmation = txtConfirmDatabase.Text.Trim();
            if (confirmation.Length == 0)
            {
                MessageBox.Show(this, "Type the exact DB_NAME() value shown by Inspect first.",
                    "Database confirmation required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show(
                this,
                "Mark database '" + confirmation + "' as an Integration DemoLab database?\r\n\r\n" +
                "This is allowed only when the database name is clearly non-production and business data is empty.",
                "Initialize Demo marker",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
                return;

            SetBusy(true);
            try
            {
                var guard = new SadrDemoDatabaseGuard(connectionString);
                await guard.InitializeMarkerAsync(confirmation).ConfigureAwait(true);
                lblResult.Text = "Demo marker initialized. Inspect state refreshed.";
            }
            catch (Exception exception)
            {
                lblResult.Text = "Marker initialization failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }

            await InspectAsync().ConfigureAwait(true);
        }

        #endregion

        #region Scenario Preview / Seed

        private void btnRandomSeed_Click(object sender, EventArgs e)
        {
            int seed = new Random(Environment.TickCount ^ Guid.NewGuid().GetHashCode()).Next(1, int.MaxValue);
            nudSeed.Value = seed;
            ShowPreview(CreateScenario());
            lblResult.Text = "Random seed selected: " + seed + ". Keep this number to reproduce the same scenario.";
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            SadrDemoScenario scenario = CreateScenario();
            ShowPreview(scenario);
            lblResult.Text = "Preview only — no database write. Seed " + scenario.Seed + ".";
        }

        private SadrDemoScenario CreateScenario()
        {
            return SadrDemoScenarioFactory.Create(new SadrDemoScenarioOptions
            {
                Seed = Decimal.ToInt32(nudSeed.Value)
            });
        }

        private void ShowPreview(SadrDemoScenario scenario)
        {
            var rows = new List<DemoPreviewRow>();

            rows.AddRange(scenario.Stores.Select(store => new DemoPreviewRow
            {
                Type = "Store",
                Key = store.StoreCode,
                Name = store.StoreName ?? string.Empty,
                Details = ""
            }));
            rows.AddRange(scenario.Groups.Select(group => new DemoPreviewRow
            {
                Type = "Group",
                Key = group.ItemClassCode,
                Name = group.ItemClassName ?? string.Empty,
                Details = ""
            }));
            rows.AddRange(scenario.Scales.Select(scale => new DemoPreviewRow
            {
                Type = "Scale",
                Key = scale.ScaleId.ToString(),
                Name = scale.DeviceName,
                Details = scale.Model + " | " + scale.IpAddress + " | Store=" + scale.StoreCode + " | disabled"
            }));
            rows.AddRange(scenario.Items.Take(20).Select(item => new DemoPreviewRow
            {
                Type = "Item",
                Key = item.PluNo.ToString(),
                Name = item.PluName ?? string.Empty,
                Details = "Group=" + item.ItemClassCode + " | Price=" + item.UnitPrice
            }));
            rows.AddRange(scenario.Invoices.Take(10).Select(invoice => new DemoPreviewRow
            {
                Type = "Invoice",
                Key = invoice.TotalBarcode,
                Name = "Scale " + invoice.ScaleId + " / FID " + invoice.Fid,
                Details = invoice.Lines.Count + " line(s) | " + (invoice.IsAcknowledged ? "AlreadyRead" : "Unread")
            }));

            dgvPreview.DataSource = rows;
        }

        #endregion

        #region Generate / Reset

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            if (!TryGetConnectionString(out string connectionString))
            {
                return;
            }

            int seed = Decimal.ToInt32(nudSeed.Value);
            DialogResult confirmation = MessageBox.Show(
                this,
                "Generate the complete synthetic Integration scenario with Seed " + seed + "?\r\n\r\n" +
                "This is allowed only on an EMPTY database that already has the valid DemoLab marker.\r\n" +
                "Demo scales are inserted disabled/offline and use TEST-NET addresses.",
                "Generate Demo Data",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirmation != DialogResult.Yes)
                return;

            SetBusy(true);
            try
            {
                var writer = new SadrDemoDataWriter(connectionString);
                SadrDemoGenerationResult generated = await writer.GenerateAsync(
                    new SadrDemoScenarioOptions { Seed = seed }).ConfigureAwait(true);

                lblResult.Text = "Generated Seed " + generated.Seed +
                    " — Stores=" + generated.StoreCount +
                    ", Groups=" + generated.GroupCount +
                    ", Items=" + generated.ItemCount +
                    ", Scales=" + generated.ScaleCount +
                    ", Invoices=" + generated.InvoiceCount +
                    ", Sales rows=" + generated.SalesRowCount + ".";

                ShowPreview(CreateScenario());
            }
            catch (Exception exception)
            {
                lblResult.Text = "Demo generation failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }

            await InspectAsync().ConfigureAwait(true);
        }

        private async void btnReset_Click(object sender, EventArgs e)
        {
            if (!TryGetConnectionString(out string connectionString))
            {
                return;
            }

            DialogResult confirmation = MessageBox.Show(
                this,
                "Reset Demo Data in this MARKED DemoLab database?\r\n\r\n" +
                "The operation removes demo business rows but preserves the database schema, Demo marker and default Store/Group.",
                "Reset Demo Data",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirmation != DialogResult.Yes)
                return;

            SetBusy(true);
            try
            {
                await new SadrDemoDatabaseGuard(connectionString)
                    .ResetDemoDataAsync()
                    .ConfigureAwait(true);
                lblResult.Text = "Demo Data reset completed. Marker and schema were preserved.";
            }
            catch (Exception exception)
            {
                lblResult.Text = "Demo reset failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }

            await InspectAsync().ConfigureAwait(true);
        }

        #endregion

        #region UI Helpers

        private bool TryGetConnectionString(out string connectionString)
        {
            connectionString = ConnectionStringProvider?.Invoke()?.Trim()
                ?? Environment.GetEnvironmentVariable("SADR_SCALES_CONNECTION_STRING")?.Trim()
                ?? string.Empty;

            if (connectionString.Length > 0)
                return true;

            MessageBox.Show(this, "Enter the Sadr Scales SQL connection string in the main Sample window.",
                "Missing connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private string FormatSafety(SadrDemoDatabaseSafety safety)
        {
            return "DB=" + safety.DatabaseName +
                " | SafeName=" + safety.HasSafeDemoName +
                " | Schema=" + safety.HasRequiredSchema +
                " | Empty=" + safety.IsBusinessDataEmpty +
                " | Marker=" + safety.HasDemoMarker +
                " | " + safety.Message;
        }

        private void SetBusy(bool busy)
        {
            _busy = busy;
            UseWaitCursor = busy;
            btnInspect.Enabled = !busy;
            btnRandomSeed.Enabled = !busy;
            btnPreview.Enabled = !busy;
            txtConfirmDatabase.Enabled = !busy;
            nudSeed.Enabled = !busy;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            btnInitializeMarker.Enabled = !_busy && _lastSafety != null && _lastSafety.CanInitializeMarker;
            btnGenerate.Enabled = !_busy && _lastSafety != null &&
                _lastSafety.CanWriteDemoData && _lastSafety.IsBusinessDataEmpty;
            btnReset.Enabled = !_busy && _lastSafety != null &&
                _lastSafety.CanWriteDemoData && !_lastSafety.IsBusinessDataEmpty;
        }

        private sealed class DemoPreviewRow
        {
            public string Type { get; set; } = string.Empty;
            public string Key { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Details { get; set; } = string.Empty;
        }

        #endregion
    }
}
