//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.Tracing;

namespace BadEcho.Odin.Logging
{
    /// <summary>
    /// Provides keywords used for the categorization and filtering of Odin's Logging framework.
    /// </summary>
    public static class Keywords
    {
        /// <summary>
        /// Attached to all events containing simple message payloads.
        /// </summary>
        internal const EventKeywords MESSAGE_KEYWORD = (EventKeywords) 0x1;

        /// <summary>
        /// Attached to all events containing <see cref="System.Exception"/> information payloads.
        /// </summary>
        internal const EventKeywords EXCEPTION_KEYWORD = (EventKeywords) 0x2;

        /// <summary>
        /// Gets the keyword attached to all events containing simple message payloads.
        /// </summary>
        public static EventKeywords MessageKeyword
            => MESSAGE_KEYWORD;

        /// <summary>
        /// Gets the keyword attached to all events containing <see cref="System.Exception"/> information payloads.
        /// </summary>
        public static EventKeywords ExceptionKeyword
            => EXCEPTION_KEYWORD;
    }
}