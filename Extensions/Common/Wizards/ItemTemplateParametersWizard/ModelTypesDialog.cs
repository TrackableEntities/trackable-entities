using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ItemTemplateParametersWizard
{
    public partial class ModelTypesDialog : Form, IModelTypes
    {
        private readonly List<ModelTypeInfo> _modelTypes;
        private readonly string _dialogTitle;
        private readonly string _dialogMessage;

        public ModelTypesDialog(List<ModelTypeInfo> modelTypes, 
            string dialogTitle, string dialogMessage)
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            _modelTypes = modelTypes;
            _dialogTitle = dialogTitle;
            _dialogMessage = dialogMessage;
        }

        public ModelTypesDialogInfo ModelTypesDialogInfo { get; private set; }

        private void okButton_Click(object sender, EventArgs e)
        {
            ModelTypesDialogInfo = ModelTypesHelper.GetModelTypesInfo(entitySetTextBox,
                entityNameComboBox, null);
            if (ModelTypesDialogInfo == null) return;
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void ModelTypesDialog_Load(object sender, EventArgs e)
        {
            bool setupValid = ModelTypesHelper.SetupUserInterface(
                _modelTypes, _dialogTitle, _dialogMessage, this,
                descriptionLabel, entityNameComboBox, null);
            if (!setupValid)
            {
                DialogResult = DialogResult.Cancel;
            }
        }
    }
}
