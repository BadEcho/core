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

using System.IO;
using BadEcho.Extensions;
using BadEcho.Vision.Apocalypse.Properties;

namespace BadEcho.Vision.Apocalypse;

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
    public int HealthHealed
    { get; init; }

    /// <inheritdoc/>
    public override string ToString()
        => EffectMessages.Orgasm.CulturedFormat(HealthHealed, HealthAfter);

    /// <inheritdoc/>
    protected override WeightedRandom<Func<Stream>> InitializeSoundMap()
    {
        var soundMap = base.InitializeSoundMap();

        soundMap.AddWeight(() => EffectSounds.OrgasmTuturu, 1);
        soundMap.AddWeight(() => EffectSounds.OrgasmWow, 1);

        return soundMap;
    }
}