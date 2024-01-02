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
/// Defines support for the Bad Echo Extensibility framework's filterable plugin capabilities, indicating that types
/// implementing this can be isolated from others belonging to different plugin families.
/// </summary>
public interface IFilterable
{
    /// <summary>
    /// Gets the identity of the filterable family that this object belongs to.
    /// </summary>
    Guid FamilyId { get; }
}