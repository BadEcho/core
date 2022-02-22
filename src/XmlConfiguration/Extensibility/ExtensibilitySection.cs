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

using System.Configuration;
using BadEcho.Extensibility.Configuration;

namespace BadEcho.XmlConfiguration.Extensibility;

/// <summary>
/// Provides a configuration section for Bad Echo's Extensibility framework.
/// </summary>
/// <suppresions>
/// ReSharper disable ConstantNullCoalescingCondition
/// </suppresions>
internal sealed class ExtensibilitySection : BindableConfigurationSection, IExtensibilityConfiguration
{
    private const string HOST_CHILD_SCHEMA = "host";
    private const string SEGMENTED_CONTRACTS_CHILD_SCHEMA = "segmentedContracts";

    private static readonly Lazy<ConfigurationPropertyCollection> _Properties
        = new(InitializeProperties, LazyThreadSafetyMode.PublicationOnly);

    string IExtensibilityConfiguration.PluginDirectory 
        => Host.PluginDirectory;

    IEnumerable<IContractConfiguration> IExtensibilityConfiguration.SegmentedContracts 
        => SegmentedContracts;

    /// <summary>
    /// Gets the collection of contracts being segmented by call-routable plugins.
    /// </summary>
    public NamedElementCollection<ContractElement> SegmentedContracts
        => (NamedElementCollection<ContractElement>) base[SEGMENTED_CONTRACTS_CHILD_SCHEMA];

    /// <summary>
    /// Gets the schema name for Bad Echo's Extensibility framework's configuration section.
    /// </summary>
    internal static string Schema
        => "extensibility";

    /// <summary>
    /// Gets the full path to Bad Echo's Extensibility framework's configuration section.
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
               new ConfigurationProperty(SEGMENTED_CONTRACTS_CHILD_SCHEMA,
                                         typeof(NamedElementCollection<ContractElement>),
                                         new NamedElementCollection<ContractElement>(),
                                         null,
                                         null,
                                         ConfigurationPropertyOptions.None),
               CreateXmlnsProperty()
           };
}