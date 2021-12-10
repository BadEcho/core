//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Hosting;
using System.Reflection;
using System.Runtime.Loader;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Logging;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Extensibility.Hosting;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to general composition using the Managed
/// Extensibility Framework.
/// </summary>
public static class CompositionExtensions
{
    private static readonly Lazy<IEnumerable<Assembly>> _ExtensibilityPointSnapshot
        = new(CaptureExtensibilityPoints, LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Gets all assemblies marked as extensibility points that were loaded at the time of the first Extensibility
    /// operation.
    /// </summary>
    internal static IEnumerable<Assembly> ExtensibilityPointSnapshot
        => _ExtensibilityPointSnapshot.Value;

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
    /// Adds part types from assemblies loaded into the current context that are marked as extensibility points.
    /// </summary>
    /// <param name="configuration">The current container configuration.</param>
    /// <returns>An object that can be used to further configure the container.</returns>
    public static ContainerConfiguration WithExtensibilityPoints(this ContainerConfiguration configuration)
    {
        Require.NotNull(configuration, nameof(configuration));

        return configuration.WithAssemblies(ExtensibilityPointSnapshot);
    }

    /// <summary>
    /// Loads all assemblies found in the specified directory for the purposes of exported part discovery.
    /// </summary>
    /// <param name="configuration">The current container configuration.</param>
    /// <param name="path">The directory containing the assemblies to load.</param>
    /// <returns>A collection of <see cref="Assembly"/> objects found at <c>path</c>.</returns>
    internal static IEnumerable<Assembly> LoadFromDirectory(this ContainerConfiguration configuration, string path)
    {
        Require.NotNull(configuration, nameof(configuration));

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
        
    private static IEnumerable<Assembly> CaptureExtensibilityPoints()
    {
        return AssemblyLoadContext.Default.Assemblies
                                  .Where(assembly => assembly.GetCustomAttribute<ExtensibilityPointAttribute>() != null)
                                  .ToList();
    }
}