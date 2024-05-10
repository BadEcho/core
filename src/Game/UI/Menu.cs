//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensions;
using BadEcho.Game.Fonts;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a menu control that organizes various menu items associated with commands.
/// </summary>
public sealed class Menu : ContentControl<Grid, Menu>, ISelectable
{
    private readonly List<MenuItem> _menuItems = [];

    private Orientation _orientation;
    private DistanceFieldFont? _itemFont;
    private Color _itemFontColor;
    private float _itemFontSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="Menu"/> class.
    /// </summary>
    public Menu()
    {   
        IsFocusable = true;
        Content.IsSelectable = true;
        Content.SelectionChanged += HandleContainerSelectionChanged;
        Content.DefaultDimension = new GridDimension(1.0f, GridDimensionUnit.Proportional);
        Content.IsHoverPersistent = true;
    }

    /// <summary>
    /// Gets or sets the dimension by which menu items are laid out.
    /// </summary>
    public Orientation Orientation
    {
        get => _orientation;
        set => RemeasureIfChanged(ref _orientation, value);
    }

    /// <summary>
    /// Gets or sets the font used for the text of selectable items inside this menu.
    /// </summary>
    public DistanceFieldFont? ItemFont
    {
        get => _itemFont;
        set
        {
            if (_itemFont == value)
                return;

            _itemFont = value;

            _menuItems.ForEach(UpdateItemAppearance);
        }
    }

    /// <summary>
    /// Gets or sets the color of the font used for the text of selectable items inside this menu.
    /// </summary>
    public Color ItemFontColor
    {
        get => _itemFontColor;
        set
        {
            if (_itemFontColor == value)
                return;

            _itemFontColor = value;

            _menuItems.ForEach(UpdateItemAppearance);
        }
    }

    /// <summary>
    /// Gets or sets the size of the font in points used for the text of selectable items inside this menu.
    /// </summary>
    public float ItemFontSize
    {
        get => _itemFontSize;
        set
        {
            if (_itemFontSize.ApproximatelyEquals(value))
                return;

            _itemFontSize = value;

            _menuItems.ForEach(UpdateItemAppearance);
        }
    }
    
    /// <inheritdoc />
    public bool IsSelectable
        => true;

    /// <inheritdoc/>
    public bool IsHoverPersistent
    {
        get => Content.IsHoverPersistent;
        set => Content.IsHoverPersistent = value;
    }

    /// <inheritdoc />
    public IVisual? HoveredItemBackground
    {
        get => Content.HoveredItemBackground;
        set => Content.HoveredItemBackground = value;
    }

    /// <inheritdoc />
    public IVisual? SelectedItemBackground
    {
        get => Content.SelectedItemBackground;
        set => Content.SelectedItemBackground = value;
    }

    /// <summary>
    /// Creates an item in a menu using the provided text as its label.
    /// </summary>
    /// <param name="label">The text of the new menu item.</param>
    /// <returns>A <see cref="MenuItem"/> instance representing the newly added menu item.</returns>
    public MenuItem AddItem(string label)
    {
        var menuItem = new MenuItem
                       {
                           Text = label,
                           HorizontalAlignment = HorizontalAlignment.Center,
                           VerticalAlignment = VerticalAlignment.Center
                       };

        UpdateItemAppearance(menuItem);

        _menuItems.Add(menuItem);
        Content.AddChild(menuItem);

        return menuItem;
    }

    /// <inheritdoc/>
    protected override Size MeasureCore(Size availableSize)
    {
        UpdateItemContainer();

        return base.MeasureCore(availableSize);
    }

    private void UpdateItemAppearance(MenuItem menuItem)
    {
        menuItem.FontColor = _itemFontColor;
        menuItem.FontSize = _itemFontSize;
        menuItem.Font = _itemFont;
    }

    private void UpdateItemContainer()
    {
        for (int i = 0; i < _menuItems.Count; i++)
        {
            MenuItem menuItem = _menuItems[i];

            if (Orientation == Orientation.Horizontal)
            {
                menuItem.Column = i;
                menuItem.Row = 0;
            }
            else
            {
                menuItem.Row = i;
                menuItem.Column = 0;
            }
        }
    }

    private void HandleContainerSelectionChanged(object? sender, EventArgs<IEnumerable<IControl>> e)
    {
        var selectedMenuItem = e.Data.OfType<MenuItem>()
                                .First();

        selectedMenuItem.Select();
        Content.Unselect();
    }
}
