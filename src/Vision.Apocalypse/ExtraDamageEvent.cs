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

using System;
using BadEcho.Odin.Extensions;
using BadEcho.Omnified.Vision.Apocalypse.Properties;

namespace BadEcho.Omnified.Vision.Apocalypse
{
    /// <summary>
    /// Provides a description for a Player Apocalypse "extra damage" event that occurred in an Omnified game.
    /// </summary>
    /// <remarks>
    /// This is, by default, the most common outcome from a Player Apocalypse event roll. For most games, the amount of
    /// extra damage applied is 2.0, however this is increased if the game features particularly low amounts of damage.
    /// </remarks>
    public sealed class ExtraDamageEvent : PlayerApocalypseEvent
    {
        /// <summary>
        /// Gets the multiplier applied to the incoming damage amount by the "extra damage" random effect.
        /// </summary>
        public double ExtraDamageMultiplier
        { get; init; }

        /// <inheritdoc/>
        public override string ToString() 
            => $"{EffectMessages.ExtraDamage.CulturedFormat(ExtraDamageMultiplier)}{base.ToString()}";
    }
}
 