using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SadrScales.Integration.Sales;

namespace SadrScales.Integration.SampleApp
{
    /// <summary>
    /// Designer-based read-only reference UI for filtered sales query and typed reports.
    /// </summary>
    internal sealed partial class SalesReportsControl : UserControl
    {
        #region State

        private bool _busy;

        /// <summary>Gets or sets the shared connection-string provider supplied by the host form.</summary>
        public Func<string?>? ConnectionStringProvider { get; set; }

        #endregion

        #region Construction

        public SalesReportsControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Period Presets

        private void btnToday_Click(object sender, EventArgs e)
        {
            ApplyPeriod(SadrSalesPeriodPreset.Today);
        }

        private void btnWeek_Click(object sender, EventArgs e)
        {
            ApplyPeriod(SadrSalesPeriodPreset.CurrentWeek);
        }

        private void btnMonth_Click(object sender, EventArgs e)
        {
            ApplyPeriod(SadrSalesPeriodPreset.CurrentMonth);
        }

        private void btnClearDates_Click(object sender, EventArgs e)
        {
            dtpStart.Checked = false;
            dtpEnd.Checked = false;
            lblStatus.Text = "Date filter cleared.";
        }

        private void ApplyPeriod(SadrSalesPeriodPreset preset)
        {
            SadrSalesDateRange range = SadrSalesPeriod.GetRange(preset, DateTime.Now);
            dtpStart.Value = range.StartDateInclusive;
            dtpEnd.Value = range.EndDateExclusive;
            dtpStart.Checked = true;
            dtpEnd.Checked = true;
            lblStatus.Text = preset + ": " +
                range.StartDateInclusive.ToString("yyyy-MM-dd") + " <= sale < " +
                range.EndDateExclusive.ToString("yyyy-MM-dd");
        }

        #endregion

        #region Query

        private async void btnQuery_Click(object sender, EventArgs e)
        {
            if (!TryCreateContext(out string connectionString, out SadrSalesQueryFilter filter))
            {
                return;
            }

            SetBusy(true);
            try
            {
                SadrSalesPage page = await new SadrScalesClient(connectionString)
                    .Sales.QueryAsync(filter)
                    .ConfigureAwait(true);

                dgvResults.DataSource = page.Rows
                    .Select(row => new
                    {
                        row.Id,
                        ScaleId = row.DeviceNo,
                        row.Identify,
                        row.DateTime,
                        row.Fid,
                        row.SubId,
                        row.Plu,
                        row.PluName,
                        row.Amount,
                        row.Unit,
                        row.UnitPrice,
                        row.TotalPrice,
                        row.Tax,
                        row.Salesman
                    })
                    .ToList();

                ShowSummary(page.Summary);
                lblStatus.Text = "Query page " + page.PageNumber + " / " + page.PageCount +
                    " — normalized page size " + page.PageSize + ". Feed cursor unchanged.";
            }
            catch (Exception exception)
            {
                dgvResults.DataSource = null;
                lblSummary.Text = "Summary: —";
                lblStatus.Text = "Sales query failed: " + exception.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        #endregion

        #region Reports

        private async void btnDaily_Click(object sender, EventArgs e)
        {
            if (!TryCreateContext(out string connectionString, out SadrSalesQueryFilter filter))
            {
                return;
            }

            SetBusy(true);
            try
            {
                var rows = await new SadrScalesClient(connectionString)
                    .Reports.GetDailyAsync(filter)
                    .ConfigureAwait(true);

                dgvResults.DataSource = rows.Select(row => new
                {
                    row.SaleDate,
                    row.Summary.RecordCount,
                    row.Summary.InvoiceCount,
                    row.Summary.TotalPrice,
                    row.Summary.TotalWeight,
                    row.Summary.TotalQuantity
                }).ToList();

                ShowReportTotals(rows.Select(row => row.Summary));
                lblStatus.Text = "Daily report: " + rows.Count + " aggregate row(s).";
            }
            catch (Exception exception)
            {
                ShowFailure("Daily report", exception);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void btnByScale_Click(object sender, EventArgs e)
        {
            if (!TryCreateContext(out string connectionString, out SadrSalesQueryFilter filter))
            {
                return;
            }

            SetBusy(true);
            try
            {
                var rows = await new SadrScalesClient(connectionString)
                    .Reports.GetByScaleAsync(filter)
                    .ConfigureAwait(true);

                dgvResults.DataSource = rows.Select(row => new
                {
                    row.ScaleId,
                    row.Identify,
                    row.Summary.RecordCount,
                    row.Summary.InvoiceCount,
                    row.Summary.TotalPrice,
                    row.Summary.TotalWeight,
                    row.Summary.TotalQuantity
                }).ToList();

                ShowReportTotals(rows.Select(row => row.Summary));
                lblStatus.Text = "Scale report: " + rows.Count + " aggregate row(s).";
            }
            catch (Exception exception)
            {
                ShowFailure("Scale report", exception);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void btnByItem_Click(object sender, EventArgs e)
        {
            if (!TryCreateContext(out string connectionString, out SadrSalesQueryFilter filter))
            {
                return;
            }

            SetBusy(true);
            try
            {
                var rows = await new SadrScalesClient(connectionString)
                    .Reports.GetByItemAsync(filter)
                    .ConfigureAwait(true);

                dgvResults.DataSource = rows.Select(row => new
                {
                    row.Plu,
                    row.PluName,
                    row.Summary.RecordCount,
                    row.Summary.InvoiceCount,
                    row.Summary.TotalPrice,
                    row.Summary.TotalWeight,
                    row.Summary.TotalQuantity
                }).ToList();

                ShowReportTotals(rows.Select(row => row.Summary));
                lblStatus.Text = "Item report: " + rows.Count +
                    " aggregate row(s), maximum " + SadrScales.Integration.Reports.SadrReportClient.MaximumItemReportRows + ".";
            }
            catch (Exception exception)
            {
                ShowFailure("Item report", exception);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ShowReportTotals(System.Collections.Generic.IEnumerable<SadrSalesSummary> summaries)
        {
            var list = summaries.ToList();
            var total = new SadrSalesSummaryForDisplay
            {
                RecordCount = list.Sum(value => value.RecordCount),
                // Invoice counts from grouped reports cannot safely be re-summed across every grouping type.
                InvoiceCount = null,
                TotalPrice = list.Sum(value => value.TotalPrice),
                TotalWeight = list.Sum(value => value.TotalWeight),
                TotalQuantity = list.Sum(value => value.TotalQuantity)
            };

            lblSummary.Text = "Visible report totals — Rows: " + total.RecordCount +
                " | InvoiceCount: see each aggregate row" +
                " | Price: " + total.TotalPrice.ToString("0.###") +
                " | Weight: " + total.TotalWeight.ToString("0.###") +
                " | Qty: " + total.TotalQuantity.ToString("0.###");
        }

        #endregion

        #region Filter Construction

        private bool TryCreateContext(out string connectionString, out SadrSalesQueryFilter filter)
        {
            filter = new SadrSalesQueryFilter();
            connectionString = ConnectionStringProvider?.Invoke()?.Trim()
                ?? Environment.GetEnvironmentVariable("SADR_SCALES_CONNECTION_STRING")?.Trim()
                ?? string.Empty;

            if (connectionString.Length == 0)
            {
                MessageBox.Show(this, "Enter the Sadr Scales SQL connection string in the main window.",
                    "Missing connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (!TryParseOptionalPositiveInt(txtScaleId.Text, "Scale ID", out int? scaleId) ||
                !TryParseOptionalPositiveInt(txtPlu.Text, "PLU", out int? plu) ||
                !TryParseOptionalPositiveInt(txtFid.Text, "FID", out int? fid))
            {
                return false;
            }

            if (scaleId.HasValue && scaleId.Value > 99)
            {
                MessageBox.Show(this, "Scale ID must be between 1 and 99.",
                    "Invalid Scale ID", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            filter.StartDateInclusive = dtpStart.Checked ? dtpStart.Value : (DateTime?)null;
            filter.EndDateExclusive = dtpEnd.Checked ? dtpEnd.Value : (DateTime?)null;
            filter.Identify = string.IsNullOrWhiteSpace(txtIdentify.Text) ? null : txtIdentify.Text.Trim();
            filter.ScaleId = scaleId;
            filter.Plu = plu;
            filter.Fid = fid;
            filter.PageNumber = Decimal.ToInt32(nudPage.Value);
            filter.PageSize = Decimal.ToInt32(nudPageSize.Value);
            return true;
        }

        private bool TryParseOptionalPositiveInt(string text, string title, out int? value)
        {
            value = null;
            if (string.IsNullOrWhiteSpace(text))
            {
                return true;
            }

            if (!int.TryParse(text.Trim(), out int parsed) || parsed <= 0)
            {
                MessageBox.Show(this, title + " must be a positive integer or blank.",
                    "Invalid filter", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            value = parsed;
            return true;
        }

        #endregion

        #region UI Helpers

        private void ShowSummary(SadrSalesSummary summary)
        {
            lblSummary.Text = "Summary — Rows: " + summary.RecordCount +
                " | Invoices: " + summary.InvoiceCount +
                " | Price: " + summary.TotalPrice.ToString("0.###") +
                " | Weight: " + summary.TotalWeight.ToString("0.###") +
                " | Qty: " + summary.TotalQuantity.ToString("0.###");
        }

        private void ShowFailure(string operation, Exception exception)
        {
            dgvResults.DataSource = null;
            lblSummary.Text = "Summary: —";
            lblStatus.Text = operation + " failed: " + exception.Message;
        }

        private void SetBusy(bool busy)
        {
            _busy = busy;
            UseWaitCursor = busy;
            btnQuery.Enabled = !busy;
            btnDaily.Enabled = !busy;
            btnByScale.Enabled = !busy;
            btnByItem.Enabled = !busy;
            btnToday.Enabled = !busy;
            btnWeek.Enabled = !busy;
            btnMonth.Enabled = !busy;
            btnClearDates.Enabled = !busy;
        }

        private sealed class SadrSalesSummaryForDisplay
        {
            public long RecordCount { get; set; }
            public long? InvoiceCount { get; set; }
            public decimal TotalPrice { get; set; }
            public decimal TotalWeight { get; set; }
            public decimal TotalQuantity { get; set; }
        }

        #endregion
    }
}
