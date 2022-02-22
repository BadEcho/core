//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensions;
using BadEcho.Omnified.Vision.Apocalypse.Properties;
using System.IO;

namespace BadEcho.Omnified.Vision.Apocalypse;

/// <summary>
/// Provides a description for a Player Apocalypse "teleportitis" event that occurred in an Omnified game.
/// </summary>
/// <remarks>
/// <para>
/// This event causes the player to shift to a new, random location. It achieves this by shifting each of the player's
/// location coordinate axes by a base displacement amount of plus-or-minus 5 units, randomly determined for each axis
/// and then multiplied by a game-normalization displacement multiplier.
/// </para>
/// <para>
/// Other than causing the player to become incredibly disoriented due to the sudden change in location, it is also notable
/// for launching the player up into the air, inevitably leading to a fatal plunge back to the ground. Hilarious.
/// </para>
/// </remarks>
public sealed class TeleportitisEvent : PlayerApocalypseEvent
{
    /// <summary>
    /// In order to give Tom the respect that's due, no other sound effect shall interrupt the Free Fallin' (which is
    /// the only possible sound effect from this event).
    /// </summary>
    public override bool IsEffectSoundUninterruptible 
        => true;

    /// <summary>
    /// Gets the amount of displacement the x-coordinate was subjected to by the "teleportitis" random effect.
    /// </summary>
    public double XDisplacement 
    { get; init; }

    /// <summary>
    /// Gets the amount of displacement the y-coordinate was subjected to by the "teleportitis" random effect.
    /// </summary>
    public double YDisplacement
    { get; init; }

    /// <summary>
    /// Gets the amount of displacement the z-coordinate was subjected to by the "teleportitis" random effect.
    /// </summary>
    public double ZDisplacement
    { get; init; }

    /// <summary>
    /// Gets a value indicating if the vertical displacement was enough for the player to be left "free fallin'" to
    /// their deaths.
    /// </summary>
    public bool IsFreeFalling
    { get; init; }

    /// <inheritdoc/>
    public override string ToString()
        => $"{EffectMessages.Teleportitis.CulturedFormat(XDisplacement, YDisplacement, ZDisplacement)}{base.ToString()}";

    /// <inheritdoc/>
    protected override WeightedRandom<Func<Stream>> InitializeSoundMap()
    {
        var soundMap = base.InitializeSoundMap();

        if (IsFreeFalling)
            soundMap.AddWeight(() => EffectSounds.FreeFallin, 1);

        return soundMap;
    }
}