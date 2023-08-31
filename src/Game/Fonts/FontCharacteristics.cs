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

namespace BadEcho.Game.Fonts;

/// <summary>
/// Provides various characteristics and metrics for a font.
/// </summary>
public sealed class FontCharacteristics
{
    /// <summary>
    /// Gets the distance field range in pixels; this is how far the distance field extends beyond the glyphs.
    /// </summary>
    public float DistanceRange
    { get; init; }

    /// <summary>
    /// Gets the width of the generated font atlas's texture.
    /// </summary>
    public float Width
    { get; init; }

    /// <summary>
    /// Gets the height of the generated font atlas's texture.
    /// </summary>
    public float Height
    { get; init; }

    /// <summary>
    /// Gets the vertical distance between one line of type to the baseline of the next.
    /// </summary>
    public float LineHeight
    { get; init; }

    /// <summary>
    /// Gets the vertical distance between the typeface's baseline and the ascender line.
    /// </summary>
    /// <remarks>
    /// Ascenders are upward strokes found in certain (and typically lowercase) letters that extend
    /// beyond the cap height (the height of the typeface's uppercase letters, measured from the baseline
    /// to the top of its flat-topped glyphs). The ascender line depicts the height of the highest ascender.
    /// </remarks>
    public double Ascender
    { get; init; }

    /// <summary>
    /// Gets the vertical distance between the typeface's baseline and the descender line.
    /// </summary>
    /// <remarks>
    /// Descenders are downward strokes found in certain (and typically lowercase) letters that extend
    /// beyond the baseline. The descender line depicts the height of the lowest descender.
    /// </remarks>
    public double Descender
    { get; init; }
}
