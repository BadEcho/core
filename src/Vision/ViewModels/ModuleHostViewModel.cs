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
    /// Provides a view model that facilitates the display of a hosted Vision module.
    /// </summary>
    internal sealed class ModuleHostViewModel : ViewModel<ModuleHost>
    {
        private IViewModel? _moduleViewModel;

        /// <summary>
        /// Gets or sets the view model exported by the bound hosted Vision module.
        /// </summary>
        public IViewModel? ModuleViewModel
        {
            get => _moduleViewModel;
            set => NotifyIfChanged(ref _moduleViewModel, value);
        }

        /// <inheritdoc/>
        protected override void OnBinding(ModuleHost model) 
            => ModuleViewModel = model.ModuleViewModel;

        /// <inheritdoc/>
        protected override void OnUnbound(ModuleHost model) 
            => ModuleViewModel = null;
    }
}
