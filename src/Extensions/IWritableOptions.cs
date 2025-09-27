// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Options;

namespace BadEcho.Extensions;

/// <summary>
/// Defines a provider of configured <typeparamref name="TOptions"/> instances that allow for changes to be persisted.
/// </summary>
/// <typeparam name="TOptions">The type of options being requested.</typeparam>
public interface IWritableOptions<out TOptions> : IOptionsMonitor<TOptions>
    where TOptions : class
{
    /// <summary>
    /// Persists changes made to the (optionally named) <typeparamref name="TOptions"/> instance.
    /// </summary>
    /// <param name="name">
    /// The name of the <typeparamref name="TOptions"/> instance to persist. If null, the default instance is persisted.
    /// </param>
    void Save(string? name);
}
