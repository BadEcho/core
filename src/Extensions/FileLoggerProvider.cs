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

using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BadEcho.Extensions;

/// <summary>
/// A provider of <see cref="FileLogger"/> instances.
/// </summary>
[ProviderAlias("File")]
internal sealed class FileLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = new();
    private readonly IOptionsMonitor<FileLoggerOptions> _options;
    private readonly IHostEnvironment? _environment;
    private readonly IDisposable? _optionsReloadToken;

    private IExternalScopeProvider? _scopeProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileLoggerProvider"/> class.
    /// </summary>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options)
    {
        _options = options;

        _optionsReloadToken = _options.OnChange(ReloadOptions);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileLoggerProvider"/> class.
    /// </summary>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options, IHostEnvironment environment)
        : this(options)
    {
        _environment = environment;
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, _ => new FileLogger(_options.CurrentValue, _scopeProvider, _environment));
    }

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;

        foreach (var logger in _loggers)
        {
            logger.Value.ScopeProvider = scopeProvider;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _optionsReloadToken?.Dispose();

        _disposed = true;
    }

    private void ReloadOptions(FileLoggerOptions options)
    {
        foreach (var logger in _loggers)
        {
            logger.Value.Options = options;
        }
    }
}
