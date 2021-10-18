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
    /// Provides a description for a Player Apocalypse "orgasm" event that occurred in an Omnified game.
    /// </summary>
    /// <remarks>
    /// This is the only beneficial Player Apocalypse event that can happen, and when it does, it heals the player
    /// to 100% full health. That's the power of spontaneous battlefield orgasms, baby.
    /// </remarks>
    public sealed class OrgasmEvent : PlayerApocalypseEvent
    {
        /// <summary>
        /// Gets the amount of health that was healed by the spontaneous orgasm.
        /// </summary>
        public double HealthHealed
        { get; init; }
    }
}
