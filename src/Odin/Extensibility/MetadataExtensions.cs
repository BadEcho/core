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

using System.ComponentModel;
using BadEcho.Odin.Extensions;

namespace BadEcho.Odin.Extensibility;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to metadata from the Managed Extensibility
/// Framework.
/// </summary>
public static class MetadataExtensions
{
    private static readonly Type _DefaultMetadataType = typeof(IDictionary<string, object>);

    /// <summary>
    /// Creates a weakly-typed variant of the strongly-typed metadata type provided.
    /// </summary>
    /// <param name="metadataType">A strongly-typed metadata type.</param>
    /// <returns>
    /// A collection of <see cref="KeyValuePair{String,Type}"/> objects composed of the names and types of all the properties belonging to
    /// <c>metadataType</c>.
    /// </returns>
    public static IEnumerable<KeyValuePair<string, Type>> BuildMetadata(Type? metadataType)
    {
        if (metadataType == null
            || _DefaultMetadataType.IsA(metadataType)
            || metadataType.FindConstructor(_DefaultMetadataType) != null 
            || !metadataType.IsInterface)
        {
            return Enumerable.Empty<KeyValuePair<string, Type>>();
        }

        return metadataType.GetAllProperties()
                           .Where(p => p.GetAttribute<DefaultValueAttribute>() == null)
                           .ToList()
                           .Select(p => new KeyValuePair<string, Type>(p.Name, p.PropertyType));
    }
}