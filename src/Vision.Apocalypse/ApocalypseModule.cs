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

using System.Composition;
using BadEcho.Fenestra.ViewModels;
using BadEcho.Odin;
using BadEcho.Odin.Extensibility.Hosting;
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
            => "apocalypse.log";

        /// <inheritdoc/>
        public bool ProcessNewMessagesOnly
            => true;

        /// <inheritdoc/>
        public IViewModel EnableModule(IMessageFileProvider messageProvider)
        {
            Require.NotNull(messageProvider, nameof(messageProvider));

            return _viewModel;
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
