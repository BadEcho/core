//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Configuration;
using System.Threading;
using BadEcho.Odin.Configuration;

namespace BadEcho.Odin.Extensibility.Configuration
{
    /// <summary>
    /// Provides a configuration element for a contract being segmented by one or more call-routable plugins.
    /// </summary>
    public sealed class ContractElement : NamedConfigurationElement
    {
        private const string ROUTABLE_PLUGIN_CHILDREN_SCHEMA = "routablePlugins";

        private static readonly Lazy<ConfigurationPropertyCollection> _Properties
            = new(InitializeProperties, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets the collection of call-routable plugins that segment the contract represented by this element.
        /// </summary>
        public GuidElementCollection<RoutablePluginElement> RoutablePlugins
            => (GuidElementCollection<RoutablePluginElement>) base[ROUTABLE_PLUGIN_CHILDREN_SCHEMA];

        /// <inheritdoc/>
        protected override ConfigurationPropertyCollection Properties
            => _Properties.Value;

        private static ConfigurationPropertyCollection InitializeProperties()
            => new()
               {
                   new ConfigurationProperty(ROUTABLE_PLUGIN_CHILDREN_SCHEMA,
                                             typeof(GuidElementCollection<RoutablePluginElement>),
                                             null,
                                             null,
                                             null,
                                             ConfigurationPropertyOptions.IsRequired),
                   CreateNameProperty()
               };
    }
}