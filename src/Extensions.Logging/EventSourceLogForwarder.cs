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

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using BadEcho.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BadEcho.Extensions.Logging;

/// <summary>
/// Providers a forwarder of Bad Echo diagnostic events to configured logger providers.
/// </summary>
/// <suppressions>
/// Resharper disable NonStaticLoggerTemplate
/// </suppressions>
internal sealed class EventSourceLogForwarder : IHostedService, IDisposable
{
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new();
    private readonly ILoggerFactory _factory;
    
    private BadEchoEventListener? _listener;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSourceLogForwarder"/> class.
    /// </summary>
    public EventSourceLogForwarder(ILoggerFactory factory)
    {
        _factory = factory;
    }

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _listener = new BadEchoEventListener(LogEvent);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _listener?.Dispose();

        _disposed = true;
    }

    private static LogLevel EventToLogLevel(EventLevel level)
        => level switch
        {
            EventLevel.Critical => LogLevel.Critical,
            EventLevel.Error => LogLevel.Error,
            EventLevel.Informational => LogLevel.Information,
            EventLevel.LogAlways => LogLevel.Information,
            EventLevel.Verbose => LogLevel.Debug,
            EventLevel.Warning => LogLevel.Warning,
            _ => throw new InvalidEnumArgumentException(nameof(level), (int) level, typeof(EventLevel))
        };

    private void LogEvent(EventWrittenEventArgs eventData)
    {
        ILogger logger = _loggers.GetOrAdd(eventData.EventSource.Name,
                                           name => _factory.CreateLogger(name.Replace('-', '.')));

        string message = EventDataFormatting.Format(eventData, false);
        
        // It's not possible to make use of LoggerMessage delegates or static message templates here due to the forwarded messages
        // having a number of varying formats. Microsoft also does not use either of these in their log forwarders.
        logger.Log(EventToLogLevel(eventData.Level), new EventId(eventData.EventId, eventData.EventName), message);
    }
}