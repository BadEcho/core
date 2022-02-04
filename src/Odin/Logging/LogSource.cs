//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace BadEcho.Odin.Logging;

/// <summary>
/// Provides a source for diagnostic messages from Bad Echo technologies to external listeners.
/// </summary>
[EventSource(Name = EVENT_SOURCE_NAME)]
internal sealed class LogSource : EventSource
{
    private const string EVENT_SOURCE_NAME = "BadEcho-Odin";

    /// <summary>
    /// Initializes a new instance of the <see cref="LogSource"/> class.
    /// </summary>
    private LogSource()
        : base(EventSourceSettings.EtwSelfDescribingEventFormat)
    { }

    /// <summary>
    /// Gets the name of the <see cref="EventSource"/> registered for this type.
    /// </summary>
    public static string EventSourceName
        => EVENT_SOURCE_NAME;

    /// <summary>
    /// Gets the singular instance of <see cref="LogSource"/>.
    /// </summary>
    public static LogSource Instance
    { get; } = new();

    /// <summary>
    /// Writes a debug event, its details described by the provided message.
    /// </summary>
    /// <param name="message">A debug message describing the details of the event.</param>
    [Event(1, Level = EventLevel.Verbose, Keywords = Keywords.MessageKeywordValue)]
    public void Debug(string message)
    {
        if (!IsEnabled())
            return;

        WriteEvent(1, message);
    }

    /// <summary>
    /// Writes an informational event, its details described by the provided message.
    /// </summary>
    /// <param name="message">An informational message describing the details of the event.</param>
    [Event(2, Level = EventLevel.Informational, Keywords = Keywords.MessageKeywordValue)]
    public void Info(string message)
    {
        if (!IsEnabled())
            return;
            
        WriteEvent(2, message);
    }

    /// <summary>
    /// Writes a warning event, its details described by the provided message.
    /// </summary>
    /// <param name="message">A warning message describing the details of the event.</param>
    [Event(3, Level = EventLevel.Warning, Keywords = Keywords.MessageKeywordValue)]
    public void Warning(string message)
    {
        if (!IsEnabled())
            return;
            
        WriteEvent(3, message);
    }

    /// <summary>
    /// Writes an error event, its details described by the provided message.
    /// </summary>
    /// <param name="message">An error message describing the details of the event.</param>
    [Event(4, Level = EventLevel.Error, Keywords = Keywords.MessageKeywordValue)]
    public void Error(string message)
    {
        if (!IsEnabled())
            return;
            
        WriteEvent(4, message);
    }

    /// <summary>
    /// Writes a critical error event, its details described by the provided message.
    /// </summary>
    /// <param name="message">A critical error message describing the details of the event.</param>
    [Event(5, Level = EventLevel.Critical, Keywords = Keywords.MessageKeywordValue)]
    public void Critical(string message)
    {
        if (!IsEnabled())
            return;

        WriteEvent(5, message);
    }

    /// <summary>
    /// Writes an error event stemming from an exception, its details described by the provided message and said exception.
    /// </summary>
    /// <param name="message">An error message describing the details of the event.</param>
    /// <param name="exception">The exception that inspired the writing of this event.</param>
    [NonEvent]
    public void Error(string? message, Exception exception)
    {
        if (!IsEnabled())
            return;

        ErrorException(message,
                       exception.GetType().FullName ?? string.Empty,
                       exception.Message,
                       exception.HResult,
                       exception.ToString());
    }

    /// <summary>
    /// Writes a critical error event stemming from an exception, its details described by the provided message and said
    /// exception.
    /// </summary>
    /// <param name="message">A critical error message describing the details of the event.</param>
    /// <param name="exception">The exception that inspired the writing of this event.</param>
    [NonEvent]
    public void Critical(string? message, Exception exception)
    {
        if (!IsEnabled())
            return;

        CriticalException(message,
                          exception.GetType().FullName ?? string.Empty,
                          exception.Message,
                          exception.HResult,
                          exception.ToString());
    }

    [Event(6, Level = EventLevel.Error, Keywords = Keywords.MessageKeywordValue | Keywords.ExceptionKeywordValue)]
    private unsafe void ErrorException(string? message,
                                       string exceptionType,
                                       string exceptionMessage,
                                       int exceptionHResult,
                                       string exceptionVerboseMessage)
    {
        fixed (char* pMessage = message)
        fixed (char* pExceptionType = exceptionType)
        fixed (char* pExceptionMessage = exceptionMessage)
        fixed (char* pExceptionVerboseMessage = exceptionVerboseMessage)
        {
            const int eventDataCount = 5;
            EventData* eventData = stackalloc EventData[eventDataCount];

            SetEventData(ref eventData[0], ref message, pMessage);
            SetEventData(ref eventData[1], ref exceptionType, pExceptionType);
            SetEventData(ref eventData[2], ref exceptionMessage, pExceptionMessage);
            SetEventData(ref eventData[3], ref exceptionHResult);
            SetEventData(ref eventData[4], ref exceptionVerboseMessage, pExceptionVerboseMessage);

            WriteEventCore(6, eventDataCount, eventData);
        }
    }

    [Event(7, Level = EventLevel.Critical, Keywords = Keywords.MessageKeywordValue | Keywords.ExceptionKeywordValue)]
    private unsafe void CriticalException(string? message,
                                          string exceptionType,
                                          string exceptionMessage,
                                          int exceptionHResult,
                                          string exceptionVerboseMessage)
    {
        fixed (char* pMessage = message)
        fixed (char* pExceptionType = exceptionType)
        fixed (char* pExceptionMessage = exceptionMessage)
        fixed (char* pExceptionVerboseMessage = exceptionVerboseMessage)
        {
            const int eventDataCount = 5;
            EventData* eventData = stackalloc EventData[eventDataCount];

            SetEventData(ref eventData[0], ref message, pMessage);
            SetEventData(ref eventData[1], ref exceptionType, pExceptionType);
            SetEventData(ref eventData[2], ref exceptionMessage, pExceptionMessage);
            SetEventData(ref eventData[3], ref exceptionHResult);
            SetEventData(ref eventData[4], ref exceptionVerboseMessage, pExceptionVerboseMessage);

            WriteEventCore(7, eventDataCount, eventData);
        }
    }

    [NonEvent]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void SetEventData<T>(ref EventData eventData, ref T value, void* pinnedString = null)
    {
        if (typeof(T) == typeof(string) && Unsafe.SizeOf<T>() == Unsafe.SizeOf<string>())
        {
            var stringValue = Unsafe.As<T, string>(ref value);

            eventData.DataPointer = (IntPtr) pinnedString;
            eventData.Size = checked((stringValue.Length + 1) * sizeof(char));
        }
        else
        {
            eventData.DataPointer = (IntPtr) Unsafe.AsPointer(ref value);
            eventData.Size = Unsafe.SizeOf<T>();
        }
    }
}