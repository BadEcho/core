//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;

namespace BadEcho.Odin.Extensions;

/// <summary>
/// Provides a set of static methods intended to simplify string use and manipulation.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Replaces the format items in this string with the string representation of corresponding objects in
    /// the specified array using current-culture formatting.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An object array that contains zero or more objects for format.</param>
    /// <returns>
    /// A copy of <c>format</c> in which the format items have been replaced by the string representation of corresponding objects
    /// in <c>args</c> using current-culture formatting.
    /// </returns>
    public static string CulturedFormat(this string format, params object[] args) 
        => string.Format(CultureInfo.CurrentCulture, format, args);

    /// <summary>
    /// Replaces the format items in this string with the string representation of corresponding objects in
    /// the specified array using invariant-culture formatting.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An object array that contains zero or more objects for format.</param>
    /// <returns>
    /// A copy of <c>format</c> in which the format items have been replaced by the string representation of corresponding objects
    /// in <c>args</c> using invariant-culture formatting.
    /// </returns>
    public static string InvariantFormat(this string format, params object[] args) 
        => string.Format(CultureInfo.InvariantCulture, format, args);
}