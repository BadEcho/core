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

using System.IO;
using BadEcho.Odin;
using BadEcho.Omnified.Vision.Apocalypse.Properties;

namespace BadEcho.Omnified.Vision.Apocalypse;

/// <summary>
/// Provides a description for a Player Apocalypse "normal damage" event that occurred in an Omnified game
/// as the result of a "risk of murder" roll.
/// </summary>
/// <remarks>
/// <para>
/// This event, which is one of the possible outcomes for the dreaded "risk of murder" roll, is the only time
/// normal, vanilla damage from an Omnified game is ever applied to the player, outside of a
/// <see cref="TeleportitisEvent"/> (which itself usually ends up being responsible for more damage, indirectly at
/// least).
/// </para>
/// <para>
/// While the player may seem to have actually lucked out by receiving less damage than they normally would have from
/// the Apocalypse system, they are not quite out of danger yet, as it is only during a normal damage event that the player
/// runs the risk of becoming afflicted with the dreaded Fatalis debuff.
/// </para>
/// <para>
/// Indeed, one more die will end up being cast to check if the player has been cursed with Fatalis. If this ends up being
/// the case, the player will find themselves inflicted with this status effect for a randomly determined, unknown amount
/// of time. While under the effects of Fatalis, the player will instantly die upon receiving any amount of damage at all,
/// something that hearkens back to the original classic Omnified experience (which basically consisted of a single hack
/// that caused all damage to one-shot the player).
/// </para>
/// <para>
/// The exact probability of a normal damage event leading to a Fatalis affliction is dependent on the particular Omnified
/// game being played, with the chances typically being lower than normal for games that feature faster paced action and
/// combat.
/// </para>
/// </remarks>
public sealed class NormalDamageEvent : RiskOfMurderEvent
{
    /// <summary>
    /// The only possible sound effect that can arise from a <see cref="NormalDamageEvent"/> is when we're exposed to Fatalis,
    /// and it is one very easily interrupted soon after by any subsequent attack to the player (which will kill them). It's also
    /// one of the best sound effects we got, and it needs to be play out in full, 'cuz it is definitely GAME OVER MAN at that point.
    /// </summary>
    public override bool IsEffectSoundUninterruptible
        => true;

    /// <summary>
    /// Gets a value indicating if the player has been afflicted with Fatalis as a result of this event.
    /// </summary>
    public bool FatalisAfflicted
    { get; init; }

    /// <inheritdoc/>
    public override string ToString()
    {
        var effectMessage
            = $"{EffectMessages.NormalDamage}{base.ToString()}";

        var fatalisMessage
            = FatalisAfflicted ? $"{Environment.NewLine}{EffectMessages.Fatalis}" : string.Empty;

        return $"{effectMessage}{fatalisMessage}";
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Normal damage events only play sounds if we've been afflicted by Fatalis.
    /// </remarks>
    protected override WeightedRandom<Func<Stream>> InitializeSoundMap()
    {
        var soundMap = base.InitializeSoundMap();

        if (FatalisAfflicted)
            soundMap.AddWeight(() => EffectSounds.FatalisAffliction, 1);

        return soundMap;
    }
}