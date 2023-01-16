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