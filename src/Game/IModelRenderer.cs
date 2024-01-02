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

namespace BadEcho.Game;

/// <summary>
/// Defines a renderer of one or more related models using shaders appropriate for the model type.
/// </summary>
public interface IModelRenderer
{
    /// <summary>
    /// Gets the size of the model when it is rendered.
    /// </summary>
    SizeF Size { get; }

    /// <summary>
    /// Draws the models to the screen.
    /// </summary>
    void Draw();

    /// <summary>
    /// Draws the models to the screen.
    /// </summary>
    /// <param name="view">The view matrix to use.</param>
    void Draw(Matrix view);

    /// <summary>
    /// Draws the models to the screen.
    /// </summary>
    /// <param name="view">The view matrix to use.</param>
    /// <param name="alpha">The transparency of the material being rendered.</param>
    void Draw(Matrix view, float alpha);
}
