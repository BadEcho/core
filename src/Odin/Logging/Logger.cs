//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace BadEcho.Odin.Logging
{
    /// <summary>
    /// Provides the main entry point into Odin's Logging framework, delivering a means for Bad Echo technologies to pass diagnostic
    /// messages to consuming applications.
    /// </summary>
    public static class Logger
    {
        private static readonly DefaultEventListener _Listener;

        /// <summary>
        /// Initializes static members of the <see cref="Logger"/> class.
        /// </summary>
        static Logger() 
            => _Listener = new DefaultEventListener();

        private static LogSource Source
            => LogSource.Instance;

        /// <summary>
        /// Logs the provided message as a debug statement.
        /// </summary>
        /// <param name="message">The debug message to log.</param>
        public static void Debug(string message) 
            => Source.Debug(message);

        /// <summary>
        /// Logs the provided message as general information.
        /// </summary>
        /// <param name="message">The informational message to log.</param>
        public static void Info(string message) 
            => Source.Info(message);

        /// <summary>
        /// Logs the provided message as a warning.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public static void Warning(string message) 
            => Source.Warning(message);

        /// <summary>
        /// Logs the provided message as an error.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void Error(string message) 
            => Source.Error(message);

        /// <summary>
        /// Logs an error message regarding an unexpected exception.
        /// </summary>
        /// <param name="message">The error message to log in addition to the provided exception's details.</param>
        /// <param name="exception">The particular exception that inspired you to log it.</param>
        public static void Error(string? message, Exception exception)
        {
            Require.NotNull(exception, nameof(exception));

            Source.Error(message, exception);
        }

        /// <summary>
        /// Logs the provided message as a critical error.
        /// </summary>
        /// <param name="message">The critical error message to log.</param>
        public static void Critical(string message) 
            => Source.Critical(message);

        /// <summary>
        /// Logs a critical error message regarding an unexpected exception.
        /// </summary>
        /// <param name="message">The critical error message to log in addition to the provided exception's details.</param>
        /// <param name="exception">The particular exception that inspired you to log it.</param>
        public static void Critical(string? message, Exception exception)
        {
            Require.NotNull(exception, nameof(exception));

            Source.Critical(message, exception);
        }

        /// <summary>
        /// Disables the default event listener created when this logger is used.
        /// </summary>
        public static void DisableDefaultListener() 
            => _Listener.DisableEvents(Source);
    }
}
