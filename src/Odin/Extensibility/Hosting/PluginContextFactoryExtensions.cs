//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Convention;
using System.Composition.Hosting;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Provides a set of static methods intended to aid in matters related to plugin context factory initialization.
    /// </summary>
    internal static class PluginContextFactoryExtensions
    {
        /// <summary>
        /// Loads all rules provided by discovered <see cref="IConventionProvider"/> parts into a container configuration as its default
        /// conventions.
        /// </summary>
        /// <param name="factory">The current plugin context factory loading the conventions.</param>
        /// <param name="configuration">The container configuration to load the conventions into.</param>
        public static void LoadConventions(this IPluginContextFactory factory, ContainerConfiguration configuration)
        {
            var conventions = new ConventionBuilder();
            
            using (var container = configuration.CreateContainer())
            {
                var conventionProviders = container.GetExports<IConventionProvider>();
                
                foreach (var conventionProvider in conventionProviders)
                {
                    conventionProvider.ConfigureRules(conventions);
                }
            }

            configuration.WithDefaultConventions(conventions);
        }
    }
}
