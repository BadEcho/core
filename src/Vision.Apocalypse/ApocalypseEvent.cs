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

using System;

namespace BadEcho.Omnified.Vision.Apocalypse
{
    /// <summary>
    /// Provides a base description for an event generated by the Apocalypse system in an Omnified game.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An Apocalypse event describes an action undertaken by the Apocalypse system in response to damage being done either to the
    /// player, or to an enemy by the player. Different types of events exist for each of the various random effects the Apocalypse
    /// system can apply to entities receiving damage.
    /// </para>
    /// <para>
    /// All of the core Apocalypse events have a timestamp and are convertible into a recognizable effect message. These messages,
    /// displayed to the player to describe what the hell is happening in their game, as well show all the other auxiliary pieces of
    /// data specific to the event, are fleshed out in the particular event type that corresponds to the applied random effect.
    /// </para>
    /// </remarks>
    public abstract class ApocalypseEvent
    {
        /// <summary>
        /// Gets the date and time at which this Apocalypse event occurred.
        /// </summary>
        public DateTime Timestamp  
        { get; init; }
    }
}
