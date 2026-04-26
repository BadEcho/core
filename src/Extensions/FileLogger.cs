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

namespace BadEcho.Extensions;

/// <summary>
/// Provides an incredibly simple logger that writes to a file.
/// </summary>
/// <remarks>
/// <para>
/// Full-featured enough to avoid the usual concurrency issues with file loggers, but there is no custom formatting,
/// log rotation, high-performance optimizations, etc. A simple file logger, good enough to use for troubleshooting
/// deployments when the need arises.
/// </para>
/// <para>
/// This logger can be configured the same way as one would configure any of the built-in loggers, with additional
/// properties available in <see cref="FileLoggerOptions"/>.
/// </para>
/// </remarks>
internal sealed class FileLogger : ILogger
{
    /// <summary>
    /// Write operations to a log file are limited to a single thread at a time -- the simplest and most warranted solution
    /// for this logger's intended purpose. Synchronization locks are exclusive to each log file that's been configured for
    /// writing, which is a use case that should be rare in practice but can still occur.
    /// </summary>
    private static readonly ConcurrentDictionary<string, Lock> _NamedLocks
        = new();

    private readonly IHostEnvironment? _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileLogger"/> class.
    /// </summary>
    public FileLogger(FileLoggerOptions options, IExternalScopeProvider? scopeProvider, IHostEnvironment? environment)
    {
        Options = options;
        ScopeProvider = scopeProvider;

        _environment = environment;
    }

    /// <summary>
    /// Gets or sets the configuration for 
    /// </summary>
    public FileLoggerOptions Options
    { get; set; }

    /// <summary>
    /// Gets or sets the scope data storage.
    /// </summary>
    public IExternalScopeProvider? ScopeProvider
    { get; set; }

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel)
        => logLevel != LogLevel.None;

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        Require.NotNull(formatter, nameof(formatter));

        string message = formatter(state, exception);

        if (string.IsNullOrEmpty(message) && exception == null)
            return;

        string path = _environment.ResolvePath(Options.Path);

        lock (_NamedLocks.GetOrAdd(path, _ => new Lock()))
        {
            using var logFile = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            using (var logWriter = new StreamWriter(logFile))
            {
                logWriter.WriteLine($"{DateTime.Now:s} - {logLevel , -11} - {IndentNewLines(message)}");

                if (exception != null)
                    logWriter.WriteLine($"\t{IndentNewLines(exception.ToString())}");
            }
        }

        static string IndentNewLines(string message)
        {
            return message.Replace(Environment.NewLine, $"{Environment.NewLine}\t", StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => ScopeProvider?.Push(state);
}
