//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Text.Json;
using BadEcho.Odin.Serialization;
using BadEcho.Omnified.Vision.Statistics.Properties;

namespace BadEcho.Omnified.Vision.Statistics
{
    /// <summary>
    /// Provides a converter of <see cref="Statistic"/> objects to or from JSON.
    /// </summary>
    public sealed class StatisticConverter : JsonPolymorphicConverter<StatisticType,Statistic>
    {
        private const string STATISTIC_DATA_PROPERTY_NAME = "Statistic";

        /// <inheritdoc/>
        protected override string DataPropertyName
            => STATISTIC_DATA_PROPERTY_NAME;

        /// <inheritdoc/>
        protected override Statistic? ReadFromDescriptor(ref Utf8JsonReader reader, StatisticType typeDescriptor)
        {
            return typeDescriptor switch
            {
                StatisticType.Whole => JsonSerializer.Deserialize<WholeStatistic>(ref reader),
                StatisticType.Fractional => JsonSerializer.Deserialize<FractionalStatistic>(ref reader),
                StatisticType.Coordinate => JsonSerializer.Deserialize<CoordinateStatistic>(ref reader),
                _ => throw new InvalidEnumArgumentException(nameof(typeDescriptor), 
                                                            (int) typeDescriptor, 
                                                            typeof(StatisticType))
            };
        }

        /// <inheritdoc/>
        protected override StatisticType DescriptorFromValue(Statistic value)
        {
            return value switch
            {
                WholeStatistic => StatisticType.Whole,
                FractionalStatistic => StatisticType.Fractional,
                CoordinateStatistic => StatisticType.Coordinate,
                _ => throw new ArgumentException(Strings.ArgumentExceptionStatisticTypeUnsupported,
                                                 nameof(value))
            };
        }
    }
}
