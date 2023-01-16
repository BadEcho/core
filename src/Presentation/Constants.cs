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

namespace BadEcho.Presentation;

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
    /// The namespace used for all Bad Echo Presentation framework XML namespace declarations.
    /// </summary>
    internal const string Namespace = "http://schemas.badecho.com/presentation/2022/02/xaml";
}