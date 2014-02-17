using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ItemTemplateParametersWizard;

namespace TemplateWizardHarness
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            List<Type> modelTypes = GetModelTypes();
            var dialog = new ModelTypesDialog(modelTypes,
                "Trackable WCF Service Type",
                "Add WCF service contact and type with CRUD operations using Trackable Entities.");
            if (dialog.ShowDialog() == DialogResult.Cancel)
            {
                Console.WriteLine("Operation cancelled");
                return;
            }

            ModelTypesInfo info = dialog.ModelTypesInfo;
            Console.WriteLine("BaseNamespace: {0}", info.BaseNamespace);
            Console.WriteLine("EntityName: {0}", info.EntityName);
            Console.WriteLine("EntitySetName: {0}", info.EntitySetName);
            Console.WriteLine("DbContextName: {0}", info.DbContextName);
        }

        static List<Type> GetModelTypes()
        {
            Assembly assembly = Assembly.Load("TemplateWizard.Service.Entities");
            return assembly.GetTypes().ToList();
        }
    }
}
