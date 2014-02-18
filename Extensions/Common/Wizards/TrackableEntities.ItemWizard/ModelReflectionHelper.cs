using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Security.Policy;
using System.Reflection;
using ItemTemplateParametersWizard;

namespace TrackableEntities.ItemWizard
{
    public static class ModelReflectionHelper
    {
        public static List<ModelTypeInfo> GetModelTypes(FileInfo assemblyLocation)
        {
            var types = new List<ModelTypeInfo>();

            if (string.IsNullOrEmpty(assemblyLocation.Directory.FullName))
            {
                throw new InvalidOperationException("Directory can't be null or empty.");
            }

            if (!Directory.Exists(assemblyLocation.Directory.FullName))
            {
                throw new InvalidOperationException(
                   string.Format(CultureInfo.CurrentCulture,
                   "Directory not found {0}",
                   assemblyLocation.Directory.FullName));
            }

            AppDomain childDomain = BuildChildDomain(
                AppDomain.CurrentDomain);

            try
            {
                Type loaderType = typeof(AssemblyLoader);
                if (loaderType.Assembly != null)
                {
                    var loader =
                        (AssemblyLoader)childDomain.
                            CreateInstanceFrom(
                            loaderType.Assembly.Location,
                            loaderType.FullName).Unwrap();

                    loader.LoadAssembly(
                        assemblyLocation.FullName);
                    types =
                        loader.GetModelTypeInfo(
                        assemblyLocation.Directory.FullName);
                }
                return types;
            }

            finally
            {
                AppDomain.Unload(childDomain);
            }
        }

        private static AppDomain BuildChildDomain(AppDomain parentDomain)
        {
            var evidence = new Evidence(parentDomain.Evidence);
            AppDomainSetup setup = parentDomain.SetupInformation;
            return AppDomain.CreateDomain("DiscoveryRegion",
                evidence, setup);
        }

        private static bool InheritsFrom(this Type type, Type comparedType)
        {
            // Test for null
            if (type == null || comparedType == null) return false;

            // Test for interfaces
            if (comparedType.IsInterface)
            {
                return type.GetInterfaces().Any(t => t.FullName == comparedType.FullName);
            }

            // Test for base class
            return type.BaseType != null &&
                type.BaseType.FullName == comparedType.FullName;
        }

        class AssemblyLoader : MarshalByRefObject
        {
            internal List<ModelTypeInfo> GetModelTypeInfo(string path)
            {

                var types = new List<ModelTypeInfo>();

                var directory = new DirectoryInfo(path);
                ResolveEventHandler resolveEventHandler =
                    (s, e) =>
                    {
                        return OnReflectionOnlyResolve(
                            e, directory);
                    };

                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve
                    += resolveEventHandler;

                Assembly reflectionOnlyAssembly =
                    AppDomain.CurrentDomain.
                        ReflectionOnlyGetAssemblies().First();

                foreach(Type type in reflectionOnlyAssembly.GetTypes())
                {
                    var modelType = ModelType.Other;
                    if (type.InheritsFrom(typeof(ITrackable)))
                        modelType = ModelType.Trackable;
                    else if (type.InheritsFrom(typeof(DbContext)))
                        modelType = ModelType.DbContext;
                    types.Add(new ModelTypeInfo
                    {
                        Namespace = type.Namespace,
                        Name = type.Name, 
                        ModelType = modelType,
                        DisplayName = string.Format("{0} ({1})",
                            type.Name, type.Namespace)
                    });
                }

                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve
                    -= resolveEventHandler;
                return types;

            }

            private Assembly OnReflectionOnlyResolve(
                ResolveEventArgs args, DirectoryInfo directory)
            {

                Assembly loadedAssembly =
                    AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()
                        .FirstOrDefault(
                          asm => string.Equals(asm.FullName, args.Name,
                              StringComparison.OrdinalIgnoreCase));

                if (loadedAssembly != null)
                {
                    return loadedAssembly;
                }

                var assemblyName = new AssemblyName(args.Name);
                string dependentAssemblyFilename =
                    Path.Combine(directory.FullName,
                    assemblyName.Name + ".dll");

                if (File.Exists(dependentAssemblyFilename))
                {
                    return Assembly.ReflectionOnlyLoadFrom(
                        dependentAssemblyFilename);
                }
                return Assembly.ReflectionOnlyLoad(args.Name);
            }

            internal void LoadAssembly(String assemblyPath)
            {
                try
                {
                    Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                }
                catch (FileNotFoundException)
                {
                    /* Continue loading assemblies even if an assembly
                     * can not be loaded in the new AppDomain. */
                }
            }
        }
    }
}

