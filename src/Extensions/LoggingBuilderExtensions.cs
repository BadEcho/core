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

using System.Diagnostics;
using BadEcho.Extensions.Properties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace BadEcho.Extensions;

/// <summary>
/// Provides extension methods for configuring Bad Echo logging providers.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds a file logger to the builder.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> instance to add the logger to.</param>
    /// <returns>The current <see cref="ILoggingBuilder"/> instance so that additional loggers can be configured.</returns>
    /// <remarks>
    /// <para>
    /// This will configure the file logger based on the configuration found in the section for the provider (Logging:File).
    /// If it turns out that the configuration lacks an appropriate log file name, a default name of ProcessName.log will be used.
    /// </para>
    /// <para>
    /// We fall back to a default file name because the configuration isn't available at this point for us to validate against,
    /// and throwing an error after this method has already returned might be regarded as violation of the least astonishment
    /// principle.
    /// </para>
    /// </remarks>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
    {
        return builder.AddFile(o =>
        {
            if (string.IsNullOrWhiteSpace(o.Path))
                o.Path = $"{Process.GetCurrentProcess().ProcessName}.log";
        });
    }

    /// <summary>
    /// Adds a file logger to the builder.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> instance to add the logger to.</param>
    /// <param name="configure">A delegate to configure the <see cref="FileLogger"/>.</param>
    /// <returns>The current <see cref="ILoggingBuilder"/> instance so that additional loggers can be configured.</returns>
    /// <remarks>
    /// There is an expectation when calling this overload that, either through a configuration file or the <c>configure</c>
    /// delegate, a valid log file name path will be specified. If this turns out to not be the case, an error will be thrown
    /// during the post-configuration phase of the <see cref="FileLoggerOptions"/>.
    /// </remarks>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
    {
        Require.NotNull(builder, nameof(builder));

        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IConfigureOptions<FileLoggerOptions>, FileLoggerConfigureOptions>());

        builder.Services.Configure(configure);
        builder.Services.PostConfigure<FileLoggerOptions>(o =>
        {
            if (string.IsNullOrWhiteSpace(o.Path))
                throw new InvalidOperationException(Strings.LogFilePathNotSpecified);
        });

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IOptionsChangeTokenSource<FileLoggerOptions>, LoggerProviderOptionsChangeTokenSource<FileLoggerOptions, FileLoggerProvider>>());

        return builder;
    }
}
