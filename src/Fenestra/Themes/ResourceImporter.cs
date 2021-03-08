//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using BadEcho.Odin;
using BadEcho.Odin.Extensibility.Hosting;

namespace BadEcho.Fenestra.Themes
{
    /// <summary>
    /// Provides a manager and importer of resources belonging to Fenestra applications and plugins.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For both performance and convenience related reasons, merging dictionaries at a user control level should be avoided.
    /// Instead, all resources should be merged a single time at the <see cref="Application"/> level. Doing so makes all of these
    /// resources available to all controls present within the assembly for linking to as static resources.
    /// </para>
    /// <para>
    /// Naturally, this becomes difficult when operating within the context of a library (as the Fenestra framework itself does),
    /// as no notion of an <see cref="Application"/> definition exists within a library. The difficulty increases doubly so when
    /// considering plugins that export graphical elements to the Fenestra application. Not only do these plugins lack an
    /// <see cref="Application"/> definition, but they typically also lack an reference from the application hosting them. This
    /// means the host application has no way of accessing the resources if it wanted to explicitly merge them into its own
    /// application-wide resource dictionary itself (which is a rather onerous and ridiculous requirement anyway that would only
    /// tightly couple the host to the plugin).
    /// </para>
    /// <para>
    /// The <see cref="ResourceImporter"/> provides us with the means to get all of a library's resources merged into an
    /// application-wide resource dictionary in a simple manner. All that is required by the library to engage in this process is
    /// to export a <see cref="IResourceProvider"/> implementation that points to a root resource dictionary pointing to all other
    /// resources required by the assembly.
    /// </para>
    /// </remarks>
    internal static class ResourceImporter
    {
        /// <summary>
        /// Loads all Fenestra-based resources into the provided <see cref="Application"/> instance.
        /// </summary>
        /// <param name="application">The <see cref="Application"/> instance to load resources into.</param>
        public static void Load(Application application)
        {
            Require.NotNull(application, nameof(application));

            var resourceProviders = PluginHost.Load<IResourceProvider>();


        }
    }
}