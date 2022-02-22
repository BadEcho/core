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
/// Defines a family of plugins that support Bad Echo's Extensibility framework's filterable plugin capabilities, indicating
/// that types belonging to this family can be isolated from types belonging to different families.
/// </summary>
public interface IFilterableFamily
{
    /// <summary>
    /// Gets the identity of this filterable family.
    /// </summary>
    Guid FamilyId { get; }

    /// <summary>
    /// Gets the name of this filterable family.
    /// </summary>
    string Name { get; }
}