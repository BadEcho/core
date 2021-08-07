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

using System.Composition.Convention;
using System.Composition.Hosting;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Provides a set of static methods intended to aid in matters related to plugin context strategy initialization.
    /// </summary>
    internal static class PluginContextStrategyExtensions
    {
        /// <summary>
        /// Loads conventions based on Extensibility framework's stock rules as well as discovered <see cref="IConventionProvider"/>
        /// parts using the provided container configuration.
        /// </summary>
        /// <param name="strategy">The current plugin context strategy loading the conventions.</param>
        /// <param name="configuration">The container configuration to load the rule providers from.</param>
        /// <returns>The resulting <see cref="ConventionBuilder"/> loaded with all provided rules.</returns>
        public static ConventionBuilder LoadConventions(this IPluginContextStrategy strategy, ContainerConfiguration configuration)
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

            return conventions;
        }
    }
}