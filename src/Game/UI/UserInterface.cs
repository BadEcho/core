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

using Microsoft.Xna.Framework.Content;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a self-contained user interface configuration that loads and deploys packaged controls onto a provided
/// <see cref="Screen"/> instances.x
/// </summary>
public abstract class UserInterface
{
    /// <summary>w
    /// Attaches this user interface onto the provided screen.
    /// </summary>
    /// <param name="screen">The screen to display this user interface on.</param>
    public void Attach(Screen screen)
    {

    }

    /// <summary>
    /// Loads resources that this interface's controls depend on using the provided content manager.
    /// </summary>
    /// <param name="contentManager">The content manager to use to load the content.</param>
    protected virtual void LoadContent(ContentManager contentManager)
    { }

    /// <summary>
    /// Initializes and returns a layout panel containing this interface's controls.
    /// </summary>
    /// <returns>A <see cref="Panel"/> instance containing this interface's controls.</returns>
    protected abstract Panel LoadControls();
}
