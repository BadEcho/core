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
using Logger = BadEcho.Logging.Logger;

namespace BadEcho.Extensions.Logging;

/// <summary>
/// Provides extension methods for setting up services that add Bad Echo event support to configured logger providers.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a service that will forward Bad Echo diagnostic events to configured logger providers.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to add services to.</param>
    /// <returns>The current <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    public static IServiceCollection AddEventSourceLogForwarder(this IServiceCollection services)
    {
        Require.NotNull(services, nameof(services));
        Logger.DisableDefaultListener();

        services.AddHostedService<EventSourceLogForwarder>();
        
        return services;
    }
}
