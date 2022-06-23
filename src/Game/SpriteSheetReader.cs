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

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a reader of raw sprite sheet content from the content pipeline.
/// </summary>
public sealed class SpriteSheetReader : ContentTypeReader<SpriteSheet>
{
    /// <inheritdoc />
    protected override SpriteSheet Read(ContentReader input, SpriteSheet existingInstance)
    {
        Require.NotNull(input, nameof(input));

        var texture = input.ReadExternalReference<Texture2D>();
        var rows = input.ReadInt32();
        var columns = input.ReadInt32();
        var rowUp = input.ReadInt32();
        var rowDown = input.ReadInt32();
        var rowLeft = input.ReadInt32();
        var rowRight = input.ReadInt32();
        var rowInitial = input.ReadInt32();
        
        var spriteSheet = new SpriteSheet(texture, columns, rows);

        spriteSheet.AddDirection(MovementDirection.None, rowInitial);
        spriteSheet.AddDirection(MovementDirection.Up, rowUp);
        spriteSheet.AddDirection(MovementDirection.Down, rowDown);
        spriteSheet.AddDirection(MovementDirection.Left, rowLeft);
        spriteSheet.AddDirection(MovementDirection.Right, rowRight);
        
        return spriteSheet;
    }
}
