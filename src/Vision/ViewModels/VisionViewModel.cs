//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Fenestra.ViewModels;

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
        public VisionViewModel() 
            : base(new CollectionViewModelOptions())
        { }

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
