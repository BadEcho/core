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
/// Provides a typographic representation for a unicode character associated with a font.
/// TODO: Comment, finish.
/// </summary>
public class FontGlyph
{
    public char Character
    { get; set; }

    public double Advance
    { get; set; }
    
    public RectangleF PlaneBounds
    { get; set; }

    public RectangleF AtlasBounds
    { get; set; }
}
