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

using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Defines a generated model of 3D triangle primitives for rendering.
/// </summary>
public interface IPrimitiveModel : IDisposable
{
    /// <summary>
    /// Draws the model to the screen.
    /// </summary>
    /// <param name="effect">The shaders to be used during the rendering of this model.</param>
    void Draw(BasicEffect effect);

    /// <summary>
    /// Draws the model to the screen.
    /// </summary>
    /// <param name="effect">Custom shaders that support texture mapping to be used during the rendering of this model.</param>
    void Draw(ITextureEffect effect);
}
