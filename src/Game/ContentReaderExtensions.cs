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
using Microsoft.Xna.Framework;

namespace BadEcho.Game;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to writing assets to the content pipeline.
/// </summary>
internal static class ContentReaderExtensions
{
    /// <summary>
    /// Reads a <see cref="Rectangle"/> value from the content pipeline.
    /// </summary>
    /// <param name="reader">The current content reader.</param>
    /// <returns>A <see cref="Rectangle"/> value read from <c>reader</c>.</returns>
    public static Rectangle ReadRectangle(this ContentReader reader)
    {
        Require.NotNull(reader, nameof(reader));

        var x = reader.ReadInt32();
        var y = reader.ReadInt32();
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();

        return new Rectangle(x, y, width, height);
    }
}
