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
/// Defines a connection point between a call-routable plugin and a host, allowing for the plugin to effectively
/// segment a common contract it implements.
/// </summary>
/// <typeparam name="T">The type of contract that is segmented by the call-routable plugin.</typeparam>
public interface IPluginAdapter<out T>
{
    /// <summary>
    /// Gets the <typeparamref name="T"/> implemented by the call-routable plugin.
    /// </summary>
    T Contract { get; }
}