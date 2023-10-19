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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Defines shaders that support a standard set of parameters.
/// </summary>
public interface IStandardEffect
{
    /// <summary>
    /// Gets or sets an optional matrix to apply to position, rotation, scale, and depth data.
    /// </summary>
    Matrix? MatrixTransform { get; set; }

    /// <summary>
    /// Gets or sets the transparency of the material being rendered.
    /// </summary>
    float Alpha { get; set; }

    /// <summary>
    /// Gets or sets the active technique.
    /// </summary>
    EffectTechnique CurrentTechnique { get; set; }
}
