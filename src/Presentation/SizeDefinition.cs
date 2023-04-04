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

using System.Windows;

namespace BadEcho.Presentation;

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