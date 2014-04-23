using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ItemTemplateParametersWizard
{
    public partial class ModelTypesContextDialog : IModelTypes
    {
        private readonly List<ModelTypeInfo> _modelTypes;
        private readonly string _dialogTitle;
        private readonly string _dialogMessage;

        public ModelTypesContextDialog(List<ModelTypeInfo> modelTypes, 
            string dialogTitle, string dialogMessage, int dialogWidth)
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            _modelTypes = modelTypes;
            _dialogTitle = dialogTitle;
            _dialogMessage = dialogMessage;
            Width = dialogWidth;
        }

        public ModelTypesDialogInfo ModelTypesDialogInfo { get; private set; }

        private void okButton_Click(object sender, EventArgs e)
        {
            ModelTypesDialogInfo = ModelTypesHelper.GetModelTypesInfo(entitySetTextBox,
                entityNameComboBox, dbContextComboBox);
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
                descriptionLabel, entityNameComboBox, dbContextComboBox);
            if (!setupValid)
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        private void ModelTypesContextDialog_Resize(object sender, EventArgs e)
        {
            PositionControl(descriptionLabel);
            PositionControl(buttonsPanel);
        }

        private void PositionControl(Control control)
        {
            var curHeight = control.Location.Y;
            control.Location = new Point(
            Width / 2 - control.Size.Width / 2, curHeight);
        }
    }
}
