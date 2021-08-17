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
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text.Json;
using BadEcho.Fenestra.ViewModels;
using BadEcho.Odin;
using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Logging;
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
        private const string DEPENDENCY_NAME 
            = nameof(StatisticsModule) + nameof(LocalDependency);

        private static readonly string _AssemblyName
            = typeof(StatisticsModule).Assembly.GetName().Name ?? string.Empty;

        private readonly StatisticsViewModel _viewModel = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsModule"/> class.
        /// </summary>
        /// <param name="configuration">Configuration settings for the Vision application.</param>
        [ImportingConstructor]
        public StatisticsModule([Import(DEPENDENCY_NAME)]IVisionConfiguration configuration)
        {
            Require.NotNull(configuration, nameof(configuration));

            if (configuration.Modules.ContainsKey(_AssemblyName))
                DefaultLocation = configuration.Modules[_AssemblyName].Location ?? DefaultLocation;
        }

        /// <inheritdoc/>
        public AnchorPointLocation DefaultLocation
        { get; } = AnchorPointLocation.TopLeft;

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

        private static IEnumerable<IStatistic> ReadStatistics(string messages)
        {
            var options = new JsonSerializerOptions
                          {
                              Converters = {new StatisticConverter()}
                          };
            try
            {
                var statistics = JsonSerializer.Deserialize<IEnumerable<IStatistic>>(messages, options);

                return statistics
                    ?? throw new ArgumentException(Strings.JsonNotStatisticsSchema.InvariantFormat(messages), nameof(messages));
            }
            catch (JsonException jsonEx)
            {
                Logger.Error(Strings.StatisticsReadMessagesFailure
                                    .InvariantFormat(Environment.NewLine, messages), jsonEx);

                return Enumerable.Empty<IStatistic>();
            }
        }

        private void HandleNewMessages(object? sender, EventArgs<string> e)
        {
            var statistics = ReadStatistics(e.Data);

            _viewModel.Bind(statistics);
        }

        /// <summary>
        /// Provides a convention provider that allows for an armed context in which this module can have its required configuration
        /// provided to it during its initialization and exportation.
        /// </summary>
        /// <suppressions>
        /// ReSharper disable ClassNeverInstantiated.Local
        /// </suppressions>
        [Export(typeof(IConventionProvider))]
        private sealed class LocalDependency : DependencyRegistry<IVisionConfiguration>
        {
            public LocalDependency() 
                : base(DEPENDENCY_NAME)
            { }
        }
    }
}