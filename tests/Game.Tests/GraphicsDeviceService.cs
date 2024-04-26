//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
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
    /// <inheritdoc />
    public event EventHandler<EventArgs>? DeviceCreated
    {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler<EventArgs>? DeviceDisposing
    {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler<EventArgs>? DeviceReset
    {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler<EventArgs>? DeviceResetting
    {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public GraphicsDevice GraphicsDevice 
    { get; } = new(GraphicsAdapter.DefaultAdapter,
                   GraphicsProfile.Reach,
                   new PresentationParameters { BackBufferWidth = 1920, BackBufferHeight = 1080 });
}
