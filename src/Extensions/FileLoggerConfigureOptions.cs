// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace BadEcho.Extensions;

/// <summary>
/// Provides configuration for a file logger.
/// </summary>
/// <remarks>
/// <para>
/// A <see cref="IConfigureOptions{TOptions}"/> implementation is provided for this logger so all of its configuration
/// parameters, be they standard or custom, can be organized in the same manner as the built-in loggers, like the following:
/// <code>
/// {
///   "Logging": {
///     "File": {
///       "LogLevel": {
///         "Default": "Warning"
///       },
///       "Path": "customSetting.log"
///     }
///   }
/// }
/// </code>
/// </para>
/// <para>
/// The injected <see cref="ILoggerProviderConfiguration{T}"/> adds a configuration section for the alias of the
/// <see cref="ILoggerProvider"/>, allowing us to bind our options type to the properties defined there.
/// </para>
/// <para>Writing this down for posterity...and, truth be told, so I don't forget.</para>
/// </remarks>
internal sealed class FileLoggerConfigureOptions : IConfigureOptions<FileLoggerOptions>
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileLoggerConfigureOptions"/> class.
    /// </summary>
    public FileLoggerConfigureOptions(ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration)
    {
        _configuration = providerConfiguration.Configuration;
    }

    /// <inheritdoc/>
    public void Configure(FileLoggerOptions options)
        => _configuration.Bind(options);
}
