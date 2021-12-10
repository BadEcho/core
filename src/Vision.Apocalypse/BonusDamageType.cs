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

namespace BadEcho.Omnified.Vision.Apocalypse;

/// <summary>
/// Specifies the type of bonus damage being applied to an Enemy Apocalypse event.
/// </summary>
public enum BonusDamageType
{
    /// <summary>
    /// A bonus stemming from a successful critical hit roll which multiplies the damage being done
    /// to an enemy by a randomly determined amount, typically falling within a range of non-ridiculous
    /// multipliers (i.e., 2.0x-5.0x).
    /// </summary>
    CriticalHit,
    /// <summary>
    /// A bonus stemming from a successful, and quite rare, Kamehameha roll, typically applying a completely
    /// ridiculous multiplier to the damage (i.e., 10000x).
    /// </summary>
    Kamehameha
}