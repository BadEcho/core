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

using System.Windows;

namespace BadEcho.Fenestra;

/// <summary>
/// Provides a simple container of an element's size value and the identity of any shared-size group it belongs to.
/// </summary>
public sealed class SizeDefinition 
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SizeDefinition"/> class.
    /// </summary>
    /// <param name="size">The size value for the element.</param>
    /// <param name="sharedSizeGroup">The identity of the shared-size group the element belongs to.</param>
    public SizeDefinition(GridLength size, string sharedSizeGroup)
    {
        Size = size;
        SharedSizeGroup = sharedSizeGroup;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeDefinition"/> class.
    /// </summary>
    public SizeDefinition()
    { }

    /// <summary>
    /// Gets or sets the size value for the element.
    /// </summary>
    public GridLength Size
    { get; set; } = new(1.0, GridUnitType.Star);

    /// <summary>
    /// Gets or sets the identity of the shared-size group the element belongs to.
    /// </summary>
    public string? SharedSizeGroup
    { get; set; }
}