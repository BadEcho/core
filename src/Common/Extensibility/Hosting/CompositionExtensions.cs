//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Hosting;
using System.Reflection;
using System.Runtime.Loader;
using BadEcho.Logging;
using BadEcho.Extensions;
using BadEcho.Properties;

namespace BadEcho.Extensibility.Hosting;

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

    /// <summary>
    /// Discovers all assemblies that are marked as extensibility points.
    /// </summary>
    /// <returns>A collection of <see cref="Assembly"/> instances that are marked as extensibility points.</returns>
    /// <remarks>
    /// <para>
    /// While we normally should be able to safely inspect assemblies loaded into the default assembly context,
    /// some application hosts engage in probing behaviors that can easily result in unrelated assembly dependencies
    /// being loaded into said context.
    /// </para>
    /// <para>
    /// Attempting to inspect these unrelated assemblies may result in errors due to unreachable dependencies that stem 
    /// from the assemblies targeting a framework SDK different from what the main application assembly targets.
    /// </para>
    /// <para>
    /// An example of a host with this kind of behavior is Microsoft's default test runner "testhost.exe". It will load
    /// all assemblies that exist in the testing directory into the default context, even if there is no connection between
    /// them and the code actually being tested.
    /// </para>
    /// </remarks>
    private static IEnumerable<Assembly> CaptureExtensibilityPoints()
    {
        return AssemblyLoadContext.Default.Assemblies
                                  .Where(IsExtensible)
                                  .ToList();

        static bool IsExtensible(Assembly assembly)
        {
            try
            {
                return assembly.GetCustomAttribute<ExtensibilityPointAttribute>() != null;
            }
            catch (FileNotFoundException ex)
            {
                Logger.Debug(
                    Strings.ExtensibilityPointMissingDependency.InvariantFormat(assembly.FullName, ex.FileName));

                return false;
            }
        }
    }
}