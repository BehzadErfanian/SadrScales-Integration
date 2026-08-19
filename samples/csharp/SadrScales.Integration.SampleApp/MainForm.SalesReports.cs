using System;
using System.Windows.Forms;

namespace SadrScales.Integration.SampleApp
{
    public partial class MainForm
    {
        #region Sales and Reports Sample Composition

        private bool _salesReportsTabAdded;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (_salesReportsTabAdded)
            {
                return;
            }

            var control = new SalesReportsControl
            {
                Dock = DockStyle.Fill,
                ConnectionStringProvider = () => txtConnectionString.Text.Trim()
            };

            var page = new TabPage
            {
                Text = "Sales / Reports",
                Padding = new Padding(8),
                UseVisualStyleBackColor = true
            };
            page.Controls.Add(control);
            tabMain.Controls.Add(page);
            _salesReportsTabAdded = true;
        }

        #endregion
    }
}
