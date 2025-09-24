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

namespace BadEcho.Extensions.Options.Tests;

public class Startup
{
    public void ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        const string appSettingsPrimary = "appsettings.primary.json";
        const string appSettingsSecondary= "appsettings.secondary.json";

        IConfiguration configuration = new ConfigurationBuilder()
                                       .AddJsonFile(appSettingsPrimary, optional: false, reloadOnChange: true)
                                       .AddJsonFile(appSettingsSecondary, optional: false, reloadOnChange : true)
                                       .Build();
        
        services.Configure<PrimaryFirstOptions>(configuration.GetSection(PrimaryFirstOptions.SectionName), appSettingsPrimary);
        services.Configure<PrimaryFirstOptions>("Two", configuration.GetSection("PrimaryFirst2"), appSettingsPrimary);
        services.Configure<PrimaryNoSectionOptions>(configuration, appSettingsPrimary);
        services.Configure<PrimarySecondOptions>(configuration.GetSection(PrimarySecondOptions.SectionName), appSettingsPrimary);
        services.Configure<SecondaryOptions>(configuration.GetSection(SecondaryOptions.SectionName), appSettingsSecondary);
    }
}
