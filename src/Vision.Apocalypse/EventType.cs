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

namespace BadEcho.Omnified.Vision.Apocalypse
{
    /// <summary>
    /// Specifies the type of Apocalypse event exported from an Omnified game.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// An Enemy Apocalypse event.
        /// </summary>
        Enemy,
        /// <summary>
        /// An "extra damage" Player Apocalypse event.
        /// </summary>
        ExtraDamage,
        /// <summary>
        /// A "teleportitis" Player Apocalypse event.
        /// </summary>
        Teleportitis,
        /// <summary>
        /// A "normal damage" risk of murder Player Apocalypse event.
        /// </summary>
        NormalDamage,
        /// <summary>
        /// A "murder" risk of murder Player Apocalypse event.
        /// </summary>
        Murder,
        /// <summary>
        /// An "orgasm" Player Apocalypse event.
        /// </summary>
        Orgasm,
        /// <summary>
        /// An event signaling the player dying due to being struck while afflicted with Fatalis.
        /// </summary>
        FatalisDeath,
        /// <summary>
        /// An event signaling the player becoming cured of a Fatalis debuff they were previously afflicted with.
        /// </summary>
        FatalisCured
    }
}
