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

namespace BadEcho.Extensions;

/// <summary>
/// Provides a set of static methods to aid in all matters numeric. 
/// </summary>
public static class NumberExtensions
{
    private const double MACHINE_DOUBLE_EPSILON = 2.2204460492503131e-016;

    /// <summary>
    /// Determines if this floating-point number equals another floating-point number while taking into account the upper bound
    /// on relative errors due to rounding (aka the machine epsilon).
    /// </summary>
    /// <param name="source">The current floating-point number.</param>
    /// <param name="other">The floating-point number to compare with the current number.</param>
    /// <returns>True if <c>source</c> and <c>other</c> are approximately equal; otherwise, false.</returns>
    public static bool ApproximatelyEquals(this double source, double other)
    {
        if (double.IsPositiveInfinity(source))
            return double.IsPositiveInfinity(other);

        if (double.IsNegativeInfinity(source))
            return double.IsNegativeInfinity(other);

        return Math.Abs(source - other) / (Math.Abs(source) + Math.Abs(other) + 1) < MACHINE_DOUBLE_EPSILON;
    }
}