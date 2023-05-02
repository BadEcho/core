﻿//-----------------------------------------------------------------------
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
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a brush that will paint a solid color on the screen when drawn.
/// </summary>
public sealed class Brush : IVisual, IDisposable
{
    private readonly Texture2D _palette;

    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Brush"/> class.
    /// </summary>
    /// <param name="device">The graphics device to use for generating the palette texture.</param>
    public Brush(GraphicsDevice device)
    {
        Require.NotNull(device, nameof(device));

        _palette = new Texture2D(device, 1, 1);

        _palette.SetData(new[] { Color.White });
    }

    /// <inheritdoc />
    public void Draw(SpriteBatch spriteBatch, Rectangle targetArea, Color color)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));
        
        spriteBatch.Draw(_palette, targetArea, color);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _palette.Dispose();

        _disposed = true;
    }
}
