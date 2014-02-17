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
        public static bool SetupUserInterface(List<Type> modelTypes,
            string dialogTitle, string dialogMessage, Form dialog,
            Label descLabel, ComboBox entityComboBox, ComboBox contextComboBox)
        {
            // Set dialog title and message
            dialog.Text = dialogTitle;
            descLabel.Text = dialogMessage;

            // Get trackable and context types
            List<TypeInfo> entityTypes = FilterModelTypes
                (modelTypes, typeof(ITrackable));

            // Populate entity combo
            entityComboBox.DataSource = entityTypes;

            // Get dbContext types
            if (contextComboBox != null)
            {
                List<TypeInfo> contextTypes = FilterModelTypes
                        (modelTypes, typeof(DbContext));
                contextComboBox.DataSource = contextTypes;
            }
            return true;
        }

        public static ModelTypesInfo GetModelTypesInfo(TextBox entitySetTextBox,
            ComboBox entityComboBox, ComboBox contextComboBox)
        {
            // Validate info
            if (!ValidateRequiredInfo(entitySetTextBox, entityComboBox,
                contextComboBox)) return null;

            // Get info
            string entityNamespace = ((Type) entityComboBox.SelectedValue).Namespace;
            string baseNamespace = entityNamespace.Substring(0, entityNamespace.IndexOf(".Entities.Models"));
            string entityName = ((Type)entityComboBox.SelectedValue).Name;
            string entitySetName = entitySetTextBox.Text;
            string dbContextName = null;
            if (contextComboBox != null)
            {
                dbContextName = ((Type) contextComboBox.SelectedValue).Name;
            }
            var info = new ModelTypesInfo
            {
                BaseNamespace = baseNamespace,
                EntityName = entityName,
                EntitySetName = entitySetName,
                DbContextName = dbContextName
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

        private static List<TypeInfo> FilterModelTypes
            (IEnumerable<Type> modelTypes, Type canAssignTo)
        {
            var trackableTypes = from t in modelTypes
                where canAssignTo.IsAssignableFrom(t)
                select new TypeInfo
                {
                    DisplayName = string.Format("{0} ({1})",
                        t.Name, t.Namespace),
                    Type = t
                };
            return trackableTypes.ToList();
        }
    }
}
