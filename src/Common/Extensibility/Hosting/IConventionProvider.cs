//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Convention;

namespace BadEcho.Extensibility.Hosting;

/// <summary>
/// Defines a provider of rule configurations used to define objects as parts belonging to the Bad Echo Extensibility framework.
/// </summary>
public interface IConventionProvider
{
    /// <summary>
    /// Configures the provided convention builder with rules specified by the provider.
    /// </summary>
    /// <param name="conventions">A convention builder to configure with the provider's rules.</param>
    void ConfigureRules(ConventionBuilder conventions);
}