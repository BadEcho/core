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

namespace BadEcho.Extensibility;

/// <summary>
/// Defines metadata that describes a filterable export to the Bad Echo Extensibility framework.
/// </summary>
public interface IFilterableMetadata
{
    /// <summary>
    /// Gets the identity of the filterable family that the part being exported belongs to.
    /// </summary>
    Guid FamilyId { get; }

    /// <summary>
    /// Gets the concrete type of the part exported.
    /// </summary>
    Type? PartType { get; }
}