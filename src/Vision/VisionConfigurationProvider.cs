//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Composition;
using System.Composition.Convention;
using BadEcho.Configuration;
using BadEcho.Extensibility.Hosting;
using BadEcho.Vision.Extensibility;

namespace BadEcho.Vision;

/// <summary>
/// Provides a source for hot-pluggable, but otherwise cached, configuration data.
/// </summary>
/// <suppressions>
/// ReSharper disable UnusedType.Local
/// </suppressions>
[Export(typeof(IConfigurationProvider))]
internal sealed class VisionConfigurationProvider : JsonConfigurationProvider<VisionModuleConfiguration>
{
    /// <inheritdoc/>
    protected override string SettingsFile 
        => "settings.json";

    /// <summary>
    /// Configures exported Vision application configuration instances to be shared singletons, preventing redundant and unneeded
    /// resource allocation.
    /// </summary>
    [Export(typeof(IConventionProvider))]
    private sealed class SharedConfigurationProvider : IConventionProvider
    {
        public void ConfigureRules(ConventionBuilder conventions)
        {
            conventions.ForTypesDerivedFrom<IConfigurationProvider>()
                       .Shared();
        }
    }
}