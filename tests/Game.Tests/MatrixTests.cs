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
using Xunit;

namespace BadEcho.Game.Tests;

public class MatrixTests
{
    [Fact]
    public void Default_NotIdentity()
    {
        Assert.NotEqual(Matrix.Identity, default);
    }

    [Fact]
    public void Projection_MultiplyIdentity_IsProjection()
    {
        Matrix projection 
            = Matrix.CreateOrthographicOffCenter(0, 1920, 1080, 0, 0, -1);

        Matrix matrix = Matrix.Identity;

        matrix = Matrix.Multiply(matrix, projection);

        Assert.Equal(projection, matrix);
    }
}
