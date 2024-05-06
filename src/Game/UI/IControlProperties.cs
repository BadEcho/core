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

using Microsoft.Xna.Framework;

namespace BadEcho.Game.UI;

/// <summary>
/// Defines the properties for a user interface element in a game.
/// </summary>
public interface IControlProperties
{
    /// <summary>
    /// Gets the parent of the control.
    /// </summary>
    IArrangeable? Parent { get; }
    
    /// <summary>
    /// Gets the desired size of the control.
    /// </summary>
    Size DesiredSize { get; }

    /// <summary>
    /// Gets the bounding rectangle of the region occupied by the control, which includes its margin, border, background,
    /// padding, and content.
    /// </summary>
    Rectangle LayoutBounds { get; }

    /// <summary>
    /// Gets the bounding rectangle of the region occupied by the control's border, which includes its background, padding,
    /// and content.
    /// </summary>
    Rectangle BorderBounds { get; }

    /// <summary>
    /// Gets the bounding rectangle of the region occupied by the control's background, padding, and content.
    /// </summary>
    Rectangle BackgroundBounds { get; }

    /// <summary>
    /// Gets the bounding rectangle of the region occupied by the control's padding and actual content.
    /// </summary>
    Rectangle ContentBounds { get; }

    /// <summary>
    /// Gets or sets a value indicating if the control is enabled in the user interface and receiving input.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets or sets a value indicating if the control is visible.
    /// </summary>
    bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the space between the control and other elements adjacent to it when the layout is generated.
    /// </summary>
    Thickness Margin { get; set; }

    /// <summary>
    /// Gets or sets the space between the control's border and its content.
    /// </summary>
    Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets the thickness of the control's border.
    /// </summary>
    Thickness BorderThickness { get; set; }

    /// <summary>
    /// Gets or sets the specific width of the control.
    /// </summary>
    /// <remarks>If a width is not explicitly set, then this control's width will be sized based on the size of its content.</remarks>
    int? Width { get; set; }

    /// <summary>
    /// Gets or sets the minimum width of the control.
    /// </summary>
    /// <remarks>
    /// This will ensure this control's width is at least the specified value, taking precedence over <see cref="MaximumWidth"/> and
    /// <see cref="Width"/> values in regard to the minimum width constraint. If a minimum width is not set, then no minimum width
    /// constraint exists.
    /// </remarks>
    int? MinimumWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum width of the control.
    /// </summary>
    /// <remarks>
    /// This will ensure this control's width doesn't exceed the specified value, however the <see cref="MinimumWidth"/> will take precedence
    /// over it if it happens to exceed this value. If a maximum width is not set, then no maximum width constraint exists.
    /// </remarks>
    int? MaximumWidth { get; set; }

    /// <summary>
    /// Gets or sets the specific height of the control.
    /// </summary>
    /// <remarks>If a height is not explicitly set, then this control's height will be sized based on the size of its content.</remarks>
    int? Height { get; set; }

    /// <summary>
    /// Gets or sets the minimum height of the control.
    /// </summary>
    /// <remarks>
    /// This will ensure this control's height is at least the specified value, taking precedence over <see cref="MaximumHeight"/> and
    /// <see cref="Height"/> values in regard to the minimum height constraint. If a minimum height is not set, then no minimum height
    /// constraint exists.
    /// </remarks>
    int? MinimumHeight { get; set; }

    /// <summary>
    /// Gets or sets the maximum height of the control.
    /// </summary>
    /// <remarks>
    /// This will ensure this control's height doesn't exceed the specified value, however the <see cref="MinimumHeight"/> will take precedence
    /// over it if it happens to exceed this value. If a maximum height is not set, then no maximum height constraint exists.
    /// </remarks>
    int? MaximumHeight { get; set; }

    /// <summary>
    /// Gets or sets the index of the column within a parent <see cref="Grid"/> panel containing the control.
    /// </summary>
    /// <remarks>
    /// This property is ignored if <see cref="Parent"/> is not set to a <see cref="Grid"/> instance.
    /// </remarks>
    int Column { get; set; }

    /// <summary>
    /// Gets or sets the index of the row within a parent <see cref="Grid"/> panel containing the control.
    /// </summary>
    /// <remarks>
    /// This property is ignored if <see cref="Parent"/> is not set to a <see cref="Grid"/> instance.
    /// </remarks>
    int Row { get; set; }

    /// <summary>
    /// Gets or sets the horizontal alignment characteristics that are applied to the control when composed in a layout parent, such as a
    /// type of <see cref="Panel"/>.
    /// </summary>
    HorizontalAlignment HorizontalAlignment { get; set; }

    /// <summary>
    /// Gets or sets the vertical alignment characteristics that are applied to the control when composed in a layout parent, such as a type
    /// of <see cref="Panel"/>.
    /// </summary>
    VerticalAlignment VerticalAlignment { get; set; }
    
    /// <summary>
    /// Gets or sets the background visual of the control.
    /// </summary>
    IVisual? Background { get; set; }

    /// <summary>
    /// Gets or sets the background visual of the control when it has been disabled via the <see cref="IsEnabled"/> property.
    /// </summary>
    IVisual? DisabledBackground { get; set; }

    /// <summary>
    /// Gets or sets the background visual of the control when the cursor is located over it.
    /// </summary>
    /// <remarks>
    /// Not setting this property will result in no change to the control's background when the cursor is over it.
    /// </remarks>
    IVisual? HoveredBackground { get; set; }

    /// <summary>
    /// Gets or sets the visual of the control's border.
    /// </summary>
    IVisual? Border { get; set; }

    /// <summary>
    /// Gets or sets the visual of the control's border when it has been disabled via the <see cref="IsEnabled"/> property.
    /// </summary>
    IVisual? DisabledBorder { get; set; }

    /// <summary>
    /// Gets or sets the visual of the control's border when the cursor is located over it.
    /// </summary>
    /// <remarks>
    /// Not setting this property will result in no change to the appearance of the button's border when the cursor is over it.
    /// </remarks>
    IVisual? HoveredBorder { get; set; }
}
