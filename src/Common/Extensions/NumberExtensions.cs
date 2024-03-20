//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Extensions;

/// <summary>
/// Provides a set of static methods to aid in all matters numeric. 
/// </summary>
public static class NumberExtensions
{
    private const double MACHINE_DOUBLE_EPSILON = 2.2204460492503131E-016;
    private const float MACHINE_FLOAT_EPSILON = 1.192093E-07f;

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

        // The values themselves influence the effective epsilon involved in order for comparisons to survive scalar multiplication.
        // We add both values together (along with some padding for increased tolerance) and then multiply that by our base epsilon.
        double epsilon = (Math.Abs(source) + Math.Abs(other) + 10) * MACHINE_DOUBLE_EPSILON;

        return Math.Abs(source - other) < epsilon;
    }

    /// <summary>
    /// Determines if this floating-point number equals another floating-point number while taking into account the upper bound
    /// on relative errors due to rounding (aka the machine epsilon).
    /// </summary>
    /// <param name="source">The current floating-point number.</param>
    /// <param name="other">The floating-point number to compare with the current number.</param>
    /// <returns>True if <c>source</c> and <c>other</c> are approximately equal; otherwise, false.</returns>
    public static bool ApproximatelyEquals(this float source, float other)
    {
        if (float.IsPositiveInfinity(source))
            return float.IsPositiveInfinity(other);

        if (float.IsNegativeInfinity(source))
            return float.IsNegativeInfinity(other);

        // The values themselves influence the effective epsilon involved in order for comparisons to survive scalar multiplication.
        // We add both values together (along with some padding for increased tolerance) and then multiply that by our base epsilon.
        float epsilon = (Math.Abs(source) + Math.Abs(other) + 10) * MACHINE_FLOAT_EPSILON;

        return Math.Abs(source - other) < epsilon;
    }

    public static float NextValue(this float source)
        => source + (Math.Abs(source) + 10) * MACHINE_FLOAT_EPSILON;

    public static float PreviousValue(this float source)
        => source - (Math.Abs(source) + 10) * MACHINE_FLOAT_EPSILON;
}