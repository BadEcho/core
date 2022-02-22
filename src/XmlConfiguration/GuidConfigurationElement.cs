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

using System.ComponentModel;
using System.Configuration;

namespace BadEcho.XmlConfiguration;

/// <summary>
/// Provides a named <see cref="ConfigurationElement"/> where a unique <see cref="Guid"/> value is used as its
/// key.
/// </summary>
internal class GuidConfigurationElement : ConfigurationElement
{
    private const string ID_ATTRIBUTE_SCHEMA = "id";
    private const string NAME_ATTRIBUTE_SCHEMA = "name";

    /// <summary>
    /// Gets or sets the identifying <see cref="Guid"/> for this configuration element.
    /// </summary>
    /// <remarks>
    /// Configuration attributes decorate this property to add support for use of the declarative coding model
    /// by derivations and/or consumers of this class.
    /// </remarks>
    [ConfigurationProperty(ID_ATTRIBUTE_SCHEMA, IsKey = true, IsRequired = true)]
    [TypeConverter(typeof(GuidConverter))]
    public Guid Id
    {
        get => (Guid) base[ID_ATTRIBUTE_SCHEMA];
        set => base[ID_ATTRIBUTE_SCHEMA] = value;
    }

    /// <summary>
    /// Gets or sets the name for this element.
    /// </summary>
    /// <remarks>
    /// Configuration attributes decorate this property to add support for use of the declarative coding model
    /// by derivations and/or consumers of this class.
    /// </remarks>
    [ConfigurationProperty(NAME_ATTRIBUTE_SCHEMA, IsKey = false, IsRequired = false, DefaultValue = "")]
    public string Name
    {
        get => (string) base[NAME_ATTRIBUTE_SCHEMA];
        set => base[NAME_ATTRIBUTE_SCHEMA] = value;
    }

    /// <summary>
    /// Creates a representative configuration object for the <see cref="Id"/> configuration property.
    /// </summary>
    /// <returns>A <see cref="ConfigurationProperty"/> objects for the <see cref="Id"/> configuration property.</returns>
    /// <remarks>
    /// This method of creating the property for <see cref="Id"/> should be used when adhering to the programmatic
    /// coding model.
    /// </remarks>
    protected static ConfigurationProperty CreateIdProperty()
        => new(ID_ATTRIBUTE_SCHEMA,
               typeof(Guid),
               null,
               new GuidConverter(),
               null,
               ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);

    /// <summary>
    /// Creates a representative configuration object for the <see cref="Name"/> configuration property.
    /// </summary>
    /// <returns>A <see cref="ConfigurationProperty"/> objects for the <see cref="Name"/> configuration property.</returns>
    /// <remarks>
    /// This method of creating the property for <see cref="Name"/> should be used when adhering to the programmatic
    /// coding model.
    /// </remarks>
    protected static ConfigurationProperty CreateNameProperty()
        => new(NAME_ATTRIBUTE_SCHEMA,
               typeof(string),
               string.Empty,
               null,
               null,
               ConfigurationPropertyOptions.None);
}