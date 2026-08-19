using System;
using System.Windows.Forms;

namespace SadrScales.Integration.SampleApp
{
    public partial class MainForm
    {
        #region Additive Sample Composition

        private bool _salesReportsTabAdded;
        private bool _demoDataTabAdded;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            AddSalesReportsTab();
            AddDemoDataTab();
        }

        private void AddSalesReportsTab()
        {
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

        private void AddDemoDataTab()
        {
            if (_demoDataTabAdded)
            {
                return;
            }

            var control = new DemoDataControl
            {
                Dock = DockStyle.Fill,
                ConnectionStringProvider = () => txtConnectionString.Text.Trim()
            };

            var page = new TabPage
            {
                Text = "Demo Data",
                Padding = new Padding(8),
                UseVisualStyleBackColor = true
            };
            page.Controls.Add(control);
            tabMain.Controls.Add(page);
            _demoDataTabAdded = true;
        }

        #endregion
    }
}
