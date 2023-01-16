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

using System.Composition;
using BadEcho.Extensions;
using BadEcho.Properties;

namespace BadEcho.Extensibility;

/// <summary>
/// Provides an attribute that specifies that a type provides a call-routable plugin adapter to Bad Echo's Extensibility
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