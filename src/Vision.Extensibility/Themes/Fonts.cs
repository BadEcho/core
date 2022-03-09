//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using BadEcho.Presentation.Markup;

namespace BadEcho.Vision.Extensibility.Themes;

/// <summary>
/// Provides resource keys for font-related resources used by Vision and its modules.
/// </summary>
public static class Fonts
{
    private static readonly BadEchoKeyProvider _Provider = new(typeof(Fonts));

    /// <summary>
    /// The standard font family for text appearing in Vision.
    /// </summary>
    public static readonly ResourceKey FontFamilyKey =
        _Provider.CreateKey(nameof(FontFamilyKey));

    /// <summary>
    /// The font family for the Vision application's title text.
    /// </summary>
    public static readonly ResourceKey VisionTitleFontFamilyKey =
        _Provider.CreateKey(nameof(VisionTitleFontFamilyKey));
}