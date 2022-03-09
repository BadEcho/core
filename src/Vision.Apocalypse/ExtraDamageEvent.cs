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

using BadEcho.Extensions;
using BadEcho.Vision.Apocalypse.Properties;

namespace BadEcho.Vision.Apocalypse;

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