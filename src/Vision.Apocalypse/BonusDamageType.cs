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

namespace BadEcho.Vision.Apocalypse;

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