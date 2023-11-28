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

using BadEcho.Extensions;
using BadEcho.XmlConfiguration.Properties;

namespace BadEcho.XmlConfiguration;

/// <summary>
/// Provides an exception that is thrown when elements are missing from a component's configuration.
/// </summary>
public sealed class ConfigurationMissingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationMissingException"/> class.
    /// </summary>
    /// <param name="missingType">
    /// An enumeration value that specifies the type of entity that is missing from the configuration.
    /// </param>
    /// <param name="missingEntityExpression">
    /// An expression naming or describing the entity that is missing from the configuration.
    /// </param>
    /// <param name="innerException">The exception that is the cause of this exception.</param>
    public ConfigurationMissingException(MissingConfigurationType missingType,
                                         string missingEntityExpression,
                                         Exception innerException)
        : base(FormatMessage(missingType, missingEntityExpression), innerException)
    {
        MissingType = missingType;
        MissingEntityExpression = missingEntityExpression;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationMissingException"/> class.
    /// </summary>
    /// <param name="missingType">
    /// An enumeration value that specifies the type of entity that is missing from the configuration.
    /// </param>
    /// <param name="missingEntityExpression">
    /// An expression naming or describing the entity that is missing from the configuration.
    /// </param>
    public ConfigurationMissingException(MissingConfigurationType missingType, string missingEntityExpression)
        : base(FormatMessage(missingType, missingEntityExpression))
    {
        MissingType = missingType;
        MissingEntityExpression = missingEntityExpression;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationMissingException"/> class.
    /// </summary>
    /// <inheritdoc/>
    public ConfigurationMissingException(string message, Exception innerException)
        : base(message, innerException)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationMissingException"/> class.
    /// </summary>
    /// <inheritdoc/>
    public ConfigurationMissingException(string message)
        : base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationMissingException"/> class.
    /// </summary>
    public ConfigurationMissingException()
    { }

    /// <summary>
    /// Gets the type of entity that is missing from the configuration.
    /// </summary>
    public MissingConfigurationType? MissingType
    { get; }

    /// <summary>
    /// Gets an expression naming or describing the entity that is missing from the configuration.
    /// </summary>
    public string? MissingEntityExpression
    { get; }

    private static string FormatMessage(MissingConfigurationType missingType, string missingEntityExpression)
    {
        return missingType switch
        {
            MissingConfigurationType.Whole 
                => Strings.MissingConfigurationWhole,
            MissingConfigurationType.Section 
                => Strings.MissingConfigurationSection.InvariantFormat(missingEntityExpression),
            MissingConfigurationType.Element 
                => Strings.MissingConfigurationElement.InvariantFormat(missingEntityExpression),
            _ 
                => Strings.MissingConfigurationAttribute.InvariantFormat(missingEntityExpression)
        };
    }
}