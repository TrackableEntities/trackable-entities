using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ItemTemplateParametersWizard;
using TrackableEntities.ItemWizard;

namespace TemplateWizardHarness
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Get model types
            List<ModelTypeInfo> modelTypes = GetModelTypes();

            // Prompt for dialog type
            Console.WriteLine("Select Dialog:\r\n\tWCF Service Type {1},\r\n\tEntity Controller {2}, " +
                "\r\n\tEntity Repo Class {3},\r\n\tEntity Repo Interface {4}");
            int dialogType;
            if (!int.TryParse(Console.ReadLine(), out dialogType)) return;

            // Show dialog
            Form dialog = GetTypesDialog(modelTypes, dialogType);
            if (dialog == null) return;
            if (dialog.ShowDialog() == DialogResult.Cancel)
            {
                Console.WriteLine("Operation cancelled");
                return;
            }

            // Print dialog info
            ModelTypesDialogInfo info = ((IModelTypes)dialog).ModelTypesDialogInfo;
            Console.WriteLine("BaseNamespace: {0}", info.BaseNamespace);
            Console.WriteLine("EntityName: {0}", info.EntityName);
            Console.WriteLine("EntitySetName: {0}", info.EntitySetName);
            Console.WriteLine("DbContextName: {0}", info.DbContextName);
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

        private static Form GetTypesDialog(List<ModelTypeInfo> modelTypes, int dialogType)
        {
            switch (dialogType)
            {
                case 1:
                    return new ModelTypesContextDialog(modelTypes,
                        Dialogs.WcfServiceType.Title,
                        Dialogs.WcfServiceType.Message);
                case 2:
                    return new ModelTypesDialog(modelTypes,
                        Dialogs.EntityController.Title,
                        Dialogs.EntityController.Message);
                case 3:
                    return new ModelTypesDialog(modelTypes,
                        Dialogs.EntityRepoClass.Title,
                        Dialogs.EntityRepoClass.Message);
                case 4:
                    return new ModelTypesDialog(modelTypes,
                        Dialogs.EntityRepoInterface.Title,
                        Dialogs.EntityRepoInterface.Message);
                default:
                    return null;
            }
        }

        static List<ModelTypeInfo> GetModelTypes()
        {
            Assembly assembly = Assembly.Load("TemplateWizard.Service.Entities");
            return ModelReflectionHelper.GetModelTypes(new FileInfo(assembly.Location));
        }
    }
}
