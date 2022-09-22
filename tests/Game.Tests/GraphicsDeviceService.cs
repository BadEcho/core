//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Tests;

/// <summary>
/// Provides a service for graphics devices used in unit tests.
/// </summary>
internal sealed class GraphicsDeviceService : IGraphicsDeviceService
{
    public GraphicsDeviceService()
    {
        GraphicsDevice 
            = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, new PresentationParameters());
    }

    /// <inheritdoc />
    public event EventHandler<EventArgs>? DeviceCreated;

    /// <inheritdoc />
    public event EventHandler<EventArgs>? DeviceDisposing;

    /// <inheritdoc />
    public event EventHandler<EventArgs>? DeviceReset;

    /// <inheritdoc />
    public event EventHandler<EventArgs>? DeviceResetting;

    /// <inheritdoc />
    public GraphicsDevice GraphicsDevice 
    { get; }
}
