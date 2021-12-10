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
using BadEcho.Omnified.Vision.Extensibility;

namespace BadEcho.Omnified.Vision.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display of a hosted Vision module.
/// </summary>
internal sealed class ModuleHostViewModel : ViewModel<ModuleHost>
{
    private IViewModel? _moduleViewModel;
    private AnchorPointLocation _location;

    /// <summary>
    /// Gets or sets the view model exported by the bound hosted Vision module.
    /// </summary>
    public IViewModel? ModuleViewModel
    {
        get => _moduleViewModel;
        set => NotifyIfChanged(ref _moduleViewModel, value);
    }

    /// <summary>
    /// Gets the location of the anchor point for the bound hosted Vision module.
    /// </summary>
    public AnchorPointLocation Location
    {
        get => _location;
        set => NotifyIfChanged(ref _location, value);
    }

    /// <inheritdoc/>
    protected override void OnBinding(ModuleHost model)
    {
        ModuleViewModel = model.ModuleViewModel;
        Location = model.Location;
    }

    /// <inheritdoc/>
    protected override void OnUnbound(ModuleHost model)
    {
        ModuleViewModel = null;
        Location = default;
    }
}