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

using System.ComponentModel.Design;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Tests;

/// <summary>
/// Provides a shared context between test instances requiring the use of a content manager.
/// </summary>
public sealed class ContentManagerFixture : IDisposable
{
    private bool _disposed;

    public ContentManagerFixture()
    {
        var services = new ServiceContainer();
        var graphicsService = new GraphicsDeviceService();

        services.AddService(typeof(IGraphicsDeviceService), graphicsService);

        Content = new ContentManager(services, "Content");
        Device = graphicsService.GraphicsDevice;
    }

    public ContentManager Content
    { get; }

    public GraphicsDevice Device
    { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        Content.Dispose();

        _disposed = true;
    }
}
