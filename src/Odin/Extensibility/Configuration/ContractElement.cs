//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Linq;
using BadEcho.Odin.Configuration;
using BadEcho.Odin.Properties;

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

        /// <summary>
        /// Searches for and returns the identifier for the call-routable plugin marked as the primary plugin.
        /// </summary>
        /// <returns>The identifier of the primary call-routable plugin.</returns>
        public Guid FindPrimaryPluginId()
        {
            List<RoutablePluginElement> primaryPlugins = RoutablePlugins.Where(p => p.Primary).ToList();

            if (primaryPlugins.Count == 0)
            {
                throw new ConfigurationMissingException(
                    MissingConfigurationType.Attribute,
                    $"//{ROUTABLE_PLUGIN_CHILDREN_SCHEMA}/*[@{RoutablePluginElement.PrimaryAttributeSchema}='true']");
            }

            if (primaryPlugins.Count > 1)
                throw new ConfigurationErrorsException(Strings.MultiplePrimaryRoutablePlugins);

            return primaryPlugins[0].Id;
        }

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