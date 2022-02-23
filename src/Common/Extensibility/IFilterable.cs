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