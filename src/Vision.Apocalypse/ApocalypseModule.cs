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

using System.Composition;
using System.Text.Json;
using BadEcho.Extensibility.Hosting;
using BadEcho.Extensions;
using BadEcho.Vision.Apocalypse.ViewModels;
using BadEcho.Vision.Extensibility;
using BadEcho.Vision.Extensibility.Properties;

namespace BadEcho.Vision.Apocalypse;

/// <summary>
/// Provides a snap-in module granting vision to the Omnified Apocalypse system.
/// </summary>
[Export(typeof(IVisionModule))]
internal sealed class ApocalypseModule : VisionModule<ApocalypseEvent, ApocalypseViewModel>
{
    private const string DEPENDENCY_NAME
        = nameof(ApocalypseModule) + nameof(LocalDependency);

    private int _eventIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApocalypseModule"/> class.
    /// </summary>
    /// <param name="configuration">Configuration settings for the Vision application.</param>
    [ImportingConstructor]
    public ApocalypseModule([Import(DEPENDENCY_NAME)] IVisionConfiguration configuration)
        : base(configuration)
    { }

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
    protected override ApocalypseViewModel InitializeViewModel()
    {
        var moduleConfiguration
            = Configuration.Modules.GetConfiguration<ApocalypseModuleConfiguration>(ModuleName);

        var viewModel = new ApocalypseViewModel(moduleConfiguration);

        if (Configuration.Dispatcher != null)
            viewModel.ChangeDispatcher(Configuration.Dispatcher);

        return viewModel;
    }

    /// <inheritdoc/>
    protected override IEnumerable<ApocalypseEvent> ParseMessages(string messages)
    {
        var options = new JsonSerializerOptions
                      {
                          Converters = { new EventConverter() }
                      };

        return messages.Split(Environment.NewLine)
                       .WhereNotNullOrEmpty()
                       .Select(ReadEvent);
            
        ApocalypseEvent ReadEvent(string message)
        {
            var apocalypseEvent = JsonSerializer.Deserialize<ApocalypseEvent>(message, options);

            if (apocalypseEvent != null)
                apocalypseEvent.Index = Interlocked.Increment(ref _eventIndex);
            
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