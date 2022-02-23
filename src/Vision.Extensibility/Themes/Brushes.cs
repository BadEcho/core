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
using System.Windows.Media;
using BadEcho.Presentation.Markup;

namespace BadEcho.Omnified.Vision.Extensibility.Themes;

/// <summary>
/// Provides resource keys for <see cref="Brush"/> resources used by Vision and its modules.
/// </summary>
public static class Brushes
{
    private static readonly FenestraKeyProvider _Provider = new(typeof(Brushes));

    /// <summary>
    /// The foreground brush used for standard text appearing in Vision.
    /// </summary>
    public static readonly ResourceKey TextForegroundKey =
        _Provider.CreateKey(nameof(TextForegroundKey));

    /// <summary>
    /// The foreground brush used for text emphasizing a critical event in Vision.
    /// </summary>
    public static readonly ResourceKey CriticalTextForegroundKey =
        _Provider.CreateKey(nameof(CriticalTextForegroundKey));

    /// <summary>
    /// The brush used to outline standard text appearing in Vision.
    /// </summary>
    public static readonly ResourceKey OutlinedTextStrokeKey =
        _Provider.CreateKey(nameof(OutlinedTextStrokeKey));

    /// <summary>
    /// The brush used to give a faded edge to outlined standard text appearing in Vision.
    /// </summary>
    public static readonly ResourceKey FadedOutlinedTextStrokeKey =
        _Provider.CreateKey(nameof(FadedOutlinedTextStrokeKey));
}