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
/// Provides a set of static methods to aid in matters related to matrices.
/// </summary>
public static class MatrixExtensions
{
    /// <summary>
    /// Multiplies this matrix by an orthogonal projection matrix such that positive values along the z-axis point towards the screen.
    /// </summary>
    /// <param name="source">The matrix to multiply by the orthogonal projection matrix (typically a view matrix).</param>
    /// <param name="viewSize">
    /// A <see cref="SizeF"/> value that defines the bounds of the projection matrix's view volume.
    /// </param>
    /// <returns><c>source</c> multiplied by the configured orthogonal projection matrix.</returns>
    public static Matrix MultiplyBy2DProjection(this Matrix source, SizeF viewSize)
        => MultiplyBy2DProjection(source, viewSize, false);

    /// <summary>
    /// Multiplies this matrix by an orthogonal projection matrix such that positive values along the z-axis point towards the screen.
    /// </summary>
    /// <param name="source">The matrix to multiply by the orthogonal projection matrix (typically a view matrix).</param>
    /// <param name="viewSize">
    /// A <see cref="SizeF"/> value that defines the bounds of the projection matrix's view volume.
    /// </param>
    /// <param name="useHalfPixelOffset">
    /// Value indicating if DirectX 9 style pixel addressing should be used (a half-pixel offset is added to the projection matrix).
    /// </param>
    /// <returns><c>source</c> multiplied by the configured orthogonal projection matrix.</returns>
    public static Matrix MultiplyBy2DProjection(this Matrix source, SizeF viewSize, bool useHalfPixelOffset)
    {   // 3D cameras look into the -z direction (z = 1 is in front of z = 0).
        // We replicate the behavior of SpriteBatch layers, which are ordered in the opposite (z  = 0 is in front of z = 1).
        // This is done by passing 0 for zNearPlane and -1 for zFarPlane; essentially a reverse mapping of the two.
        Matrix projection 
            = Matrix.CreateOrthographicOffCenter(0, viewSize.Width, viewSize.Height, 0, 0, -1);

        if (useHalfPixelOffset)
        {
            projection.M41 -= 0.5f * projection.M11;
            projection.M42 -= 0.5f * projection.M22;
        }

        return Matrix.Multiply(source, projection);
    }
}
