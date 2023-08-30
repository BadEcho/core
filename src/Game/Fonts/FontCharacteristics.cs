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
/// TODO: comment, finish.
/// </summary>
public sealed class FontCharacteristics
{
    public float DistanceRange
    { get; init; }

    public float Width
    { get; init; }

    public float Height
    { get; init; }

    public float LineHeight
    { get; init; }

    public double Ascender
    { get; init; }

    public double Descender
    { get; init; }
}
