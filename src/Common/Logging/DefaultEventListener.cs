//-----------------------------------------------------------------------
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
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.Tracing;
using BadEcho.Properties;

namespace BadEcho.Logging;

/// <summary>
/// Provides the default logging behavior for Bad Echo's Logging framework, with output limited to an attached debugger.
/// </summary>
internal sealed class DefaultEventListener : EventListener
{
    /// <inheritdoc/>
    /// <remarks>Only events from <see cref="LogSource"/> are tracked by this listener.</remarks>
    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        base.OnEventSourceCreated(eventSource);
        
        if (eventSource.Name == LogSource.EventSourceName)
            EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All);
    }

    /// <inheritdoc/>
    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        base.OnEventWritten(eventData);

        // We must continue to check that the event source name corresponds to our own (BadEcho).
        // Even though our event source is the only one we explicitly enabled, this listener will have
        // been automatically attached to any event sources enabled at process start, all courtesy of the runtime.
        // One such event source is 'EventCounters', released with VS 17.8 and exceptionally noisy.
        if (eventData.EventSource.Name!= LogSource.EventSourceName)
            return;

        var outputMessage = $@"{eventData.TimeStamp:HH:mm} | {eventData.OSThreadId} | {eventData.Level}";

        outputMessage
            = $"{outputMessage} | {(eventData.Payload != null ? eventData.Payload[0] : Strings.LoggingMissingMessage)}";

        if (eventData.Keywords.HasFlag(Keywords.ExceptionKeyword))
        {
            outputMessage = $@"{outputMessage}{Environment.NewLine}    {
                (eventData.Payload != null ? eventData.Payload[4] : Strings.LoggingMissingException)}";
        }

        Debugger.Log(0, null, $"{outputMessage}{Environment.NewLine}");
    }
}