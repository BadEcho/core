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
    /// Provides a configuration section for the Extensibility framework.
    /// </summary>
    /// <suppresions>
    /// ReSharper disable ConstantNullCoalescingCondition
    /// </suppresions>
    public sealed class ExtensibilitySection : BindableConfigurationSection
    {
        private const string HOST_CHILD_SCHEMA = "host";
        private const string CONTRACTS_CHILD_SCHEMA = "contracts";

        private static readonly Lazy<ConfigurationPropertyCollection> _Properties
            = new(InitializeProperties, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets the collection of contracts being segmented by call-routable plugins.
        /// </summary>
        public NamedElementCollection<ContractElement> Contracts
            => (NamedElementCollection<ContractElement>) base[CONTRACTS_CHILD_SCHEMA];

        /// <summary>
        /// Gets the schema name for the Extensibility framework's configuration section.
        /// </summary>
        internal static string Schema
            => "extensibility";

        /// <summary>
        /// Gets the full path to the Extensibility framework's configuration section.
        /// </summary>
        internal static string SectionPath
            => $"{BadEchoSectionGroup.Schema}/{Schema}";

        /// <inheritdoc/>
        protected override ConfigurationPropertyCollection Properties 
            => _Properties.Value;

        /// <summary>
        /// Gets an instance of this configuration section from the current application's default configuration.
        /// </summary>
        /// <returns>This section as it is declared in the current application's default configuration.</returns>
        internal static ExtensibilitySection GetSection()
            => (ExtensibilitySection) ConfigurationManager.GetSection(SectionPath) ?? new ExtensibilitySection();

        /// <summary>
        /// Gets or sets the configuration element containing settings for the plugin host.
        /// </summary>
        public HostElement Host
        {
            get => (HostElement) base[HOST_CHILD_SCHEMA];
            set => base[HOST_CHILD_SCHEMA] = value;
        }

        private static ConfigurationPropertyCollection InitializeProperties()
            => new()
               {
                   new ConfigurationProperty(HOST_CHILD_SCHEMA,
                                             typeof(HostElement),
                                             new HostElement(),
                                             null,
                                             null,
                                             ConfigurationPropertyOptions.None),
                   new ConfigurationProperty(CONTRACTS_CHILD_SCHEMA,
                                             typeof(NamedElementCollection<ContractElement>),
                                             new NamedElementCollection<ContractElement>(),
                                             null,
                                             null,
                                             ConfigurationPropertyOptions.None),
                   CreateXmlnsProperty()
               };
    }
}