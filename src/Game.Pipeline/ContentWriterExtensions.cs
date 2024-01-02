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

using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.Pipeline;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to writing assets to the content pipeline.
/// </summary>
internal static class ContentWriterExtensions
{
    /// <summary>
    /// Writes a <see cref="Rectangle"/> value to the content pipeline.
    /// </summary>
    /// <param name="writer">The current content writer.</param>
    /// <param name="value">A rectangle value to write.</param>
    public static void Write(this ContentWriter writer, Rectangle value)
    {
        Require.NotNull(writer, nameof(writer));

        writer.Write(value.X);
        writer.Write(value.Y);
        writer.Write(value.Width);
        writer.Write(value.Height);
    }
}
