//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Configuration;

namespace BadEcho.XmlConfiguration;

/// <summary>
/// Provides a configuration element containing a collection of <see cref="GuidConfigurationElement"/> child elements.
/// </summary>
/// <typeparam name="TElement">The type of configuration element contained by the collection.</typeparam>
[ConfigurationCollection(typeof(GuidConfigurationElement))]
internal sealed class GuidElementCollection<TElement> : ConfigurationElementCollection<TElement, Guid>
    where TElement : GuidConfigurationElement, new()
{
    /// <inheritdoc/>
    protected override Guid GetElementKey(TElement element) 
        => element.Id;
}