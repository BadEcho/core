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

namespace BadEcho.Game.UI;

/// <summary>
/// Defines a layout parent control, responsible for positioning child controls on a rendering surface.
/// </summary>
public interface IPanel : IControl
{
    /// <summary>
    /// Gets a collection of all child controls of this panel.
    /// </summary>
    IReadOnlyCollection<IControl> Children { get; }

    /// <summary>
    /// Adds the provided control to this panel.
    /// </summary>
    /// <typeparam name="T">The type of control that derives from <see cref="Control{T}"/>.</typeparam>
    /// <param name="child">The control to add to this panel.</param>
    void AddChild<T>(T child) where T : Control<T>;

    /// <summary>
    /// Removes the specified child control from this panel.
    /// </summary>
    /// <typeparam name="T">The type of control that derives from <see cref="Control{T}"/>.</typeparam>
    /// <param name="child">The control to remove from this panel.</param>
    void RemoveChild<T>(T child) where T : Control<T>;
}
