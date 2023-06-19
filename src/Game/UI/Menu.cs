//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a menu control that organizes various menu items associated with commands.
/// </summary>
public sealed class Menu : Control, ISelectable
{
    private readonly Grid _itemContainer = new();
    private readonly List<MenuItem> _menuItems = new();

    private Orientation _orientation;
    private SpriteFont? _itemFont;
    private Color _itemFontColor;

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
    public SpriteFont? ItemFont
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

    /// <inheritdoc />
    public bool IsSelectable
        => true;

    /// <inheritdoc />
    public IVisual? HoveredItemBackground
    {
        get => _itemContainer.HoveredItemBackground;
        set => _itemContainer.HoveredItemBackground = value;
    }

    /// <inheritdoc />
    public IVisual? SelectedItemBackground
    {
        get => _itemContainer.SelectedItemBackground;
        set => _itemContainer.SelectedItemBackground = value;
    }

    /// <summary>
    /// Gets or sets the color of the font used for the text of selectable items inside this menu.
    /// </summary>
    public Color LabelFontColor
    {  get; set; }

    /// <summary>
    /// Creates a item in a menu using the provided text as its label.
    /// </summary>
    /// <param name="label">The text of the new menu item.</param>
    /// <returns>A <see cref="MenuItem"/> instance representing the newly added menu item.</returns>
    public MenuItem AddItem(string label)
    {
        var menuItem = new MenuItem
                       {
                           Text = label,
                       };

        _menuItems.Add(menuItem);
        _itemContainer.AddChild(menuItem);

        return menuItem;
    }

    /// <inheritdoc/>
    protected override Size MeasureCore(Size availableSize)
        => throw new NotImplementedException();

    /// <inheritdoc />
    protected override void DrawCore(SpriteBatch spriteBatch) 
        => throw new NotImplementedException();

    private void UpdateItemAppearance(MenuItem menuItem)
    {
        menuItem.Font = _itemFont;
        menuItem.FontColor = _itemFontColor;
    }
}
