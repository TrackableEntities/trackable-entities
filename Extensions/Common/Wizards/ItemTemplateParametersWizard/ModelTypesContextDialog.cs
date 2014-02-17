using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ItemTemplateParametersWizard
{
    public partial class ModelTypesContextDialog : IModelTypes
    {
        private readonly List<Type> _modelTypes;
        private readonly string _dialogTitle;
        private readonly string _dialogMessage;

        public ModelTypesContextDialog(List<Type> modelTypes, 
            string dialogTitle, string dialogMessage)
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            _modelTypes = modelTypes;
            _dialogTitle = dialogTitle;
            _dialogMessage = dialogMessage;
        }

        public ModelTypesInfo ModelTypesInfo { get; private set; }

        private void okButton_Click(object sender, EventArgs e)
        {
            ModelTypesInfo = ModelTypesHelper.GetModelTypesInfo(entitySetTextBox,
                entityNameComboBox, dbContextComboBox);
            if (ModelTypesInfo == null) return;
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
                descriptionLabel, entityNameComboBox, dbContextComboBox);
            if (!setupValid)
            {
                DialogResult = DialogResult.Cancel;
            }
        }
    }
}
