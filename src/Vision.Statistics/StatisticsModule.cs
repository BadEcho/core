//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Composition;
using System.Text.Json;
using BadEcho.Fenestra.ViewModels;
using BadEcho.Odin;
using BadEcho.Odin.Extensions;
using BadEcho.Omnified.Vision.Extensibility;
using BadEcho.Omnified.Vision.Statistics.Properties;
using BadEcho.Omnified.Vision.Statistics.ViewModels;

namespace BadEcho.Omnified.Vision.Statistics
{
    /// <summary>
    /// Provides a snap-in module granting vision to Omnified game statistics data.
    /// </summary>
    [Export(typeof(IVisionModule))]
    public sealed class StatisticsModule : IVisionModule
    {
        private readonly StatisticsViewModel _viewModel = new();

        /// <inheritdoc/>
        public AnchorPointLocation DefaultLocation
            => AnchorPointLocation.TopLeft;

        /// <inheritdoc/>
        public GrowthDirection GrowthDirection
            => GrowthDirection.Vertical;

        /// <inheritdoc/>
        public string MessageFile
            => "statistics.json";

        /// <inheritdoc/>
        public bool ProcessNewMessagesOnly
            => false;

        /// <inheritdoc/>
        public IViewModel EnableModule(IMessageFileProvider messageProvider)
        {
            Require.NotNull(messageProvider, nameof(messageProvider));

            if (!string.IsNullOrEmpty(messageProvider.CurrentMessages))
                _viewModel.Bind(ReadStatistics(messageProvider.CurrentMessages));

            messageProvider.NewMessages += HandleNewMessages;

            return _viewModel;
        }

        private static IEnumerable<Statistic> ReadStatistics(string messages)
        {
            var options = new JsonSerializerOptions
                          {
                              Converters = {new StatisticConverter()}
                          };

            var statistics = JsonSerializer.Deserialize<IEnumerable<Statistic>>(messages, options);

            return statistics
                ?? throw new ArgumentException(Strings.JsonNotStatisticsSchema.InvariantFormat(messages), nameof(messages));
        }

        private void HandleNewMessages(object? sender, EventArgs<string> e)
        {
            var statistics = ReadStatistics(e.Data);

            _viewModel.Bind(statistics);
        }
    }
}
