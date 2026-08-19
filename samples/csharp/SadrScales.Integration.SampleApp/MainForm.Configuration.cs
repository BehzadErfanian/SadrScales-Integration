using System;
using System.Windows.Forms;

namespace SadrScales.Integration.SampleApp
{
    public partial class MainForm
    {
        #region Configuration Sample Composition

        private bool _configurationTabAdded;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (_configurationTabAdded)
            {
                return;
            }

            var configurationControl = new ScaleConfigurationControl
            {
                Dock = DockStyle.Fill,
                ConnectionStringProvider = () => txtConnectionString.Text.Trim()
            };

            var page = new TabPage
            {
                Text = "Assignments / Mapping / HotKeys",
                Padding = new Padding(8),
                UseVisualStyleBackColor = true
            };
            page.Controls.Add(configurationControl);

            tabMain.Controls.Add(page);
            _configurationTabAdded = true;
        }

        #endregion
    }
}
