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

namespace BadEcho.Configuration;

/// <summary>
/// Provides a format-neutral source of hot-pluggable configuration data for an application.
/// </summary>
public abstract class ConfigurationProvider : IConfigurationProvider
{
    /// <inheritdoc/>
    public event EventHandler<EventArgs>? ConfigurationChanged;

    /// <inheritdoc/>
    public T GetConfiguration<T>() where T : new()
        => GetConfiguration<T>(null);

    /// <inheritdoc/>
    public abstract T GetConfiguration<T>(string? sectionName) where T : new();

    /// <summary>
    /// Called when the configuration data has been externally updated, raising the <see cref="ConfigurationChanged"/> event.
    /// </summary>
    protected virtual void OnConfigurationChanged()
        => ConfigurationChanged?.Invoke(this, EventArgs.Empty);
}