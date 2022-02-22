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

using System.Composition.Convention;

namespace BadEcho.Extensibility.Hosting;

/// <summary>
/// Defines a provider of rule configurations used to define objects as Odin's Extensibility framework parts.
/// </summary>
public interface IConventionProvider
{
    /// <summary>
    /// Configures the provided convention builder with rules specified by the provider.
    /// </summary>
    /// <param name="conventions">A convention builder to configure with the provider's rules.</param>
    void ConfigureRules(ConventionBuilder conventions);
}