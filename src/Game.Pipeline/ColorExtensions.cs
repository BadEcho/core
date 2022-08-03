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

using System.Globalization;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.Pipeline;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to colors.
/// </summary>
internal static class ColorExtensions
{
    /// <summary>
    /// Converts a color hex code string into a corresponding <see cref="Color"/> value.
    /// </summary>
    /// <param name="colorHex">A color hex code string, with or without a leading '#' character.</param>
    /// <returns>A <see cref="Color"/> representation of <c>colorHex</c>.</returns>
    public static Color ToColor(this string colorHex)
    {
        Require.NotNullOrEmpty(colorHex, nameof(colorHex));

        colorHex = colorHex.Trim('#');

        int r = int.Parse(colorHex[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        int g = int.Parse(colorHex[2..4], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        int b = int.Parse(colorHex[4..6], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        int a = colorHex.Length > 6
            ? int.Parse(colorHex[6..8], NumberStyles.HexNumber, CultureInfo.InvariantCulture)
            : 255;

        return new Color(r, g, b, a);
    }
}
