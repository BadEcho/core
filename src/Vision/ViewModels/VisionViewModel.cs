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

using BadEcho.Fenestra.ViewModels;
using BadEcho.Odin;
using BadEcho.Omnified.Vision.Extensibility;

namespace BadEcho.Omnified.Vision.ViewModels
{
    /// <summary>
    /// Provides the root view model for the Vision application, which acts as a collection of hosted Vision modules.
    /// </summary>
    internal sealed class VisionViewModel : CollectionViewModel<ModuleHost, ModuleHostViewModel>
    {
        private AnchorPointLocation _titleLocation;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="VisionViewModel"/> class.
        /// </summary>
        public VisionViewModel()
            : base(new CollectionViewModelOptions())
        { }

        /// <summary>
        /// Gets or sets the location for the Vision application title's anchor point.
        /// </summary>
        public AnchorPointLocation TitleLocation
        {
            get => _titleLocation;
            set => NotifyIfChanged(ref _titleLocation, value);
        }

        /// <inheritdoc/>
        public override ModuleHostViewModel CreateChild(ModuleHost model)
        {
            var viewModel = new ModuleHostViewModel();
            
            viewModel.Bind(model);

            return viewModel;
        }
        
        /// <inheritdoc/>
        public override void UpdateChild(ModuleHost model)
        {
            var existingChild = FindChild<ModuleHostViewModel>(model);

            existingChild?.Bind(model);
        }

        /// <summary>
        /// Applies the provided Vision application configuration to this root view model instance.
        /// </summary>
        /// <param name="configuration">The Vision application configuration to apply to this view model.</param>
        public void ApplyConfiguration(IVisionConfiguration configuration)
        {
            Require.NotNull(configuration, nameof(configuration));

            TitleLocation = configuration.TitleLocation;
        }
    }
}