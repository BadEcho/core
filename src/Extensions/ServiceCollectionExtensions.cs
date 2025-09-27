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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Logger = BadEcho.Logging.Logger;

namespace BadEcho.Extensions;

/// <summary>
/// Provides extension methods for setting up services that integrate Bad Echo core frameworks with a hosted application.
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

    /// <summary>
    /// Registers a configuration instance that writable <typeparamref name="TOptions"/> instances will bind against.
    /// </summary>
    /// <typeparam name="TOptions">The type of options being configured.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to add services to.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <param name="fileName">The name of the file to write to when changes are saved.</param>
    /// <returns>The current <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    public static IServiceCollection Configure<TOptions>(this IServiceCollection services,
                                                         IConfiguration configuration,
                                                         string fileName)
        where TOptions : class
    {
        var changeTokenSource = new ConfigurationChangeTokenSource<TOptions>(configuration);
        var configureOptions = new ConfigureWritableOptions<TOptions>(configuration, fileName);

        AddRequiredServices(services, changeTokenSource, configureOptions);

        return services;
    }

    /// <summary>
    /// Registers a configuration instance that writable <typeparamref name="TOptions"/> instances will bind against.
    /// </summary>
    /// <typeparam name="TOptions">The type of options being configured.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to add services to.</param>
    /// <param name="configuration">The configuration section being bound.</param>
    /// <param name="fileName">The name of the file to write to when changes are saved.</param>
    /// <returns>The current <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    public static IServiceCollection Configure<TOptions>(this IServiceCollection services,
                                                         IConfigurationSection configuration,
                                                         string fileName)
        where TOptions : class
    {
        var changeTokenSource = new ConfigurationChangeTokenSource<TOptions>(configuration);
        var configureOptions = new ConfigureWritableOptions<TOptions>(configuration, fileName);

        AddRequiredServices(services, changeTokenSource, configureOptions);

        return services;
    }

    /// <summary>
    /// Registers a configuration instance that named writable <typeparamref name="TOptions"/> instances will bind against.
    /// </summary>
    /// <typeparam name="TOptions">The type of options being configured.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to add services to.</param>
    /// <param name="name">The name of the options instance.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <param name="fileName">The name of the file to write to when changes are saved.</param>
    /// <returns>The current <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    public static IServiceCollection Configure<TOptions>(this IServiceCollection services,
                                                         string name,
                                                         IConfiguration configuration,
                                                         string fileName)
        where TOptions : class
    {
        var changeTokenSource = new ConfigurationChangeTokenSource<TOptions>(name, configuration);
        var configureOptions = new ConfigureWritableOptions<TOptions>(name, configuration, fileName);

        AddRequiredServices(services, changeTokenSource, configureOptions);

        return services;
    }

    /// <summary>
    /// Registers a configuration instance that named writable <typeparamref name="TOptions"/> instances will bind against.
    /// </summary>
    /// <typeparam name="TOptions">The type of options being configured.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to add services to.</param>
    /// <param name="name">The name of the options instance.</param>
    /// <param name="configuration">The configuration section being bound.</param>
    /// <param name="fileName">The name of the file to write to when changes are saved.</param>
    /// <returns>The current <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    public static IServiceCollection Configure<TOptions>(this IServiceCollection services,
                                                         string name,
                                                         IConfigurationSection configuration,
                                                         string fileName)
        where TOptions : class
    {

        var changeTokenSource = new ConfigurationChangeTokenSource<TOptions>(name, configuration);
        var configureOptions = new ConfigureWritableOptions<TOptions>(name, configuration, fileName);

        AddRequiredServices(services, changeTokenSource, configureOptions);

        return services;
    }

    private static void AddRequiredServices<TOptions>(IServiceCollection services,
                                                      IOptionsChangeTokenSource<TOptions> changeTokenSource,
                                                      ConfigureWritableOptions<TOptions> configureOptions)
        where TOptions : class
    {
        services.TryAdd(ServiceDescriptor.Singleton(typeof(IWritableOptions<>), typeof(WritableOptions<>)));
        services.AddSingleton(changeTokenSource);
        services.AddSingleton<IConfigureOptions<TOptions>>(configureOptions);
        services.AddSingleton(configureOptions);
    }
}
