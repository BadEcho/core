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
    /// Provides a description for the "extra damage" Player Apocalypse event that occurred in an Omnified game.
    /// </summary>
    /// <remarks>
    /// This is, by default, the most common outcome from a Player Apocalypse event roll. For most games, the amount of
    /// extra damage applied is 2.0, however this is increased if the game features particularly low amounts of damage.
    /// </remarks>
    public sealed class ExtraDamageEvent : PlayerApocalypseEvent
    {
        /// <summary>
        /// Gets the multiplier applied to incoming damage amounts that are being subjected to an "extra damage"
        /// random effect.
        /// </summary>
        public double ExtraDamageMultiplier
        { get; init; }
    }
}
 