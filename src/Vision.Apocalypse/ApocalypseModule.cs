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
using BadEcho.Omnified.Vision.Apocalypse.Properties;
using BadEcho.Omnified.Vision.Apocalypse.ViewModels;
using BadEcho.Omnified.Vision.Extensibility;

namespace BadEcho.Omnified.Vision.Apocalypse
{
    /// <summary>
    /// Provides a snap-in module granting vision to the Omnified Apocalypse system.
    /// </summary>
    [Export(typeof(IVisionModule))]
    public sealed class ApocalypseModule : IVisionModule
    {
        private const string DEPENDENCY_NAME
            = nameof(ApocalypseModule) + nameof(LocalDependency);

        private static readonly string _AssemblyName
            = typeof(ApocalypseModule).Assembly.GetName().Name ?? string.Empty;

        private readonly ApocalypseViewModel _viewModel = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApocalypseModule"/> class.
        /// </summary>
        [ImportingConstructor]
        public ApocalypseModule([Import(DEPENDENCY_NAME)]IVisionConfiguration configuration)
        {
            Require.NotNull(configuration, nameof(configuration));

            if (configuration.Modules.ContainsKey(_AssemblyName))
                DefaultLocation = configuration.Modules[_AssemblyName].Location ?? DefaultLocation;
        }

        /// <inheritdoc/>
        public AnchorPointLocation DefaultLocation 
        { get; } = AnchorPointLocation.TopCenter;

        /// <inheritdoc/>
        public GrowthDirection GrowthDirection
            => GrowthDirection.Vertical;

        /// <inheritdoc/>
        public string MessageFile
            => "apocalypse.jsonl";

        /// <inheritdoc/>
        public bool ProcessNewMessagesOnly
            => true;

        /// <inheritdoc/>
        public IViewModel EnableModule(IMessageFileProvider messageProvider)
        {
            Require.NotNull(messageProvider, nameof(messageProvider));

            if (!string.IsNullOrEmpty(messageProvider.CurrentMessages))
                _viewModel.Bind(ReadEvents(messageProvider.CurrentMessages));

            messageProvider.NewMessages += HandleNewMessages;

            return _viewModel;
        }

        private static IEnumerable<ApocalypseEvent> ReadEvents(string messages)
        {
            var options = new JsonSerializerOptions
                          {
                              Converters = {new EventConverter()}
                          };
            try
            {
                return messages.Split(Environment.NewLine).Select(ReadEvent);
            }
            catch (JsonException jsonEx)
            {
                Logger.Error(Strings.ApocalypseReadMessagesFailure
                                    .InvariantFormat(Environment.NewLine, messages), jsonEx);

                return Enumerable.Empty<ApocalypseEvent>();
            }

            ApocalypseEvent ReadEvent(string message)
            {
                var apocalypseEvent = JsonSerializer.Deserialize<ApocalypseEvent>(message, options);

                return apocalypseEvent
                    ?? throw new ArgumentException(Strings.JsonNotApocalypseSchema.InvariantFormat(message),
                                                   nameof(message));
            }
        }
        
        private void HandleNewMessages(object? sender, EventArgs<string> e)
        {
            var apocalypseEvents = ReadEvents(e.Data);

            _viewModel.Bind(apocalypseEvents);
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
