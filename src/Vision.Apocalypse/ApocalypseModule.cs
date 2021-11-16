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
using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Odin.Extensions;
using BadEcho.Omnified.Vision.Apocalypse.ViewModels;
using BadEcho.Omnified.Vision.Extensibility;
using BadEcho.Omnified.Vision.Extensibility.Properties;

namespace BadEcho.Omnified.Vision.Apocalypse
{
    /// <summary>
    /// Provides a snap-in module granting vision to the Omnified Apocalypse system.
    /// </summary>
    [Export(typeof(IVisionModule))]
    internal sealed class ApocalypseModule : VisionModule<ApocalypseEvent, ApocalypseViewModel>
    {
        private const string DEPENDENCY_NAME
            = nameof(ApocalypseModule) + nameof(LocalDependency);

        /// <summary>
        /// Initializes a new instance of the <see cref="ApocalypseModule"/> class.
        /// </summary>
        /// <param name="configuration">Configuration settings for the Vision application.</param>
        [ImportingConstructor]
        public ApocalypseModule([Import(DEPENDENCY_NAME)] IVisionConfiguration configuration)
            : base(configuration)
        {
            var moduleConfiguration
                = configuration.Modules.GetConfiguration<ApocalypseModuleConfiguration>(ModuleName);

            ViewModel.ApplyConfiguration(moduleConfiguration);

            if (configuration.Dispatcher != null)
                ViewModel.ChangeDispatcher(configuration.Dispatcher);
        }

        /// <inheritdoc/>
        public override string MessageFile
            => "apocalypse.jsonl";

        /// <inheritdoc/>
        public override bool ProcessNewMessagesOnly
            => true;

        /// <inheritdoc/>
        protected override AnchorPointLocation DefaultLocation
            => AnchorPointLocation.TopCenter;

        /// <inheritdoc/>
        protected override IEnumerable<ApocalypseEvent> ParseMessages(string messages)
        {
            var options = new JsonSerializerOptions
                          {
                              Converters = { new EventConverter() }
                          };

            return messages.TrimStart(Environment.NewLine.ToCharArray())
                           .Split(Environment.NewLine)
                           .Select(ReadEvent);

            ApocalypseEvent ReadEvent(string message)
            {
                var apocalypseEvent = JsonSerializer.Deserialize<ApocalypseEvent>(message, options);

                return apocalypseEvent
                    ?? throw new ArgumentException(Strings.NullMessageValue.InvariantFormat(message),
                                                   nameof(message));
            }
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
