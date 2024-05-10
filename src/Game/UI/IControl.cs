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
/// Defines a user interface element in a game.
/// </summary>
public interface IControl : IArrangeable, IInputElement
{
    /// <summary>
    /// Gets or sets the source of user input for this control.
    /// </summary>
    IInputHandler? InputHandler { get; set; }

    /// <summary>
    /// Gets or sets the parent of this control.
    /// </summary>
    IArrangeable? Parent { get; set; }

    /// <summary>
    /// Gets the desired size of this control.
    /// </summary>
    Size DesiredSize { get; }

    /// <summary>
    /// Gets the bounding rectangle of the region occupied by this control, which includes its margin, border, background,
    /// padding, and content.
    /// </summary>
    Rectangle LayoutBounds { get; }

    /// <summary>
    /// Gets the bounding rectangle of the region occupied by this control's border, which includes its background, padding,
    /// and content.
    /// </summary>
    Rectangle BorderBounds { get; }

    /// <summary>
    /// Gets the bounding rectangle of the region occupied by this control's background, padding, and content.
    /// </summary>
    Rectangle BackgroundBounds { get; }

    /// <summary>
    /// Gets the bounding rectangle of the region occupied by the control's padding and actual content.
    /// </summary>
    Rectangle ContentBounds { get; }

    /// <summary>
    /// Gets or sets a value indicating if this control is enabled in the user interface and receiving input.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets or sets a value indicating if this control is visible.
    /// </summary>
    bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the space between this control and other elements adjacent to it when the layout is generated.
    /// </summary>
    Thickness Margin { get; set; }

    /// <summary>
    /// Gets or sets the space between this control's border and its content.
    /// </summary>
    Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets the thickness of this control's border.
    /// </summary>
    Thickness BorderThickness { get; set; }

    /// <summary>
    /// Gets or sets the specific width of this control.
    /// </summary>
    int? Width { get; set; }

    /// <summary>
    /// Gets or sets the minimum width of this control.
    /// </summary>
    int? MinimumWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum width of this control.
    /// </summary>
    int? MaximumWidth { get; set; }

    /// <summary>
    /// Gets or sets the specific height of this control.
    /// </summary>
    int? Height { get; set; }

    /// <summary>
    /// Gets or sets the minimum height of this control.
    /// </summary>
    int? MinimumHeight { get; set; }

    /// <summary>
    /// Gets or sets the maximum height of this control.
    /// </summary>
    int? MaximumHeight { get; set; }

    /// <summary>
    /// Gets or sets the index of the column within a parent <see cref="Grid"/> panel containing this control.
    /// </summary>
    int Column { get; set; }

    /// <summary>
    /// Gets or sets the index of the row within a parent <see cref="Grid"/> panel containing this control.
    /// </summary>
    int Row { get; set; }

    /// <summary>
    /// Gets or sets the horizontal alignment characteristics that are applied to this control when composed
    /// in a layout parent, such as a type of <see cref="IPanel"/>.
    /// </summary>
    HorizontalAlignment HorizontalAlignment { get; set; }

    /// <summary>
    /// Gets or sets the vertical alignment characteristics that are applied to this control when composed in
    /// a layout parent, such as a type of <see cref="IPanel"/>.
    /// </summary>
    VerticalAlignment VerticalAlignment { get; set; }

    /// <summary>
    /// Gets or sets the background visual of this control.
    /// </summary>
    IVisual? Background { get; set; }

    /// <summary>
    /// Gets or sets the background visual of this control when it has been disabled via the <see cref="IsEnabled"/> property.
    /// </summary>
    IVisual? DisabledBackground { get; set; }

    /// <summary>
    /// Gets or sets the background visual of this control when the cursor is located over it.
    /// </summary>
    IVisual? HoveredBackground { get; set; }

    /// <summary>
    /// Gets or sets the visual of this control's border.
    /// </summary>
    IVisual? Border { get; set; }

    /// <summary>
    /// Gets or sets the visual of this control's border when it has been disabled via the <see cref="IsEnabled"/> property.
    /// </summary>
    IVisual? DisabledBorder { get; set; }

    /// <summary>
    /// Gets or sets the visual of this control's border when the cursor is located over it.
    /// </summary>
    IVisual? HoveredBorder { get; set; }

    /// <summary>
    /// Updates the desired size of this control, called during the <c>Measure</c> pass of a layout update.
    /// </summary>
    /// <param name="availableSize">The available space a parent control is allocating for this control.</param>
    void Measure(Size availableSize);

    /// <summary>
    /// Updates the position of this control, called during the <c>Arrange</c> pass of a layout update.
    /// </summary>
    /// <param name="effectiveArea">
    /// The effective area within the parent that this control should use to arrange itself and any children.   
    /// </param>
    void Arrange(Rectangle effectiveArea);

    /// <summary>
    /// Draws the user interface to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="ConfiguredSpriteBatch"/> instance to use to draw the user interface.</param>
    void Draw(ConfiguredSpriteBatch spriteBatch);

    /// <summary>
    /// Processes events related to user input.
    /// </summary>
    /// <exception cref="InvalidOperationException">Control does not have a valid <see cref="InputHandler"/> assigned.</exception>
    void UpdateInput();
}