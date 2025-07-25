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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Logger = BadEcho.Logging.Logger;

namespace BadEcho.Extensions.Logging;

/// <summary>
/// Provides extension methods for adding Bad Echo event support to an <see cref="ILoggingBuilder"/> instance.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds support for forwarding Bad Echo diagnostic events to configured logger providers.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The current <see cref="ILoggingBuilder"/> instance so that additional calls can be chained.</returns>
    public static ILoggingBuilder ForwardBadEchoEvents(this ILoggingBuilder builder)
    {
        Require.NotNull(builder, nameof(builder));
        
        Logger.DisableDefaultListener();

        builder.Services.AddHostedService<BadEchoEventSourceLogForwarder>();
        
        return builder;
    }
}
