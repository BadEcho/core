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

using BadEcho.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xunit;

namespace BadEcho.Game.Tests;

public class StackPanelTests : IClassFixture<ContentManagerFixture>
{
    private readonly StackPanel _stackPanel;
    private readonly Label _firstLabel;
    private readonly Label _secondLabel;

    public StackPanelTests(ContentManagerFixture contentFixture)
    {
        var font = contentFixture.Content.Load<SpriteFont>("Fonts\\Lato");

        _firstLabel = new Label
                      {
                          Background = new Brush(Color.Gray),
                          Border = new Brush(Color.Aqua),
                          BorderThickness = new Thickness(1),
                          Font = font,
                          Text = "Hello there",
                          VerticalAlignment = VerticalAlignment.Top,
                          Padding = new Thickness(10)
                      };

        _secondLabel = new Label
                       {
                           Background = new Brush(Color.SteelBlue),
                           Border = new Brush(Color.Yellow),
                           BorderThickness = new Thickness(1),
                           Font = font,
                           Text = "What's up?",
                           VerticalAlignment = VerticalAlignment.Top,
                           Padding = new Thickness(10),
                           Margin = new Thickness(10, 0, 0, 0)
                       };

        _stackPanel = new StackPanel
                      {
                          Background = new Brush(Color.Black)
                      };

        _stackPanel.AddChild(_firstLabel);
        _stackPanel.AddChild(_secondLabel);
    }

    [Fact]
    public void MeasureArrange_LeftTopAlignment_ValidLayoutBounds()
    {
        _stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
        _stackPanel.VerticalAlignment = VerticalAlignment.Top;

        var screenBounds = new Rectangle(0, 0, 1920, 1080);

        _stackPanel.Measure(screenBounds.Size);
        _stackPanel.Arrange(screenBounds);

        Assert.Equal(new Rectangle(0, 0, 369, 60), _stackPanel.LayoutBounds);
        Assert.Equal(new Rectangle(0, 0, 369, 60), _stackPanel.ContentBounds);

        Assert.Equal(new Rectangle(0, 0, 180, 60), _firstLabel.LayoutBounds);
        Assert.Equal(new Rectangle(1, 1, 178, 58), _firstLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(11, 11, 158, 38), _firstLabel.ContentBounds);

        Assert.Equal(new Rectangle(180, 0, 189, 60), _secondLabel.LayoutBounds);
        Assert.Equal(new Rectangle(191, 1, 177, 58), _secondLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(201, 11, 157, 38), _secondLabel.ContentBounds);
    }

    [Fact]
    public void MeasureArrange_TopLeftMarginLeftTopAlignment_ValidLayoutBounds()
    {
        _stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
        _stackPanel.VerticalAlignment = VerticalAlignment.Top;
        _stackPanel.Margin = new Thickness(10, 10, 0, 0);

        var screenBounds = new Rectangle(0, 0, 1920, 1080);

        _stackPanel.Measure(screenBounds.Size);
        _stackPanel.Arrange(screenBounds);

        Assert.Equal(new Rectangle(0, 0, 379, 70), _stackPanel.LayoutBounds);
        Assert.Equal(new Rectangle(10, 10, 369, 60), _stackPanel.ContentBounds);

        Assert.Equal(new Rectangle(10, 10, 180, 60), _firstLabel.LayoutBounds);
        Assert.Equal(new Rectangle(11, 11, 178, 58), _firstLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(21, 21, 158, 38), _firstLabel.ContentBounds);

        Assert.Equal(new Rectangle(190, 10, 189, 60), _secondLabel.LayoutBounds);
        Assert.Equal(new Rectangle(201, 11, 177, 58), _secondLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(211, 21, 157, 38), _secondLabel.ContentBounds);
    }

    [Fact]
    public void MeasureArrange_CenterTopAlignment_ValidLayoutBounds()
    {
        _stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
        _stackPanel.VerticalAlignment = VerticalAlignment.Top;

        var screenBounds = new Rectangle(0, 0, 1920, 1080);

        _stackPanel.Measure(screenBounds.Size);
        _stackPanel.Arrange(screenBounds);

        Assert.Equal(new Rectangle(775, 0, 369, 60), _stackPanel.LayoutBounds);
        Assert.Equal(new Rectangle(775, 0, 369, 60), _stackPanel.ContentBounds);

        Assert.Equal(new Rectangle(775, 0, 180, 60), _firstLabel.LayoutBounds);
        Assert.Equal(new Rectangle(776, 1, 178, 58), _firstLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(786, 11, 158, 38), _firstLabel.ContentBounds);

        Assert.Equal(new Rectangle(955, 0, 189, 60), _secondLabel.LayoutBounds);
        Assert.Equal(new Rectangle(966, 1, 177, 58), _secondLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(976, 11, 157, 38), _secondLabel.ContentBounds);
    }

    [Fact]
    public void MeasureArrange_StretchTopAlignment_ValidLayoutBounds()
    {
        _stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
        _stackPanel.VerticalAlignment = VerticalAlignment.Top;

        var screenBounds = new Rectangle(0, 0, 1920, 1080);

        _stackPanel.Measure(screenBounds.Size);
        _stackPanel.Arrange(screenBounds);

        Assert.Equal(new Rectangle(0, 0, 1920, 60), _stackPanel.LayoutBounds);
        Assert.Equal(new Rectangle(0, 0, 1920, 60), _stackPanel.ContentBounds);

        Assert.Equal(new Rectangle(0, 0, 180, 60), _firstLabel.LayoutBounds);
        Assert.Equal(new Rectangle(1, 1, 178, 58), _firstLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(11, 11, 158, 38), _firstLabel.ContentBounds);

        Assert.Equal(new Rectangle(180, 0, 189, 60), _secondLabel.LayoutBounds);
        Assert.Equal(new Rectangle(191, 1, 177, 58), _secondLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(201, 11, 157, 38), _secondLabel.ContentBounds);
    }

    [Fact]
    public void MeasureArrange_CenterCenterAlignment_ValidLayoutBounds()
    {
        _stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
        _stackPanel.VerticalAlignment = VerticalAlignment.Center;

        var screenBounds = new Rectangle(0, 0, 1920, 1080);

        _stackPanel.Measure(screenBounds.Size);
        _stackPanel.Arrange(screenBounds);

        Assert.Equal(new Rectangle(775, 510, 369, 60), _stackPanel.LayoutBounds);
        Assert.Equal(new Rectangle(775, 510, 369, 60), _stackPanel.ContentBounds);

        Assert.Equal(new Rectangle(775, 510, 180, 60), _firstLabel.LayoutBounds);
        Assert.Equal(new Rectangle(776, 511, 178, 58), _firstLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(786, 521, 158, 38), _firstLabel.ContentBounds);

        Assert.Equal(new Rectangle(955, 510, 189, 60), _secondLabel.LayoutBounds);
        Assert.Equal(new Rectangle(966, 511, 177, 58), _secondLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(976, 521, 157, 38), _secondLabel.ContentBounds);
    }
}
