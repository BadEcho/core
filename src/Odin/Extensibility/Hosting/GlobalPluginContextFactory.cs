//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Hosting;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Provides a factory that constructs a <see cref="PluginContext"/> object covering all plugins that are discoverable
    /// through Odin's Extensibility framework.
    /// </summary>
    /// <remarks>
    /// This is the standard plugin context factory used to load any plugins exported for consumption with no amount of discretion
    /// at play. Any contracts loaded through contexts created with this factory will have all possible implementations of said
    /// contracts returned.
    /// </remarks>
    internal sealed class GlobalPluginContextFactory : IPluginContextFactory
    {
        private readonly string _pluginDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalPluginContextFactory"/> class.
        /// </summary>
        /// <param name="pluginDirectory">Full path to the directory where plugins will be loaded from.</param>
        public GlobalPluginContextFactory(string pluginDirectory)
        { 
            _pluginDirectory = pluginDirectory;
        }

        /// <inheritdoc/>
        public CompositionHost CreateContainer()
        {
            var configuration = new ContainerConfiguration()
                .WithDirectory(_pluginDirectory);

            this.LoadConventions(configuration);

            return configuration.CreateContainer();
        }
    }
}
