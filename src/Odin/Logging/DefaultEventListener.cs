//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Logging
{
/// <summary>
/// Provides the default logging behavior for Odin's Logging framework, mainly limited to output to an attached debugger.
/// </summary>
internal sealed class DefaultEventListener : EventListener
{
    /// <inheritdoc/>
    /// <remarks>Only events from <see cref="LogSource"/> are tracked by this listener.</remarks>
    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        base.OnEventSourceCreated(eventSource);
        
        if (eventSource == LogSource.Instance)
            EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All);
    }

    /// <inheritdoc/>
    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        var outputMessage = $@"{eventData.TimeStamp:HH:mm} | {eventData.OSThreadId} | {eventData.Level}";
        
        outputMessage
            = $"{outputMessage} | {(eventData.Payload != null ? eventData.Payload[0] : Strings.LoggingMissingMessage)}";

        if (eventData.Keywords.HasFlag(Keywords.ExceptionKeyword))
        {
            outputMessage = $@"{outputMessage}{Environment.NewLine}    {
                (eventData.Payload != null ? eventData.Payload[4] : Strings.LoggingMissingException)}";
        }

        Debugger.Log(0, null, $"{outputMessage}{Environment.NewLine}");
        
        base.OnEventWritten(eventData);
    }
}
}
