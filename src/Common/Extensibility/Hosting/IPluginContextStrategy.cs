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

using System.Composition.Hosting;

namespace BadEcho.Extensibility.Hosting;

/// <summary>
/// Defines an initialization strategy for a <see cref="PluginContext"/> which influences the manner in which plugins are loaded
/// through the Bad Echo Extensibility framework.
/// </summary>
internal interface IPluginContextStrategy
{
    /// <summary>
    /// Creates the <see cref="CompositionHost"/> used by the <see cref="PluginContext"/> to compose its objects.
    /// </summary>
    /// <returns>
    /// A <see cref="CompositionHost"/> instance configured to compose objects as specified by the
    /// <see cref="IPluginContextStrategy"/> implementation.
    /// </returns>
    CompositionHost CreateContainer();
}