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

using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace BadEcho.Logging;

/// <summary>
/// Provides an event listener which will log events written to a Bad Echo event source.
/// </summary>
public sealed class BadEchoEventListener : EventListener
{
    private readonly Action<EventWrittenEventArgs> _logEvent;

    /// <summary>
    /// Initializes a new instance of the <see cref="BadEchoEventListener"/> class.
    /// </summary>
    public BadEchoEventListener()
        : this(LogEvent)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadEchoEventListener"/> class.
    /// </summary>
    /// <param name="logEvent">The logging action to execute when an event is written.</param>
    public BadEchoEventListener(Action<EventWrittenEventArgs> logEvent)
    {
        Require.NotNull(logEvent, nameof(logEvent));

        _logEvent = logEvent;
    }

    /// <summary>
    /// Gets the name of the identifying trait used by Bad Echo event sources.
    /// </summary>
    public static string TraitName 
        => "BadEcho";

    /// <summary>
    /// Gets the value of the identifying trait used by Bad Echo event sources.
    /// </summary>
    /// <remarks>This only exists due to key-value pairs being required when defining traits.</remarks>
    public static string TraitValue 
        => "true";

    /// <inheritdoc/>
    /// <remarks>Only events from <see cref="BadEchoEventSource"/> are tracked by this listener.</remarks>
    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        base.OnEventSourceCreated(eventSource);

        Require.NotNull(eventSource, nameof(eventSource));

        if (eventSource.GetTrait(TraitName) == TraitValue)
            EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All);
    }

    /// <inheritdoc/>
    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        base.OnEventWritten(eventData);

        Require.NotNull(eventData, nameof(eventData));

        // Even though our own event sources are the only ones we explicitly enable upon their creation,
        // an event listener is attached to all existing event sources when said listener is created
        // (courtesy of the runtime), so we need to filter out these extraneous event sources.
        if (eventData.EventSource.GetTrait(TraitName) != TraitValue)
            return;

        _logEvent.Invoke(eventData);
    }

    private static void LogEvent(EventWrittenEventArgs eventData)
    {
        var outputMessage = EventDataFormatting.Format(eventData, true);

        Debugger.Log(0, null, $"{outputMessage}{Environment.NewLine}");
    }
}
