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

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a panel that arranges child controls into a single line that can be oriented horizontally or vertically.
/// </summary>
public sealed class StackPanel : Panel
{
    private Orientation _orientation;

    /// <summary>
    /// Gets or sets the dimension by which child controls are stacked.
    /// </summary>
    public Orientation Orientation
    {
        get => _orientation;
        set => RemeasureIfChanged(ref _orientation, value);
    }

    /// <inheritdoc />
    protected override Size MeasureCore(Size availableSize)
    {
        bool isHorizontal = Orientation == Orientation.Horizontal;
        var layoutAllocation = new Size(isHorizontal ? int.MaxValue : availableSize.Width,
                                        isHorizontal ? availableSize.Height : int.MaxValue);
        var desiredSize = new Size();

        foreach (Control child in Children.Where(c => c.IsVisible))
        {
            child.Measure(layoutAllocation);
            Size childDesiredSize = child.DesiredSize;

            if (isHorizontal)
            {
                desiredSize = new Size(desiredSize.Width + childDesiredSize.Width,
                                       Math.Max(desiredSize.Height, childDesiredSize.Height));
            }
            else
            {
                desiredSize = new Size(Math.Max(desiredSize.Width, childDesiredSize.Width),
                                       desiredSize.Height + childDesiredSize.Height);
            }
        }

        return desiredSize;
    }

    /// <inheritdoc/>
    protected override void ArrangeCore()
    {
        base.ArrangeCore();

        bool isHorizontal = Orientation == Orientation.Horizontal;
        
        int nextChildPosition = isHorizontal ? ContentBounds.X : ContentBounds.Y;

        foreach (Control child in Children.Where(c => c.IsVisible))
        {
            Rectangle effectiveChildArea;

            if (isHorizontal)
            {
                effectiveChildArea = new Rectangle(nextChildPosition,
                                                   ContentBounds.Y,
                                                   child.DesiredSize.Width,
                                                   Math.Max(DesiredSize.Height, child.DesiredSize.Height));   

                nextChildPosition += child.DesiredSize.Width;
            }
            else
            {
                effectiveChildArea = new Rectangle(ContentBounds.X,
                                                   nextChildPosition,
                                                   Math.Max(DesiredSize.Width, child.DesiredSize.Width),
                                                   child.DesiredSize.Height);

                nextChildPosition += child.DesiredSize.Height;
            }

            child.Arrange(effectiveChildArea);
        }
    }
}
