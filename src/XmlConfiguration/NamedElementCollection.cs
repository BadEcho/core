//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
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
/// Provides a collection of <see cref="NamedConfigurationElement"/> objects.
/// </summary>
/// <typeparam name="TElement">The type of configuration element contained by the collection.</typeparam>
[ConfigurationCollection(typeof(NamedConfigurationElement))]
internal sealed class NamedElementCollection<TElement> : ConfigurationElementCollection<TElement, string>
    where TElement : NamedConfigurationElement, new()
{
    /// <inheritdoc/>
    protected override string GetElementKey(TElement element) 
        => element.Name;
}