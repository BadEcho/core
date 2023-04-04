//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.Tracing;

namespace BadEcho.Logging;

/// <summary>
/// Provides keywords used for the categorization and filtering of Bad Echo's Logging framework.
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