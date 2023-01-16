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
/// Provides resource keys for <see cref="Color"/> resources used by Vision and its modules.
/// </summary>
public static class Colors
{
    private static readonly BadEchoKeyProvider _Provider = new(typeof(Colors));

    /// <summary>
    /// The standard color for standard text appearing in Vision.
    /// </summary>
    public static readonly ResourceKey TextColorKey =
        _Provider.CreateKey(nameof(TextColorKey));

    /// <summary>
    /// The color for text emphasizing a critical event in Vision.
    /// </summary>
    public static readonly ResourceKey CriticalTextColorKey =
        _Provider.CreateKey(nameof(CriticalTextColorKey));

    /// <summary>
    /// The color used for outlining standard text appearing in Vision.
    /// </summary>
    public static readonly ResourceKey OutlineColorKey =
        _Provider.CreateKey(nameof(OutlineColorKey));

    /// <summary>
    /// The color representing the halfway point between the color used for outlining and its opposite
    /// on the color spectrum.
    /// </summary>
    public static readonly ResourceKey OutlineMidpointColorKey =
        _Provider.CreateKey(nameof(OutlineMidpointColorKey));

    /// <summary>
    /// The color used for fading out the outlines of standard text appearing in Vision.
    /// </summary>
    public static readonly ResourceKey FadingOutlineColorKey =
        _Provider.CreateKey(nameof(FadingOutlineColorKey));
}