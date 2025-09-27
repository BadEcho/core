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
using Microsoft.Extensions.Logging;

namespace BadEcho.Extensions.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        const string appSettingsPrimary = "appsettings.primary.json";
        const string appSettingsSecondary = "appsettings.secondary.json";
        const string appSettingsNonexistent = "appsettings.nonexistent.json";

        IConfiguration configuration = new ConfigurationBuilder()
                                       .AddJsonFile("appsettings.json")
                                       .AddJsonFile(appSettingsPrimary, optional: false, reloadOnChange: true)
                                       .AddJsonFile(appSettingsSecondary, optional: false, reloadOnChange: true)
                                       .AddJsonFile(appSettingsNonexistent, optional: true, reloadOnChange: true)
                                       .Build();
        services.AddLogging(l =>
        {
            l.AddDebug();
            l.AddConfiguration(configuration.GetSection("Logging"));
            l.AddProvider(new TestLoggerProvider());
        });

        services.AddEventSourceLogForwarder();

        services.Configure<PrimaryFirstOptions>(configuration.GetSection(PrimaryFirstOptions.SectionName), appSettingsPrimary);
        services.Configure<PrimaryFirstOptions>("Two", configuration.GetSection("PrimaryFirst2"), appSettingsPrimary);
        services.Configure<PrimaryNoSectionOptions>(configuration, appSettingsPrimary);
        services.Configure<PrimarySecondOptions>(configuration.GetSection(PrimarySecondOptions.SectionName), appSettingsPrimary);
        services.Configure<SecondaryOptions>(configuration.GetSection(SecondaryOptions.SectionName), appSettingsSecondary);
        services.Configure<NonexistentOptions>(configuration.GetSection(NonexistentOptions.SectionName),
                                               appSettingsNonexistent);
    }
}
