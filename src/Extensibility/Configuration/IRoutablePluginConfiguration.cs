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

namespace BadEcho.Extensibility.Configuration;

/// <summary>
/// Defines configuration settings for a call-routable plugin.
/// </summary>
public interface IRoutablePluginConfiguration
{
    /// <summary>
    /// Gets the identifying <see cref="Guid"/> for the call-routable plugin.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets a value indicating whether the represented plugin is the primary call-routable plugin
    /// for a particular context.
    /// </summary>
    bool Primary { get; }

    /// <summary>
    /// Gets the collection of names for methods claimed by the represented call-routable plugin.
    /// </summary>
    IEnumerable<string> MethodClaims { get; }
}