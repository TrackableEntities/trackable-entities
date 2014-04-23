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

            int dialogType = 0;
            do
            {
                // Prompt for dialog type
                Console.WriteLine("\nSelect Dialog:\r\n\tWCF Service Type {1},\r\n\tEntity Controller {2}, " +
                    "\r\n\tEntity Repo Class {3},\r\n\tEntity Repo Interface {4}");
                if (!int.TryParse(Console.ReadLine(), out dialogType)) return;

                // Show dialog
                Form dialog = GetTypesDialog(modelTypes, dialogType);
                if (dialog == null)
                {
                    Console.WriteLine("Incorrect selection: {0}", dialogType);
                    continue;
                };
                if (dialog.ShowDialog() == DialogResult.Cancel) continue;

                // Print dialog info
                PrintDialogInfo((IModelTypes)dialog);
            } while (dialogType > 0);
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

        private static void PrintDialogInfo(IModelTypes dialog)
        {
            ModelTypesDialogInfo info = dialog.ModelTypesDialogInfo;
            Console.WriteLine("BaseNamespace: {0}", info.BaseNamespace);
            Console.WriteLine("EntityName: {0}", info.EntityName);
            Console.WriteLine("EntitySetName: {0}", info.EntitySetName);
            Console.WriteLine("DbContextName: {0}", info.DbContextName);
        }

        private static Form GetTypesDialog(List<ModelTypeInfo> modelTypes, int dialogType)
        {
            switch (dialogType)
            {
                case 1:
                    return new ModelTypesContextDialog(modelTypes,
                        Dialogs.WcfServiceType.Title,
                        Dialogs.WcfServiceType.Message,
                        Dialogs.WcfServiceType.DialogWidth);
                case 2:
                    return new ModelTypesDialog(modelTypes,
                        Dialogs.EntityController.Title,
                        Dialogs.EntityController.Message,
                        Dialogs.EntityController.DialogWidth);
                case 3:
                    return new ModelTypesDialog(modelTypes,
                        Dialogs.EntityRepoClass.Title,
                        Dialogs.EntityRepoClass.Message,
                        Dialogs.EntityRepoClass.DialogWidth);
                case 4:
                    return new ModelTypesDialog(modelTypes,
                        Dialogs.EntityRepoInterface.Title,
                        Dialogs.EntityRepoInterface.Message,
                        Dialogs.EntityRepoInterface.DialogWidth);
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
