//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using BadEcho.Fenestra.Properties;
using BadEcho.Extensibility.Hosting;

namespace BadEcho.Fenestra.Extensions;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to WPF <see cref="Application"/> objects.
/// </summary>
public static class ApplicationExtensions
{
    /// <summary>
    /// Imports and loads all Fenestra-based resources into this <see cref="Application"/> instance.
    /// </summary>
    /// <param name="application">The current WPF application object to import resources into.</param>
    /// <remarks>
    /// <para>
    /// For both performance and convenience related reasons, merging dictionaries at a user control level should be avoided.
    /// Instead, all resources should be merged a single time at the <see cref="Application"/> level. Doing so makes all of these
    /// resources available to all controls present within the assembly for linking to as static resources.
    /// </para>
    /// <para>
    /// Naturally, this becomes difficult when operating within the context of a library (as the Fenestra framework itself does),
    /// as no notion of an <see cref="Application"/> definition exists within a library. The difficulty increases doubly so when
    /// considering plugins that export graphical elements to the WPF application. Not only do these plugins lack an
    /// <see cref="Application"/> definition, but they typically also lack an reference from the application hosting them. This
    /// means the host application has no way of accessing the resources if it wanted to explicitly merge them into its own
    /// application-scope resource dictionary itself (which is a rather onerous and ridiculous requirement anyway that would only
    /// tightly couple the host to the plugin).
    /// </para>
    /// <para>
    /// This method provides us with the means to get all of a library's resources merged into an application-scoped resource
    /// dictionary in a simple manner. All that is required by the library to engage in this process is to export a
    /// <see cref="IResourceProvider"/> implementation that points to a root resource dictionary pointing to all other resources
    /// required by the assembly.
    /// </para>
    /// </remarks>
    public static void ImportResources(this Application application)
    {
        Require.NotNull(application, nameof(application));

        var resourceProviders = PluginHost.Load<IResourceProvider>();

        foreach (var resourceProvider in resourceProviders)
        {
            object component = Application.LoadComponent(resourceProvider.ResourceUri);

            if (component is not ResourceDictionary componentDictionary)
                throw new InvalidOperationException(Strings.ResourceUriNotResourceDictionary);

            application.Resources.MergedDictionaries.Add(componentDictionary);
        }
    }
}