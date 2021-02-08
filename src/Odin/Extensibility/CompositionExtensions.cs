//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Logging;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides a set of static methods intended to aid in matters related to general composition using the Managed
    /// Extensibility Framework.
    /// </summary>
    public static class CompositionExtensions
    {
        /// <summary>
        /// Adds part types from the assemblies found in the specified directory to this container configuration.
        /// </summary>
        /// <param name="configuration">The current container configuration.</param>
        /// <param name="path">The directory containing assemblies to add part types from.</param>
        /// <returns>An object that can be used to further configure the container.</returns>
        public static ContainerConfiguration WithDirectory(this ContainerConfiguration configuration, string path)
        {
            var assemblies = configuration.LoadFromDirectory(path);

            return configuration.WithAssemblies(assemblies);
        }


        /// <summary>
        /// Loads all assemblies found in the specified directory for the purposes of exported part discovery.
        /// </summary>
        /// <param name="configuration">The current container configuration.</param>
        /// <param name="path">The directory containing the assemblies to load.</param>
        /// <returns>A collection of <see cref="Assembly"/> objects found at <c>path</c>.</returns>
        internal static IEnumerable<Assembly> LoadFromDirectory([NotNull] this ContainerConfiguration configuration, string path)
        {
            if (configuration == null) 
                throw new ArgumentNullException(nameof(configuration));

            var assemblies = new List<Assembly>();
            string[] dllFiles = Directory.GetFiles(Path.GetFullPath(path), "*.dll");

            foreach (var dllFile in dllFiles)
            {
                try
                {
                    assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(dllFile));
                }
                catch (FileLoadException ex)
                {
                    var errorMessage = Strings.PluginFileLoadException.InvariantFormat(dllFile);
                    Logger.Error(errorMessage, ex);
                }
                catch (BadImageFormatException)
                {
                    var errorMessage = Strings.PluginBadImageException.InvariantFormat(dllFile);
                    Logger.Warning(errorMessage);
                }
            }

            return assemblies;
        }
    }
}