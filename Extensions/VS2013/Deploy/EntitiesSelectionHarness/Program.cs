using System;
using System.Windows.Forms;
using EntitiesSelectionWizard;

namespace EntitiesSelectionHarness
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool multiproject = MessageBox.Show(
                "Multi-Project?", "Project Type", MessageBoxButtons.YesNo) == DialogResult.Yes;
            bool webApiSharedPortable = false;
            if (multiproject)
            webApiSharedPortable = MessageBox.Show(
                "Web API Shared Portable?", "Shared Portable Exception", MessageBoxButtons.YesNo) == DialogResult.Yes;
            
            var result = DialogResult.OK;
            while (result != DialogResult.Cancel)
            {
                var dialog = new EntitiesSelectionDialog(multiproject, webApiSharedPortable);
                result = dialog.ShowDialog();
                if (result == DialogResult.Cancel) continue;
                string entitiesSelection = dialog.EntitiesSelection.ToString();
                MessageBox.Show(entitiesSelection, "Entities Selection");
            }
        }
    }
}
