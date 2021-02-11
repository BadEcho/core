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
        internal const EventKeywords MessageKeywordValue = (EventKeywords) 0x1;

        /// <summary>
        /// Attached to all events containing <see cref="System.Exception"/> information payloads.
        /// </summary>
        internal const EventKeywords ExceptionKeywordValue = (EventKeywords) 0x2;

        /// <summary>
        /// Gets the keyword attached to all events containing simple message payloads.
        /// </summary>
        public static EventKeywords MessageKeyword
            => MessageKeywordValue;

        /// <summary>
        /// Gets the keyword attached to all events containing <see cref="System.Exception"/> information payloads.
        /// </summary>
        public static EventKeywords ExceptionKeyword
            => ExceptionKeywordValue;
    }
}