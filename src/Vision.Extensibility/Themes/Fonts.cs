//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using BadEcho.Presentation.Markup;

namespace BadEcho.Omnified.Vision.Extensibility.Themes;

/// <summary>
/// Provides resource keys for font-related resources used by Vision and its modules.
/// </summary>
public static class Fonts
{
    private static readonly FenestraKeyProvider _Provider = new(typeof(Fonts));

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