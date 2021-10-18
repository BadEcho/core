﻿//-----------------------------------------------------------------------
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
    /// Provides a description for the event generated by the Apocalypse system upon the player dying due to being
    /// struck while afflicted with Fatalis in an Omnified game.
    /// </summary>
    /// <remarks>
    /// While this Apocalypse event is ultimately triggered from the player being struck, it is not treated as a typical
    /// <see cref="PlayerApocalypseEvent"/> due to there being no primary die roll involved with it.
    /// </remarks>
    public sealed class FatalisDeathEvent : ApocalypseEvent
    {
        /// <summary>
        /// Gets the amount of health lost due to being hit while afflicted with Fatalis.
        /// </summary>
        /// <remarks>
        /// Since being hit while being afflicted with Fatalis causes the player to die immediately, this value will reflect
        /// their current health at the time immediately prior to the attack.
        /// </remarks>
        public double HealthLost
        { get; init; }
    }
}
