//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides an adaptive connection to a render-target surface with its output scaled to fit the surface.
/// </summary>
/// <remarks>
/// <para>
/// A viewport connector allows for the manipulation of graphics from when its drawn to when it eventually
/// is rendered on an output surface. A viewport connector makes use of two sizing concepts: the physical
/// resolution of the surface, and the virtual resolution of the content.
/// </para>
/// <para>
/// The physical resolution of the surface is, by default, the number of actual pixels of screen-space composing
/// our render-target. This will either be the total number of pixels for a monitor, if fullscreen, or that of the
/// client area of a particular window.
/// </para>
/// <para>
/// The virtual resolution is size that drawn content is treated as having naturally. During rendering, the content
/// will be scaled to fit the physical resolution of the surface, in a manner that depends on the particular type
/// of connector.
/// </para>
/// <para>
/// With the base type of <see cref="ViewportConnector"/>, drawn content is stretched so it completely fills the
/// physical surface of the render-target surface. This will result in the aspect ratio of the source content not
/// being maintained if the physical resolution's is not the same; however, the content will completely fill the
/// physical surface, without any pillarboxing or letterboxing required.
/// </para>
/// <para>
/// If no virtual resolution is specified during object initialization, then the initial size of the physical surface
/// is treated as the virtual resolution.
/// </para>
/// </remarks>
public class ViewportConnector
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewportConnector"/> class.
    /// </summary>
    /// <param name="device">The graphics device to use for drawing onto the render-target surface.</param>
    /// <param name="virtualSize">The base resolution of content drawn onto the surface.</param>
    public ViewportConnector(GraphicsDevice device, Size virtualSize)
        : this(device)
    {
        VirtualSize = virtualSize;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewportConnector"/> class.
    /// </summary>
    /// <param name="device">The graphics device to use for drawing onto the render-target surface.</param>
    /// <remarks>
    /// Not specifying the virtual resolution will result in <see cref="VirtualSize"/> defaulting to the initial
    /// value for <see cref="Size"/>, which will be whatever the physical surface's size is upon object initialization.
    /// </remarks>
    public ViewportConnector(GraphicsDevice device)
    {
        Require.NotNull(device, nameof(device));

        Device = device;
        VirtualSize = Size;
    }

    /// <summary>
    /// Gets the bounding rectangle of the region of the render-target surface that will receive draw calls.
    /// </summary>
    public Rectangle BoundingArea 
        => new(0, 0, Size.Width, Size.Height);

    /// <summary>
    /// Gets the physical resolution of the render-target surface.
    /// </summary>
    public Size Size
        => new(Device.Viewport.Width, Device.Viewport.Height);

    /// <summary>
    /// Gets the base resolution of drawn content, scaled to fit the actual surface <see cref="Size"/> during rendering.
    /// </summary>
    public Size VirtualSize
    { get; }

    /// <summary>
    /// Gets the graphics device to use for drawing onto the render-target surface.
    /// </summary>
    public GraphicsDevice Device
    { get; }

    /// <summary>
    /// Gets an adapted scaling matrix used to transform coordinates to be in accordance with this connector.
    /// </summary>
    /// <returns>A scaling <see cref="Matrix"/> to use to transform coordinates.</returns>
    /// <remarks>
    /// The matrix returned here is a scale matrix with its dimensional scales equal to the ratio between physical and
    /// virtual resolutions. This should be used during the transformation of world space into view space by consumers
    /// of this viewport connector. 
    /// </remarks>
    public Matrix GetScaleMatrix()
    {
        if (VirtualSize == Size.Empty)
            return Matrix.Identity;

        var widthScale = (float) Size.Width / VirtualSize.Width;
        var heightScale = (float) Size.Height / VirtualSize.Height;

        return Matrix.CreateScale(widthScale, heightScale, 1f);
    }
}
