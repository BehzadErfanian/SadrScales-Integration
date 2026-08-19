using System;
using System.Windows.Forms;

namespace SadrScales.Integration.SampleApp
{
    internal static class Program
    {
        #region Entry Point

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        #endregion
    }
}
