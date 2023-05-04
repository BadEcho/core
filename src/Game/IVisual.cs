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

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BadEcho.Game;

/// <summary>
/// Defines a visual component that can be drawn to the screen.
/// </summary>
public interface IVisual
{
    /// <summary>
    /// Draws the component to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance to use to draw the component.</param>
    /// <param name="targetArea">The bounding rectangle of the region of the screen that this component will be drawn to.</param>
    void Draw(SpriteBatch spriteBatch, Rectangle targetArea);
}
