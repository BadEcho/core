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
using BadEcho.Odin.Serialization;
using BadEcho.Omnified.Vision.Statistics.Properties;

namespace BadEcho.Omnified.Vision.Statistics;

/// <summary>
/// Provides a converter of <see cref="Statistic"/> objects to or from JSON.
/// </summary>
public sealed class StatisticConverter : JsonPolymorphicConverter<StatisticType,IStatistic>
{
    /// <inheritdoc/>
    protected override string DataPropertyName
        => "Statistic";

    /// <inheritdoc/>
    protected override IStatistic? ReadFromDescriptor(ref Utf8JsonReader reader, StatisticType typeDescriptor)
    {
        var options = new JsonSerializerOptions
                      {
                          Converters = { new StatisticConverter() }
                      };

        return typeDescriptor switch
        {
            StatisticType.Whole => JsonSerializer.Deserialize<WholeStatistic>(ref reader),
            StatisticType.Fractional => JsonSerializer.Deserialize<FractionalStatistic>(ref reader),
            StatisticType.Coordinate => JsonSerializer.Deserialize<CoordinateStatistic>(ref reader),
            StatisticType.Group => JsonSerializer.Deserialize<StatisticGroup>(ref reader, options),
            _ => throw new InvalidEnumArgumentException(nameof(typeDescriptor), 
                                                        (int) typeDescriptor, 
                                                        typeof(StatisticType))
        };
    }

    /// <inheritdoc/>
    protected override StatisticType DescriptorFromValue(IStatistic value)
    {
        return value switch
        {
            WholeStatistic => StatisticType.Whole,
            FractionalStatistic => StatisticType.Fractional,
            CoordinateStatistic => StatisticType.Coordinate,
            StatisticGroup => StatisticType.Group,
            _ => throw new ArgumentException(Strings.StatisticTypeUnsupportedJson,
                                             nameof(value))
        };
    }
}