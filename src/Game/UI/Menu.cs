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

public sealed class Menu : Control
{
    private Orientation _orientation;
    private SpriteFont? _labelFont;

    public Orientation Orientation
    {
        get => _orientation;
        set => RemeasureIfChanged(ref _orientation, value);
    }

    /// <summary>
    /// Gets or sets the font used fo r the text of selectable items inside this menu.
    /// </summary>
    public SpriteFont? LabelFont
    {
        get => _labelFont;
        set => RemeasureIfChanged(ref _labelFont, value);
    }

    /// <summary>
    /// Gets or sets the color of the font used for the text of selectable items inside this menu.
    /// </summary>
    public Color LabelFontColor
    {  get; set; }

    /// <summary>
    /// Creates
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    public MenuItem AddItem(string label)
    {

    }
    

    protected override Size MeasureCore(Size availableSize)
        => throw new NotImplementedException();

    /// <inheritdoc />
    protected override void DrawCore(SpriteBatch spriteBatch) 
        => throw new NotImplementedException();
}
