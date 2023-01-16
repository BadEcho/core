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
/// Provides an attribute that specifies that a type, property, field, or method provides a particular filterable
/// export to the Bad Echo Extensibility framework.
/// </summary>
[MetadataAttribute]
[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
public sealed class FilterableAttribute : ExportAttribute, IFilterableMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterableAttribute"/> class.
    /// </summary>
    /// <param name="familyId">The identity of the filterable family that the part being exported belongs to.</param>
    /// <param name="partType">The concrete type of the part being exported.</param>
    public FilterableAttribute(string familyId, Type partType)
        : base(typeof(IFilterable))
    {
        Require.NotNull(familyId, nameof(familyId));
        Require.NotNull(partType, nameof(partType));
            
        PartType = partType;

        if (!Guid.TryParse(familyId, out Guid parsedId))
        {
            throw new ArgumentException(Strings.FamilyIdNotValid.InvariantFormat(familyId),
                                        nameof(familyId));
        }

        FamilyId = parsedId;
    }
        
    /// <inheritdoc/>
    public Guid FamilyId
    { get; }

    /// <inheritdoc/>
    public Type PartType 
    { get; }
}