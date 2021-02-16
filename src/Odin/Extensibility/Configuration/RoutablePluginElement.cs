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
    /// Provides a configuration element for a call-routable plugin.
    /// </summary>
    public sealed class RoutablePluginElement : GuidConfigurationElement
    {
        private const string METHOD_CLAIMS_CHILD_SCHEMA = "methodClaims";
        private const string PRIMARY_ATTRIBUTE_SCHEMA = "primary";

        private static readonly Lazy<ConfigurationPropertyCollection> _Properties
            = new(InitializeProperties, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets the collection of methods claimed by the plugin represented by this element.
        /// </summary>
        public NamedElementCollection<MethodClaimElement> MethodClaims
            => (NamedElementCollection<MethodClaimElement>) base[METHOD_CLAIMS_CHILD_SCHEMA];

        /// <summary>
        /// Gets a value indicating whether the plugin represented by this element is the primary call-routable plugin
        /// for a particular contract.
        /// </summary>
        public bool Primary
            => (bool) base[PRIMARY_ATTRIBUTE_SCHEMA];

        /// <inheritdoc/>
        protected override ConfigurationPropertyCollection Properties
            => _Properties.Value;

        private static ConfigurationPropertyCollection InitializeProperties()
            => new()
               {
                   new ConfigurationProperty(METHOD_CLAIMS_CHILD_SCHEMA,
                                             typeof(NamedElementCollection<MethodClaimElement>),
                                             null,
                                             null,
                                             null,
                                             ConfigurationPropertyOptions.None),
                   new ConfigurationProperty(PRIMARY_ATTRIBUTE_SCHEMA,
                                             typeof(bool),
                                             false,
                                             null,
                                             null,
                                             ConfigurationPropertyOptions.None),
                   CreateIdProperty(),
                   CreateNameProperty()
               };
    }
}