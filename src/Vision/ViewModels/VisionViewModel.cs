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
        /// <summary>
        /// Initializes a new instance of the <see cref="VisionViewModel"/> class.
        /// </summary>
        public VisionViewModel(IVisionConfiguration configuration)
            : base(new CollectionViewModelOptions())
        {
            Require.NotNull(configuration, nameof(configuration));
            
            LeftAnchorVerticalOffset = configuration.LeftAnchorVerticalOffset;
        }

        /// <summary>
        /// Gets or sets the distance between Vision components anchored to the left side of the screen and the top of
        /// the screen.
        /// </summary>
        public double LeftAnchorVerticalOffset
        { get; set; }

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

        /// <inheritdoc/>
        public override void OnChangeCompleted()
        { }
    }
}
