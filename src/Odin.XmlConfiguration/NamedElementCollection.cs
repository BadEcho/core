//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Configuration;

namespace BadEcho.Odin.XmlConfiguration;

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