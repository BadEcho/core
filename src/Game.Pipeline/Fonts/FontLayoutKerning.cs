//-----------------------------------------------------------------------
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

namespace BadEcho.Game.Pipeline.Fonts;

/// <summary>
/// Provides a class for holding deserialized multi-channel signed distance field font kerning information.
/// </summary>
internal sealed class FontLayoutKerning
{
    /// <summary>
    /// Gets the first unicode character in the pair.
    /// </summary>
    public char Unicode1
    { get; init; }

    /// <summary>
    /// Gets the second unicode character in the pair.
    /// </summary>
    public char Unicode2
    { get; init; }

    /// <summary>
    /// Gets the advance width between <see cref="Unicode1"/> and <see cref="Unicode2"/>.
    /// </summary>
    public float Advance
    { get; init; }
}
