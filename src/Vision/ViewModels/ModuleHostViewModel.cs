//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Presentation.ViewModels;
using BadEcho.Vision.Extensibility;

namespace BadEcho.Vision.ViewModels;

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