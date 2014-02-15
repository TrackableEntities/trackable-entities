using System;
using System.Windows.Forms;

namespace TrackableEntities.PatternItemsWizard
{
    public partial class GetParamatersDialog : Form
    {
        public GetParamatersDialog(string title, string desc)
        {
            InitializeComponent();
            Text = title;
            descriptionLabel.Text = desc;
        }

        public string EntitiesNamespace { get; set; }
        public string EntityName { get; set; }
        public string EntitySetName { get; set; }
        public string DbContextName { get; set; }

        private void okButton_Click(object sender, EventArgs e)
        {
            EntitiesNamespace = namespaceTextBox.Text;
            EntityName = entityTextBox.Text;
            EntitySetName = entitySetTextBox.Text;
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
