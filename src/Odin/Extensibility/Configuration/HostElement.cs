//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Configuration;
using System.Threading;

namespace BadEcho.Odin.Extensibility.Configuration
{
    /// <summary>
    /// Provides a configuration element for settings options on all instances of Odin's Extensibility framework's
    /// plugin host.
    /// </summary>
    public sealed class HostElement : ConfigurationElement
    {
        private const string PLUGIN_DIRECTORY_ATTRIBUTE_SCHEMA = "pluginDirectory";

        private static readonly Lazy<ConfigurationPropertyCollection> _Properties
            = new(InitializeProperties, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets or sets the path relative from the current application context's base directory to the
        /// directory where all plugins are stored.
        /// </summary>
        /// <remarks>
        /// If this configuration property is not set then it will be assumed that all plugins are stored in the
        /// base directory of the current application context.
        /// </remarks>
        public string PluginDirectory
        {
            get => (string) base[PLUGIN_DIRECTORY_ATTRIBUTE_SCHEMA];
            set => base[PLUGIN_DIRECTORY_ATTRIBUTE_SCHEMA] = value;
        }

        /// <inheritdoc/>
        protected override ConfigurationPropertyCollection Properties
            => _Properties.Value;

        private static ConfigurationPropertyCollection InitializeProperties()
            => new()
               {
                   new ConfigurationProperty(PLUGIN_DIRECTORY_ATTRIBUTE_SCHEMA,
                                             typeof(string),
                                             string.Empty,
                                             null,
                                             null,
                                             ConfigurationPropertyOptions.None)
               };
    }
}