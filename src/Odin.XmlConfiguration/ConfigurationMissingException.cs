//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.XmlConfiguration.Properties;

namespace BadEcho.Odin.XmlConfiguration
{
    /// <summary>
    /// Provides an exception that is thrown when elements are missing from a component's configuration.
    /// </summary>
    [Serializable]
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
        /// Initializes a new instance of the <see cref="ConfigurationMissingException"/> class with serialized data.
        /// </summary>
        /// <inheritdoc/>
        private ConfigurationMissingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            MissingType 
                = (MissingConfigurationType?) info.GetValue(nameof(MissingType), typeof(MissingConfigurationType));
            
            MissingEntityExpression = info.GetString(nameof(MissingEntityExpression));
        }

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

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Require.NotNull(info, nameof(info));

            info.AddValue(nameof(MissingType), MissingType);
            info.AddValue(nameof(MissingEntityExpression), MissingEntityExpression);

            base.GetObjectData(info, context);
        }

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
}