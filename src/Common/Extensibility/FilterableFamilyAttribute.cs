//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Composition;
using BadEcho.Extensions;
using BadEcho.Properties;

namespace BadEcho.Extensibility;

/// <summary>
/// Provides an attribute that specifies that a class defines a family of filterable exports to Bad Echo's Extensibility
/// framework.
/// </summary>
[MetadataAttribute]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class FilterableFamilyAttribute : ExportAttribute, IFilterableFamilyMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterableFamilyAttribute"/> class.
    /// </summary>
    /// <param name="familyId">The identity of the filterable family being defined.</param>
    /// <param name="name">The name of the filterable family being defined.</param>
    public FilterableFamilyAttribute(string familyId, string name)
        : base(typeof(IFilterableFamily))
    {
        Require.NotNullOrEmpty(name, nameof(name));
        Require.NotNull(familyId, nameof(familyId));

        if (!Guid.TryParse(familyId, out Guid parsedId))
        {
            throw new ArgumentException(Strings.FamilyIdNotValid.InvariantFormat(familyId),
                                        nameof(familyId));
        }

        FamilyId = parsedId;
        Name = name;
    }

    /// <inheritdoc/>
    public Guid FamilyId
    { get; }

    /// <inheritdoc/>
    public string Name 
    { get; }
}