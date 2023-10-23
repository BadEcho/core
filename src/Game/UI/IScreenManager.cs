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

namespace BadEcho.Game.UI;

/// <summary>
/// Defines a component that can be used to tear down and create new <see cref="Screen"/> instances attached to
/// a <see cref="UserInterface"/> instance.
/// </summary>
public interface IScreenManager
{
    /// <summary>
    /// Creates a new <see cref="Screen"/> instance attached to the provided <see cref="UserInterface"/> instance.
    /// </summary>
    /// <param name="userInterface">The user interface to attach the created screen to.</param>
    void AddScreen(UserInterface userInterface);

    /// <summary>
    /// Creates a new <see cref="Screen"/> instance attached to the provided <see cref="UserInterface"/> instance.
    /// </summary>
    /// <param name="userInterface">The user interface to attach the created screen to.</param>
    /// <param name="transitions">
    /// The transitions the user interface undergoes when being attached to and detached from the screen.
    /// </param>
    void AddScreen(UserInterface userInterface, Transitions transitions);

    /// <summary>
    /// Removes the last screen created by the manager.
    /// </summary>
    void RemoveScreen();
}
