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
public interface IControl : IControlProperties, IArrangeable, IInputElement
{
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
}