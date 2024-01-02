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