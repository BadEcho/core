using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadEcho.Game.UI;


public interface IPanel : IControl
{
    /// <summary>
    /// Gets a collection of all child controls of this panel.
    /// </summary>
    IReadOnlyCollection<IControl> Children { get; }

    /// <summary>
    /// Adds the provided control to this panel.
    /// </summary>
    /// <param name="child">The control to add to this panel.</param>
    void AddChild<T>(T child) where T : Control<T>;

    /// <summary>
    /// Removes the specified child control from this panel.
    /// </summary>
    /// <param name="child">The control to remove from this panel.</param>
    void RemoveChild<T>(T child) where T : Control<T>;
}
