//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Convention;
using System.Composition.Hosting;

namespace BadEcho.Extensibility.Hosting;

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