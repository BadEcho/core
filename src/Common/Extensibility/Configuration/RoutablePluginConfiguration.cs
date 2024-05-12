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
/// Provides configuration settings for a call-routable plugin suitable for binding against with a
/// generic configuration provider (i.e. Microsoft's IConfiguration binder).
/// </summary>
public sealed class RoutablePluginConfiguration : IRoutablePluginConfiguration
{
    IEnumerable<string> IRoutablePluginConfiguration.MethodClaims
        => MethodClaims ?? [];

    /// <inheritdoc/>
    public Guid Id 
    { get; init; }

    /// <inheritdoc/>
    public bool Primary 
    { get; init; }

    /// <inheritdoc cref="IRoutablePluginConfiguration.MethodClaims"/>
    /// <remarks>
    /// This property exists in order to provide a configuration binder with a property that has no default value
    /// assigned to it (which causes issues with some binders) while maintaining the property's non-nullability
    /// contract found on the <see cref="IRoutablePluginConfiguration"/> interface.
    /// </remarks>
    public IEnumerable<string>? MethodClaims 
    { get; init; }
}