//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BadEcho.Odin.XmlConfiguration.Properties;

namespace BadEcho.Odin.XmlConfiguration.Extensibility
{
    /// <summary>
    /// Provides a validator for a collection of call-routable plugin configuration elements assigned to
    /// a contract.
    /// </summary>
    internal sealed class RoutablePluginsValidator : ConfigurationValidatorBase
    {
        /// <inheritdoc/>
        public override bool CanValidate(Type type)
            => type == typeof(GuidElementCollection<RoutablePluginElement>);

        /// <inheritdoc/>
        public override void Validate(object value)
        {
            Require.NotNull(value, nameof(value));

            if (value is not GuidElementCollection<RoutablePluginElement> routablePlugins)
                throw new ArgumentException(Strings.RoutablePluginsCollectionWrongType, nameof(value));

            IEnumerable<IGrouping<string, MethodClaimElement>> claimsByName
                = routablePlugins.SelectMany(p => p.MethodClaims)
                                 .GroupBy(m => m.Name);

            if (claimsByName.Any(m => m.Count() > 1))
                throw new ConfigurationErrorsException(Strings.MethodClaimedByMultiplePlugins);

            int primaryPluginsCount = routablePlugins.Count(p => p.Primary);

            if (primaryPluginsCount == 0)
            {
                throw new ConfigurationMissingException(
                    MissingConfigurationType.Attribute,
                    $"//{ContractElement.RoutablePluginChildrenSchema}/*[@{RoutablePluginElement.PrimaryAttributeSchema}='true']");
            }

            if (primaryPluginsCount > 1)
                throw new ConfigurationErrorsException(Strings.MultiplePrimaryRoutablePlugins);
        }
    }
}