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

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Atlases;

/// <summary>
/// Provides a reader of raw texture atlas content from the content pipeline.
/// </summary>
public sealed class TextureAtlasReader : ContentTypeReader<TextureAtlas>
{
    /// <inheritdoc />
    protected override TextureAtlas Read(ContentReader input, TextureAtlas existingInstance)
    {
        Require.NotNull(input, nameof(input));

        var texture = input.ReadExternalReference<Texture2D>();
        var atlas = new TextureAtlas(texture);

        var regionsToRead = input.ReadInt32();
        
        while (regionsToRead > 0)
        {
            var isNineSliced = input.ReadBoolean();
            var name = input.ReadString();
            var sourceArea = input.ReadRectangle();

            if (isNineSliced)
            {
                var nineSliceArea = input.ReadRectangle();
                var padding = new Thickness(nineSliceArea.X,
                                            nineSliceArea.Y,
                                            sourceArea.Width - nineSliceArea.Width - nineSliceArea.X,
                                            sourceArea.Height - nineSliceArea.Height - nineSliceArea.Y);

                atlas.AddRegion(name, sourceArea, padding);
            }
            else
                atlas.AddRegion(name, sourceArea);

            regionsToRead--;
        }

        return atlas;
    }
}
