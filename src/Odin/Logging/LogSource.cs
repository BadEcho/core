//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace BadEcho.Odin.Logging
{
    /// <summary>
    /// Provides a source for diagnostic messages from Bad Echo technologies to external listeners.
    /// </summary>
    [EventSource(Name = "BadEcho-Odin")]
    internal sealed class LogSource : EventSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogSource"/> class.
        /// </summary>
        private LogSource()
            : base(EventSourceSettings.EtwSelfDescribingEventFormat)
        { }

        /// <summary>
        /// Gets the singular instance of <see cref="LogSource"/>.
        /// </summary>
        public static LogSource Instance
        { get; } = new();

        /// <summary>
        /// Writes a debug event, its details described by the provided message.
        /// </summary>
        /// <param name="message">A debug message describing the details of the event.</param>
        [Event(1, Level = EventLevel.Verbose, Keywords = Keywords.MESSAGE_KEYWORD)]
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
        [Event(2, Level = EventLevel.Informational, Keywords = Keywords.MESSAGE_KEYWORD)]
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
        [Event(3, Level = EventLevel.Warning, Keywords = Keywords.MESSAGE_KEYWORD)]
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
        [Event(4, Level = EventLevel.Error, Keywords = Keywords.MESSAGE_KEYWORD)]
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
        [Event(5, Level = EventLevel.Critical, Keywords = Keywords.MESSAGE_KEYWORD)]
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

        [Event(6, Level = EventLevel.Error, Keywords = Keywords.MESSAGE_KEYWORD | Keywords.EXCEPTION_KEYWORD)]
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

        [Event(7, Level = EventLevel.Critical, Keywords = Keywords.MESSAGE_KEYWORD | Keywords.EXCEPTION_KEYWORD)]
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
}
