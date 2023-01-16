//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Media;
using BadEcho.Presentation.Markup;

namespace BadEcho.Vision.Extensibility.Themes;

/// <summary>
/// Provides resource keys for <see cref="Brush"/> resources used by Vision and its modules.
/// </summary>
public static class Brushes
{
    private static readonly BadEchoKeyProvider _Provider = new(typeof(Brushes));

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