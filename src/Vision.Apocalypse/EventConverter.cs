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

using System.ComponentModel;
using System.Text.Json;
using BadEcho.Serialization;
using BadEcho.Vision.Apocalypse.Properties;

namespace BadEcho.Vision.Apocalypse;

/// <summary>
/// Provides a converter of <see cref="ApocalypseEvent"/> objects to or from JSON.
/// </summary>
public sealed class EventConverter : JsonPolymorphicConverter<EventType,ApocalypseEvent>
{
    /// <inheritdoc/>
    protected override string DataPropertyName
        => "Event";

    /// <inheritdoc/>
    protected override ApocalypseEvent? ReadFromDescriptor(ref Utf8JsonReader reader, EventType typeDescriptor)
    {
        return typeDescriptor switch
        {
            EventType.Enemy => JsonSerializer.Deserialize<EnemyApocalypseEvent>(ref reader),
            EventType.ExtraDamage => JsonSerializer.Deserialize<ExtraDamageEvent>(ref reader),
            EventType.Teleportitis => JsonSerializer.Deserialize<TeleportitisEvent>(ref reader),
            EventType.NormalDamage => JsonSerializer.Deserialize<NormalDamageEvent>(ref reader),
            EventType.Murder => JsonSerializer.Deserialize<MurderEvent>(ref reader),
            EventType.Orgasm => JsonSerializer.Deserialize<OrgasmEvent>(ref reader),
            EventType.FatalisDeath => JsonSerializer.Deserialize<FatalisDeathEvent>(ref reader),
            EventType.FatalisCured => JsonSerializer.Deserialize<FatalisCuredEvent>(ref reader),
            _ => throw new InvalidEnumArgumentException(nameof(typeDescriptor),
                                                        (int) typeDescriptor,
                                                        typeof(EventType))
        };
    }

    /// <inheritdoc/>
    protected override EventType DescriptorFromValue(ApocalypseEvent value)
    {
        return value switch
        {
            EnemyApocalypseEvent => EventType.Enemy,
            ExtraDamageEvent => EventType.ExtraDamage,
            TeleportitisEvent => EventType.Teleportitis,
            NormalDamageEvent => EventType.NormalDamage,
            MurderEvent => EventType.Murder,
            OrgasmEvent => EventType.Orgasm,
            FatalisDeathEvent => EventType.FatalisDeath,
            FatalisCuredEvent => EventType.FatalisCured,
            _ => throw new ArgumentException(Strings.EventTypeUnsupportedJson,
                                             nameof(value))
        };
    }
}