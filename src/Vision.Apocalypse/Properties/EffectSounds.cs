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
using System.Globalization;
using System.IO;
using System.Resources;
using BadEcho.Odin.Extensions;

namespace BadEcho.Omnified.Vision.Apocalypse.Properties
{
    /// <summary>
    /// Provides access to sound effect resources.
    /// </summary>
    /// <remarks>
    /// Visual Studio's .resx editor is absolutely broken when it comes to embedding audio resources in .resx files.
    /// Even worse, Microsoft is aware of the issue, check the relevant GitHub discussions. To get around this, manual
    /// creation of a .resources file was required, and this file makes loading said file easier.
    /// </remarks>
    public static class EffectSounds
    {
        private static readonly ResourceManager _Manager = new("BadEcho.Omnified.Vision.Apocalypse.Properties.EffectSounds",
                                                               typeof(EffectSounds).Assembly);
        /// <summary>
        /// Gets a stream for the lovely 'Chocobo' sound effect played during some critical hits.
        /// </summary>
        public static Stream CriticalHitChocobo
            => GetStream(nameof(CriticalHitChocobo));

        /// <summary>
        /// Gets a stream for the notable 'Combo Breaker!' sound effect played during some critical hits.
        /// </summary>
        public static Stream CriticalHitComboBreaker
            => GetStream(nameof(CriticalHitComboBreaker));

        /// <summary>
        /// Gets a stream for the distinctive 'Mudbone' sound effect played during some critical hits.
        /// </summary>
        public static Stream CriticalHitMudbone
            => GetStream(nameof(CriticalHitMudbone));

        /// <summary>
        /// Gets a stream for the memorable quip from 'Aliens' played during Fatalis affliction.
        /// </summary>
        public static Stream FatalisAffliction
            => GetStream(nameof(FatalisAffliction));

        /// <summary>
        /// Gets a stream for the wonderful sound effect played upon the awful Fatalis being cured.
        /// </summary>
        public static Stream FatalisCured
            => GetStream(nameof(FatalisCured));

        /// <summary>
        /// Gets a stream for the often-heard sound effect sometimes played upon dying from a Fatalis affliction.
        /// </summary>
        public static Stream FatalisDeath
            => GetStream(nameof(FatalisDeath));

        /// <summary>
        /// Gets a stream for the very annoying sound effect sometimes played upon dying from a Fatalis affliction.
        /// </summary>
        public static Stream FatalisDeathHaha
            => GetStream(nameof(FatalisDeathHaha));

        /// <summary>
        /// Gets a stream for the hilarious sound effect sometimes played upon dying from a Fatalis affliction.
        /// </summary>
        public static Stream FatalisDeathWilhelm
            => GetStream(nameof(FatalisDeathWilhelm));

        /// <summary>
        /// Gets a stream for the awesome sound effect played when we're ported too high up.
        /// </summary>
        public static Stream FreeFallin
            => GetStream(nameof(FreeFallin));

        /// <summary>
        /// Gets a stream for the very animoo-ish sound effect played upon achieving a Kamehameha.
        /// </summary>
        public static Stream Kamehameha
            => GetStream(nameof(Kamehameha));

        /// <summary>
        /// Gets a stream for the definitive (well, in a secondary sense) 'Murdered' sound effect.
        /// </summary>
        public static Stream MurderedHeadshot
            => GetStream(nameof(MurderedHeadshot));

        /// <summary>
        /// Gets a stream for the definitive 'Murdered' aka 'Sixty-Nined'  sound effect.
        /// </summary>
        public static Stream MurderedHolyShit
            => GetStream(nameof(MurderedHolyShit));

        /// <summary>
        /// Gets a stream for the rather pleasant sound effect of Vincent Price laughing at us for being murdered.
        /// </summary>
        public static Stream MurderedVincentLaugh
            => GetStream(nameof(MurderedVincentLaugh));

        /// <summary>
        /// Gets a stream for the very Steins-Gatey sound effect played during a spontaneous battlefield orgasm.
        /// </summary>
        public static Stream OrgasmTuturu
            => GetStream(nameof(OrgasmTuturu));

        /// <summary>
        /// Gets a stream for the sound effect, commonly encountered in Hentais, played during a spontaneous battlefield orgasm.
        /// </summary>
        public static Stream OrgasmWow
            => GetStream(nameof(OrgasmWow));

        private static Stream GetStream(string name)
        {
            UnmanagedMemoryStream? stream = _Manager.GetStream(name, CultureInfo.InvariantCulture);

            if (stream == null)
                throw new BadImageFormatException(Strings.SoundMissingResource.InvariantFormat(name));

            return stream;
        }
    }
}