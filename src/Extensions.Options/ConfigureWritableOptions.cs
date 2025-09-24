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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BadEcho.Extensions.Options;

/// <summary>
/// Provides configuration for a writable <typeparamref name="TOptions"/> type.
/// </summary>
/// <typeparam name="TOptions">The type of options being configured.</typeparam>
public sealed class ConfigureWritableOptions<TOptions> : ConfigureNamedOptions<TOptions>
    where TOptions : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureWritableOptions{TOptions}"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfigurationSection"/> instance to the source the options from.</param>
    /// <param name="fileName">The name of the file to write to when changes are saved.</param>
    public ConfigureWritableOptions(IConfigurationSection configuration, string fileName)
        : this(string.Empty, configuration, fileName)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureWritableOptions{TOptions}"/> class.
    /// </summary>
    /// <param name="name">The name of the options.</param>
    /// <param name="configuration">The <see cref="IConfigurationSection"/> instance to the source the options from.</param>
    /// <param name="fileName">The name of the file to write to when changes are saved.</param>
    public ConfigureWritableOptions(string name, IConfigurationSection configuration, string fileName) 
        : this(name, configuration as IConfiguration, fileName)
    {
        Require.NotNull(configuration, nameof(configuration));

        SectionName = configuration.Key;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureWritableOptions{TOptions}"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance to the source the options from.</param>
    /// <param name="fileName">The name of the file to write to when changes are saved.</param>
    public ConfigureWritableOptions(IConfiguration configuration, string fileName)
        : this(string.Empty, configuration, fileName)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureWritableOptions{TOptions}"/> class.
    /// </summary>
    /// <param name="name">The name of the options.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance to the source the options from.</param>
    /// <param name="fileName">The name of the file to write to when changes are saved.</param>
    public ConfigureWritableOptions(string name, IConfiguration configuration, string fileName)
        : base(name, configuration.Bind)
    {
        FileName = fileName;
    }

    /// <summary>
    /// Gets the name of the file to write to when changes are saved.
    /// </summary>
    public string FileName
    { get; }

    /// <summary>
    /// Gets the name of the configuration section the options are sourced from.
    /// </summary>
    public string SectionName
    { get; } = string.Empty;
}
