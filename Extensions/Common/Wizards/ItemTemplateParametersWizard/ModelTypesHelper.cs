using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using TrackableEntities;

namespace ItemTemplateParametersWizard
{
    internal static class ModelTypesHelper
    {
        public static bool SetupUserInterface(List<ModelTypeInfo> modelTypes,
            string dialogTitle, string dialogMessage, Form dialog,
            Label descLabel, ComboBox entityComboBox, ComboBox contextComboBox)
        {
            // Set dialog title and message
            dialog.Text = dialogTitle;
            descLabel.Text = dialogMessage;

            // Get trackable and context types
            List<ModelTypeInfo> entityTypes = FilterModelTypes
                (modelTypes, ModelType.Trackable);

            // Populate entity combo
            entityComboBox.DataSource = entityTypes;

            // Get dbContext types
            if (contextComboBox != null)
            {
                List<ModelTypeInfo> contextTypes = FilterModelTypes
                    (modelTypes, ModelType.DbContext);
                contextComboBox.DataSource = contextTypes;
            }
            return true;
        }

        public static ModelTypesDialogInfo GetModelTypesInfo(TextBox entitySetTextBox,
            ComboBox entityComboBox, ComboBox contextComboBox)
        {
            // Validate info
            if (!ValidateRequiredInfo(entitySetTextBox, entityComboBox,
                contextComboBox)) return null;

            // Get info
            string entitiesNamespace = ((ModelTypeInfo) entityComboBox.SelectedItem).Namespace;
            string baseNamespace = entitiesNamespace;
            int namespaceEnd = entitiesNamespace.IndexOf(".Entities.Models", StringComparison.OrdinalIgnoreCase);
            if (namespaceEnd < 0) namespaceEnd = entitiesNamespace.IndexOf(".Entities", StringComparison.OrdinalIgnoreCase);
            if (namespaceEnd >= 0) baseNamespace = entitiesNamespace.Substring(0, namespaceEnd);
            string entityName = ((ModelTypeInfo)entityComboBox.SelectedItem).Name;
            string entitySetName = entitySetTextBox.Text;
            string dbContextName = null;
            if (contextComboBox != null)
            {
                dbContextName = ((ModelTypeInfo) contextComboBox.SelectedItem).Name;
            }
            var info = new ModelTypesDialogInfo
            {
                EntityName = entityName,
                EntitySetName = entitySetName,
                DbContextName = dbContextName,
                BaseNamespace = baseNamespace,
                EntitiesNamespace = entitiesNamespace,
            };
            return info;
        }

        private static bool ValidateRequiredInfo(TextBox entitySetTextBox,
            ComboBox entityComboBox, ComboBox contextComboBox)
        {
            if (entityComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select an Entity Name", "Entity Name");
                return false;
            }

            if (string.IsNullOrWhiteSpace(entitySetTextBox.Text))
            {
                MessageBox.Show("Enter an Entity Set Name", "Entity Set Name");
                return false;
            }

            if (contextComboBox != null && contextComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select an DbContext Name", "DbContext Name");
                return false;
            }
            return true;
        }

        private static List<ModelTypeInfo> FilterModelTypes
            (IEnumerable<ModelTypeInfo> modelTypes, ModelType modelType)
        {
            var trackableTypes = from t in modelTypes
                where t.ModelType == modelType
                select t;
            return trackableTypes.ToList();
        }
    }
}
