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
using Xunit;

namespace BadEcho.Game.Tests;

public class DistanceFieldFontTests : IClassFixture<ContentManagerFixture>
{
    private readonly ContentManager _content;

    public DistanceFieldFontTests(ContentManagerFixture contentFixture)
        => _content = contentFixture.Content;

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
}
