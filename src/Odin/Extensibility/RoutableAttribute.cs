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

using System.Composition;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Extensibility;

/// <summary>
/// Provides an attribute that specifies that a type provides a call-routable plugin adapter to Odin's Extensibility
/// framework.
/// </summary>
[MetadataAttribute]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RoutableAttribute : ExportAttribute, IRoutableMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoutableAttribute"/> class.
    /// </summary>
    /// <param name="pluginId">The identity of the call-routable plugin being exported.</param>
    /// <param name="segmentedType">The type of contract being segmented.</param>
    public RoutableAttribute(string pluginId, Type segmentedType)
        : base(CreatePluginAdapterType(segmentedType))
    {
        Require.NotNull(pluginId, nameof(pluginId));
            
        if (!Guid.TryParse(pluginId, out Guid parsedId))
        {
            throw new ArgumentException(Strings.RoutablePluginIdNotValid.InvariantFormat(pluginId),
                                        nameof(pluginId));
        }
            
        PluginId = parsedId;
    }

    /// <inheritdoc/>
    public Guid PluginId 
    { get; }

    private static Type CreatePluginAdapterType(Type segmentedType)
    {
        Require.NotNull(segmentedType, nameof(segmentedType));

        var openAdapterType = typeof(IPluginAdapter<>);

        return openAdapterType.MakeGenericType(segmentedType);
    }
}