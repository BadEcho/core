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

using BadEcho.Game.Fonts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xunit;

namespace BadEcho.Game.Tests;

public class DistanceFieldFontTests : IClassFixture<ContentManagerFixture>
{
    private readonly ContentManager _content;
    private readonly GraphicsDevice _device;

    public DistanceFieldFontTests(ContentManagerFixture contentFixture)
    {
        _content = contentFixture.Content;
        _device = contentFixture.Device;
    }

    [Fact]
    public void Load_Lato_NotNull()
    {
        DistanceFieldFont font = _content.Load<DistanceFieldFont>("Fonts\\Lato");

        Assert.NotNull(font);
        
        var gGlyph = font.FindGlyph('g');

        Assert.Equal(0.52f, gGlyph.Advance);
    }

    [Fact]
    public void GetNextAdvance_Lato_ReturnsValid()
    {
        DistanceFieldFont font = _content.Load<DistanceFieldFont>("Fonts\\Lato");

        Vector2 direction = new(1, 0);
        float scale = 1.0f;

        Vector2 expectedAdvance = direction * 0.5655f * scale;

        expectedAdvance += direction * -0.093f * scale;

        Assert.Equal(expectedAdvance, font.GetNextAdvance('F', 'J', direction, scale));
    }

    [Fact]
    public void AddText_Lato_ReturnsValid()
    {
        DistanceFieldFont font = _content.Load<DistanceFieldFont>("Fonts\\Lato");
        var modelData = new FontModelData(font, Color.White);

        modelData.AddText("Hi", new Vector2(20, 20), 32);

        var vertexBuffer = new VertexBuffer(_device,
                                            VertexPositionOutlinedColorTexture.VertexDeclaration,
                                            8,
                                            BufferUsage.None);

        modelData.LoadVertices(vertexBuffer);

        VertexPositionOutlinedColorTexture[] vertices = new VertexPositionOutlinedColorTexture[8];

        vertexBuffer.GetData(vertices);

        var expectedVertices = new VertexPositionOutlinedColorTexture[]
                               {
                                   new(new Vector3(21.716f, 21.7960014f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.286317557f, 0.8353041f)),
                                   new(new Vector3(42.716f, 21.7960014f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.3572635f, 0.8353041f)),
                                   new(new Vector3(21.716f, 46.796f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.286317557f, 0.9197635f)),
                                   new(new Vector3(42.716f, 46.796f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.3572635f, 0.9197635f)),
                                   new(new Vector3(45.022f, 21.3380013f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.9771959f, 0.0008445946f)),
                                   new(new Vector3(51.522f, 21.3380013f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.9991554f, 0.0008445946f)),
                                   new(new Vector3(45.022f, 46.838f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.9771959f, 0.08699324f)),
                                   new(new Vector3(51.522f, 46.838f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.9991554f, 0.08699324f))
                               };

        Assert.Collection(vertices,
                          t => Assert.Equal(expectedVertices[0], t),
                          t => Assert.Equal(expectedVertices[1], t),
                          t => Assert.Equal(expectedVertices[2], t),
                          t => Assert.Equal(expectedVertices[3], t),
                          t => Assert.Equal(expectedVertices[4], t),
                          t => Assert.Equal(expectedVertices[5], t),
                          t => Assert.Equal(expectedVertices[6], t),
                          t => Assert.Equal(expectedVertices[7], t));
    }

    [Fact]
    public void AddText_LatoKerning_ReturnsValid()
    { 
        DistanceFieldFont font = _content.Load<DistanceFieldFont>("Fonts\\Lato");
        // A kerning adjustment exists in Lato for L + v.
        var modelData = new FontModelData(font, Color.White);
        
        modelData.AddText("Lv", new Vector2(20, 20), 32);
        
        var vertexBuffer = new VertexBuffer(_device,
                                            VertexPositionOutlinedColorTexture.VertexDeclaration,
                                            8,
                                            BufferUsage.None);

        modelData.LoadVertices(vertexBuffer);

        VertexPositionOutlinedColorTexture[] vertices = new VertexPositionOutlinedColorTexture[8];

        vertexBuffer.GetData(vertices);

        var expectedVertices = new VertexPositionOutlinedColorTexture[]
                               {
                                   new(new Vector3(21.682f, 21.7960014f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.7440878f, 0.8724662f)),
                                   new(new Vector3(37.182f, 21.7960014f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.7964527f, 0.8724662f)),
                                   new(new Vector3(21.682f, 46.796f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.7440878f, 0.9569257f)),
                                   new(new Vector3(37.182f, 46.796f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.7964527f, 0.9569257f)),
                                   new(new Vector3(32.744f, 28.406002f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.504223f, 0.0008445946f)),
                                   new(new Vector3(50.744f, 28.406002f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.5650338f, 0.0008445946f)),
                                   new(new Vector3(32.744f, 46.906002f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.504223f, 0.0633446f)),
                                   new(new Vector3(50.744f, 46.906002f, 0f),
                                       Color.White,
                                       Color.Transparent,
                                       new Vector2(0.5650338f, 0.0633446f))
                               };

        Assert.Collection(vertices,
                          t => Assert.Equal(expectedVertices[0], t),
                          t => Assert.Equal(expectedVertices[1], t),
                          t => Assert.Equal(expectedVertices[2], t),
                          t => Assert.Equal(expectedVertices[3], t),
                          t => Assert.Equal(expectedVertices[4], t),
                          t => Assert.Equal(expectedVertices[5], t),
                          t => Assert.Equal(expectedVertices[6], t),
                          t => Assert.Equal(expectedVertices[7], t));
    }
}
