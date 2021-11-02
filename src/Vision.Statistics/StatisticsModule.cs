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

using System.Collections.Generic;
using System.Composition;
using System.Text.Json;
using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Omnified.Vision.Extensibility;
using BadEcho.Omnified.Vision.Statistics.ViewModels;

namespace BadEcho.Omnified.Vision.Statistics
{
    /// <summary>
    /// Provides a snap-in module granting vision to Omnified game statistics data.
    /// </summary>
    [Export(typeof(IVisionModule))]
    internal sealed class StatisticsModule : VisionModule<IStatistic, StatisticsViewModel>
    {
        private const string DEPENDENCY_NAME 
            = nameof(StatisticsModule) + nameof(LocalDependency);

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsModule"/> class.
        /// </summary>
        /// <param name="configuration">Configuration settings for the Vision application.</param>
        [ImportingConstructor]
        public StatisticsModule([Import(DEPENDENCY_NAME)]IVisionConfiguration configuration)
            : base(configuration)
        { }

        /// <inheritdoc/>
        public override string MessageFile
            => "statistics.json";

        /// <inheritdoc/>
        protected override AnchorPointLocation DefaultLocation
            => AnchorPointLocation.TopLeft;

        protected override IEnumerable<IStatistic>? ParseMessages(string messages)
        {
            var options = new JsonSerializerOptions
                          {
                              Converters = { new StatisticConverter() }
                          };

            return JsonSerializer.Deserialize<IEnumerable<IStatistic>>(messages, options);
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