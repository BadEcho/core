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

namespace BadEcho.Extensibility.Configuration;

/// <summary>
/// Defines configuration settings for a contract being segmented by one or more call-routable plugins.
/// </summary>
public interface IContractConfiguration
{
    /// <summary>
    /// Gets the type name of the contract being segmented.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the collection of call-routable plugins that segment the represented contract.
    /// </summary>
    IEnumerable<IRoutablePluginConfiguration> RoutablePlugins { get; }
}