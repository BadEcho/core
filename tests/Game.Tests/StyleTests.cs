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

using BadEcho.Game.UI;
using Microsoft.Xna.Framework;
using Xunit;

namespace BadEcho.Game.Tests;

public class StyleTests
{
    private const int STYLE_WIDTH = 5;
    private const int STYLE_HEIGHT = 7;
    
    private readonly IVisual _background = new Brush(Color.DarkGray);
    private readonly IVisual _border = new Brush(Color.AliceBlue);

    [Fact]
    public void Initialize_NonsensicalExpression_ThrowsException()
    {
        Assert.Throws<ArgumentException>(
            () =>
            {
                Style<Button> _ =
                [
                    (b => b.Width + b.Height, STYLE_HEIGHT)
                ];
            });
    }

    [Fact]
    public void Initialize_WrongValueType_ThrowsException()
    {
        Assert.Throws<ArgumentException>(
            () =>
            {
                Style<Button> _ =
                [
                    (b => b.Width, "Hello")
                ];
            });
    }

    [Fact]
    public void Initialize_NonLocalProperty_ThrowsException()
    {
        Assert.Throws<ArgumentException>(
            () =>
            {
                var list = new List<int>{1};

                Style<Button> _ =
                [
                    (b => list.Count, 3)
                ];
            });
    }

    [Fact]
    public void Initialize_SamePropertyDifferentObject_ThrowsException()
    {
        Assert.Throws<ArgumentException>(
            () =>
            {
                var grid = new Grid();

                Style<Button> _ =
                [
                    (b => grid.Width, 3)
                ];
            });
    }

    [Fact]
    public void Initialize_MemberNotProperty_ThrowsException()
    {
        Assert.Throws<ArgumentException>(
            () =>
            {
                Style<Button> _ =
                [
                    (b => _background, "Hello")
                ];
            });
    }

    [Fact]
    public void ApplyTo_Button_MatchesStyle()
    {   // Normal control
        Style<Button> style =
        [
            (b => b.Width, STYLE_WIDTH),
            (b => b.Height, STYLE_HEIGHT),
            (b => b.Background, _background),
            (b => b.Border, _border)
        ];

        var button = new Button { Style = style };

        Assert.Equal(STYLE_WIDTH, button.Width);
        Assert.Equal(STYLE_HEIGHT, button.Height);
        Assert.Equal(_background, button.Background);
        Assert.Equal(_border, button.Border);
    }

    [Fact]
    public void ApplyTo_Grid_MatchesStyle()
    {   // Panel control
        Style<Grid> style =
        [
            (g => g.Width, STYLE_WIDTH),
            (g => g.Height, STYLE_HEIGHT),
            (g => g.Background, _background),
            (g => g.Border, _border)
        ];

        var grid = new Grid { Style = style };

        Assert.Equal(STYLE_WIDTH, grid.Width);
        Assert.Equal(STYLE_HEIGHT, grid.Height);
        Assert.Equal(_background, grid.Background);
        Assert.Equal(_border, grid.Border);
    }
}