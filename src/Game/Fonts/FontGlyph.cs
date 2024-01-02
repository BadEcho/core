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

namespace BadEcho.Game.Fonts;

/// <summary>
/// Provides a typographic representation for a unicode character associated with a font.
/// </summary>
public class FontGlyph
{
    /// <summary>
    /// Gets the unicode character that this glyph represents.
    /// </summary>
    public char Character
    { get; set; }

    /// <summary>
    /// Gets the advance width of this glyph, which is the distance between the origin of this glyph to the origin of the
    /// next glyph.
    /// </summary>
    /// <remarks>
    /// The advance width is used to know where to place the next glyph; it is the distance between two successive pen positions.
    /// </remarks>
    public float Advance
    { get; set; }
    
    /// <summary>
    /// Gets the bounding rectangle of the region that the glyph is drawn to, relative to the baseline cursor.
    /// </summary>
    public RectangleF PlaneBounds
    { get; set; }

    /// <summary>
    /// Gets the bounding rectangle for this glyph within the font atlas's texture.
    /// </summary>
    public RectangleF AtlasBounds
    { get; set; }
}
