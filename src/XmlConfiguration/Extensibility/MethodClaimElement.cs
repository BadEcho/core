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

namespace BadEcho.XmlConfiguration.Extensibility;

/// <summary>
/// Provides a configuration element that expresses a claim of ownership over a segmented contract's
/// method.
/// </summary>
internal sealed class MethodClaimElement : NamedConfigurationElement
{
    private static readonly Lazy<ConfigurationPropertyCollection> _Properties
        = new(InitializeProperties, LazyThreadSafetyMode.PublicationOnly);

    /// <inheritdoc/>
    protected override ConfigurationPropertyCollection Properties
        => _Properties.Value;

    private static ConfigurationPropertyCollection InitializeProperties()
        => new()
           {
               CreateNameProperty()
           };
}