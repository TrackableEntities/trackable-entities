using System;
using System.Windows.Forms;

namespace TrackableEntities.WcfItemWizard
{
    public partial class GetParamatersDialog : Form
    {
        public GetParamatersDialog()
        {
            InitializeComponent();
        }

        public string RootNamespace { get; set; }
        public string EntityName { get; set; }
        public string EntitySetName { get; set; }
        public string DbContextName { get; set; }

        private void okButton_Click(object sender, EventArgs e)
        {
            RootNamespace = namespaceTextBox.Text;
            EntityName = entityTextBox.Text;
            EntitySetName = entitySetTextBox.Text;
            DbContextName = dbContextTextBox.Text;
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
