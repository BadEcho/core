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
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides an adaptive connection to a render-target surface with its output scaled to fit the surface while also
/// maintaining a specific aspect ratio.
/// </summary>
public sealed class AspectMaintainedViewportConnector : ViewportConnector
{
    private readonly GameWindow _window;

    /// <summary>
    /// Initializes a new instance of the <see cref="AspectMaintainedViewportConnector"/> class.
    /// </summary>
    /// <param name="window">The game window to adapt to.</param>
    /// <param name="device">The graphics device to use for drawing onto the render-target surface.</param>
    /// <param name="virtualSize">The base resolution of content drawn onto the surface.</param>
    public AspectMaintainedViewportConnector(GameWindow window, GraphicsDevice device, Size virtualSize) 
        : base(device, virtualSize)
    {
        Require.NotNull(window, nameof(window));

        _window = window;

        AdjustToWindow();

        _window.ClientSizeChanged += HandleWindowClientSizeChanged;
    }

    /// <summary>
    /// Adjusts the content to fit within the confines of the window while maintaining the content's aspect
    /// ratio.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Because the size of the window very likely will present an aspect ratio different from that of the content,
    /// we manipulate the size of the viewport itself so, upon scaling the content to fit within it, the aspect
    /// ratio is the same.
    /// </para>
    /// <para>
    /// This of course will lead to either pillarboxing or letterboxing, but this is required if we want the aspect
    /// ratio to not be screwed up.
    /// </para>
    /// </remarks>
    private void AdjustToWindow()
    {
        float aspectRatio = (float) VirtualSize.Width / VirtualSize.Height;
        // Given the width of our window, we'll calculate what the height needs to be in order for our aspect ratio to
        // stay the same.
        int width = _window.ClientBounds.Width;
        int height = (int) (width / aspectRatio + 0.5f); // We add 0.5 to facilitate rounding, as the int cast will truncate.

        // If the height exceeds the available height of the window, we'll have to work with the width instead.
        if (height > _window.ClientBounds.Height)
        {
            height = _window.ClientBounds.Height;
            width = (int) (height * aspectRatio + 0.5f);
        }

        // We then calculate offsets for our viewport such that we're centered on our scaled content.
        int x = (_window.ClientBounds.Width - width) / 2;
        int y = (_window.ClientBounds.Height - height) / 2;

        Device.Viewport = new Viewport(x, y, width, height);
    }

    private void HandleWindowClientSizeChanged(object? sender, EventArgs e)
        => AdjustToWindow();
}
