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

namespace BadEcho.Fenestra;

/// <summary>
/// Provides a set of general UI-related constants.
/// </summary>
internal static class Constants
{
    /// <summary>
    /// The maximum number of units a line of text of infinite width is allowed.
    /// </summary>
    internal const double InfiniteLineWidth
        = 0x3FFFFFFE / (28800.0 / 96);

    /// <summary>
    /// The namespace used for all Fenestra framework XML namespace declarations.
    /// </summary>
    internal const string Namespace = "http://schemas.badecho.com/fenestra/2021/02/xaml";
}