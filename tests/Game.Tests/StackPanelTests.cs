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

using BadEcho.Game.Fonts;
using BadEcho.Game.UI;
using Microsoft.Xna.Framework;
using Xunit;

namespace BadEcho.Game.Tests;

public class StackPanelTests : IClassFixture<ContentManagerFixture>
{
    private readonly StackPanel _stackPanel;
    private readonly Label _firstLabel;
    private readonly Label _secondLabel;

    public StackPanelTests(ContentManagerFixture contentFixture)
    {
        var font = contentFixture.Content.Load<DistanceFieldFont>("Fonts\\Lato");

        _firstLabel = new Label
                      {
                          Background = new Brush(Color.Gray),
                          Border = new Brush(Color.Aqua),
                          BorderThickness = new Thickness(1),
                          FontColor = Color.White,
                          FontSize = 21,
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
                           FontColor = Color.White,
                           FontSize = 21,
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

        Assert.Equal(new Rectangle(0, 0, 324, 50), _stackPanel.LayoutBounds);
        Assert.Equal(new Rectangle(0, 0, 324, 50), _stackPanel.ContentBounds);

        Assert.Equal(new Rectangle(0, 0, 157, 45), _firstLabel.LayoutBounds);
        Assert.Equal(new Rectangle(1, 1, 155, 43), _firstLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(11, 11, 135, 23), _firstLabel.ContentBounds);

        Assert.Equal(new Rectangle(157, 0, 167, 50), _secondLabel.LayoutBounds);
        Assert.Equal(new Rectangle(168, 1, 155, 48), _secondLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(178, 11, 135, 28), _secondLabel.ContentBounds);
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

        Assert.Equal(new Rectangle(0, 0, 334, 60), _stackPanel.LayoutBounds);
        Assert.Equal(new Rectangle(10, 10, 324, 50), _stackPanel.ContentBounds);

        Assert.Equal(new Rectangle(10, 10, 157, 45), _firstLabel.LayoutBounds);
        Assert.Equal(new Rectangle(11, 11, 155, 43), _firstLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(21, 21, 135, 23), _firstLabel.ContentBounds);

        Assert.Equal(new Rectangle(167, 10, 167, 50), _secondLabel.LayoutBounds);
        Assert.Equal(new Rectangle(178, 11, 155, 48), _secondLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(188, 21, 135, 28), _secondLabel.ContentBounds);
    }

    [Fact]
    public void MeasureArrange_CenterTopAlignment_ValidLayoutBounds()
    {
        _stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
        _stackPanel.VerticalAlignment = VerticalAlignment.Top;

        var screenBounds = new Rectangle(0, 0, 1920, 1080);

        _stackPanel.Measure(screenBounds.Size);
        _stackPanel.Arrange(screenBounds);

        Assert.Equal(new Rectangle(798, 0, 324, 50), _stackPanel.LayoutBounds);
        Assert.Equal(new Rectangle(798, 0, 324, 50), _stackPanel.ContentBounds);

        Assert.Equal(new Rectangle(798, 0, 157, 45), _firstLabel.LayoutBounds);
        Assert.Equal(new Rectangle(799, 1, 155, 43), _firstLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(809, 11, 135, 23), _firstLabel.ContentBounds);

        Assert.Equal(new Rectangle(955, 0, 167, 50), _secondLabel.LayoutBounds);
        Assert.Equal(new Rectangle(966, 1, 155, 48), _secondLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(976, 11, 135, 28), _secondLabel.ContentBounds);
    }

    [Fact]
    public void MeasureArrange_StretchTopAlignment_ValidLayoutBounds()
    {
        _stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
        _stackPanel.VerticalAlignment = VerticalAlignment.Top;

        var screenBounds = new Rectangle(0, 0, 1920, 1080);

        _stackPanel.Measure(screenBounds.Size);
        _stackPanel.Arrange(screenBounds);

        Assert.Equal(new Rectangle(0, 0, 1920, 50), _stackPanel.LayoutBounds);
        Assert.Equal(new Rectangle(0, 0, 1920, 50), _stackPanel.ContentBounds);

        Assert.Equal(new Rectangle(0, 0, 157, 45), _firstLabel.LayoutBounds);
        Assert.Equal(new Rectangle(1, 1, 155, 43), _firstLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(11, 11, 135, 23), _firstLabel.ContentBounds);

        Assert.Equal(new Rectangle(157, 0, 167, 50), _secondLabel.LayoutBounds);
        Assert.Equal(new Rectangle(168, 1, 155, 48), _secondLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(178, 11, 135, 28), _secondLabel.ContentBounds);
    }

    [Fact]
    public void MeasureArrange_CenterCenterAlignment_ValidLayoutBounds()
    {
        _stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
        _stackPanel.VerticalAlignment = VerticalAlignment.Center;

        var screenBounds = new Rectangle(0, 0, 1920, 1080);

        _stackPanel.Measure(screenBounds.Size);
        _stackPanel.Arrange(screenBounds);

        Assert.Equal(new Rectangle(798, 515, 324, 50), _stackPanel.LayoutBounds);
        Assert.Equal(new Rectangle(798, 515, 324, 50), _stackPanel.ContentBounds);

        Assert.Equal(new Rectangle(798, 515, 157, 45), _firstLabel.LayoutBounds);
        Assert.Equal(new Rectangle(799, 516, 155, 43), _firstLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(809, 526, 135, 23), _firstLabel.ContentBounds);

        Assert.Equal(new Rectangle(955, 515, 167, 50), _secondLabel.LayoutBounds);
        Assert.Equal(new Rectangle(966, 516, 155, 48), _secondLabel.BackgroundBounds);
        Assert.Equal(new Rectangle(976, 526, 135, 28), _secondLabel.ContentBounds);
    }
}
