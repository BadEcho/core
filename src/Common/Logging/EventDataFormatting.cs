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

using System.Diagnostics.Tracing;
using System.Globalization;
using System.Text;
using BadEcho.Extensions;
using BadEcho.Properties;

namespace BadEcho.Logging;

/// <summary>
/// Provides methods for formatting event data written to Bad Echo event sources.
/// </summary>
public static class EventDataFormatting
{
    private const int BUILDER_CAPACITY = 512;

    private static StringBuilder? _CachedBuilder;

    /// <summary>
    /// Converts the provided event data into a readable string.
    /// </summary>
    /// <param name="eventData">The event data to format.</param>
    /// <param name="includeEventName">Value indicating if the name of the event should be included in the returned string.</param>
    /// <returns>A string representation of <c>eventData</c>.</returns>
    public static string Format(EventWrittenEventArgs eventData, bool includeEventName)
    {
        Require.NotNull(eventData, nameof(eventData));
        
        StringBuilder builder = RentBuilder();

        if (includeEventName)
            builder.Append(CultureInfo.InvariantCulture, $"{eventData.EventName}: ");

        builder.Append(CultureInfo.InvariantCulture,
                       $"{(eventData.Payload != null ? eventData.Payload[0] : Strings.LoggingMissingMessage)}");

        if (eventData.Keywords.HasFlag(Keywords.ExceptionKeyword) && eventData.Payload != null)
        {
            for (int i = 1; i < eventData.Payload.Count - 1; i++)
            {
                builder.AppendLine();

                if (eventData.PayloadNames != null)
                    builder.Append(CultureInfo.InvariantCulture, $"{eventData.PayloadNames[i].ToUpperFirstInvariant()}: ");
                
                builder.Append(eventData.Payload[i]);
            }

            builder.AppendLine();
            builder.Append(eventData.Payload[^1]);
        }

        return FlushBuilder(builder);
    }

    private static StringBuilder RentBuilder()
    {
        StringBuilder? builder = _CachedBuilder;

        if (builder == null)
            return new StringBuilder(BUILDER_CAPACITY);

        _CachedBuilder = null;
        
        return builder;
    }

    private static string FlushBuilder(StringBuilder builder)
    {
        string result = builder.ToString();

        if (builder.Capacity <= BUILDER_CAPACITY) 
            _CachedBuilder = builder.Clear();

        return result;
    }
}
