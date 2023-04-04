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

namespace BadEcho.Extensibility.Hosting;

/// <summary>
/// Defines an adapter that is responsible for establishing connections between a host and several routable
/// plugin adapters that segment a common contract.
/// </summary>
public interface IHostAdapter
{
    /// <summary>
    /// Routes this call to the appropriate call-routable plugin contract implementation.
    /// </summary>
    /// <param name="methodName">The name of the contract method to execute.</param>
    /// <returns>
    /// The particular contract implementation that has been registered to execute the method specified by <c>methodName</c>.
    /// </returns>
    object Route(string methodName);
}