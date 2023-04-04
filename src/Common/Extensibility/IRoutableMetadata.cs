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

namespace BadEcho.Extensibility;

/// <summary>
/// Defines metadata that describes a call-routable plugin to the Bad Echo Extensibility framework.
/// </summary>
public interface IRoutableMetadata
{
    /// <summary>
    /// Gets the identity of the call-routable plugin being exported.
    /// </summary>
    Guid PluginId { get; }
}