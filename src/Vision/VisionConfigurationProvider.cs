//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
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
        /// <inheritdoc/>
        public void ConfigureRules(ConventionBuilder conventions)
        {
            conventions.ForTypesDerivedFrom<IConfigurationProvider>()
                       .Shared();
        }
    }
}