//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Extensibility;

/// <summary>
/// Provides a metadata view for a filterable family's metadata.
/// </summary>
public sealed class FilterableFamilyMetadataView : IFilterableFamilyMetadata
{
    /// <inheritdoc/>
    public Guid FamilyId 
    { get; set; }

    /// <inheritdoc/>
    public string Name 
    { get; set; } = string.Empty;
}