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

namespace BadEcho.Extensibility;

/// <summary>
/// Defines metadata that describes a filterable family of plugins used by the Bad Echo Extensibility framework to
/// isolate plugins from each other.
/// </summary>
public interface IFilterableFamilyMetadata
{
    /// <summary>
    /// Gets the identity of the filterable family.
    /// </summary>
    Guid FamilyId { get; }

    /// <summary>
    /// Gets the name of the filterable family.
    /// </summary>
    string Name { get; }
}