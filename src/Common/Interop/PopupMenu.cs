// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Logging;
using System.ComponentModel;
using System.Runtime.InteropServices;
using BadEcho.Properties;

namespace BadEcho.Interop;

/// <summary>
/// Provides a basic, native popup contextual menu.
/// </summary>
public sealed class PopupMenu : IDisposable
{
    private readonly List<MenuItem> _items = [];

    private readonly WindowWrapper _windowWrapper;
    private readonly MenuHandle _menu;

    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="PopupMenu"/> class.
    /// </summary>
    /// <param name="windowWrapper">A wrapper around the window that will receive the menu's messages.</param>
    public PopupMenu(WindowWrapper windowWrapper)
    {
        Require.NotNull(windowWrapper, nameof(windowWrapper));

        _windowWrapper = windowWrapper;
        _menu = User32.CreatePopupMenu();

        if (_menu.IsInvalid)
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    /// <summary>
    /// Adds a separator to this menu.
    /// </summary>
    /// <returns>A <see cref="MenuItem"/> representing the newly added separator.</returns>
    public MenuItem AddSeparator() 
        => AddItem(string.Empty, MenuFlags.Separator);

    /// <summary>
    /// Adds a new menu item to this menu using the provided text as its label.
    /// </summary>
    /// <param name="label">The text of the new menu item.</param>
    /// <returns>A <see cref="MenuItem"/> instance representing the newly added menu item.</returns>
    public MenuItem AddItem(string label) 
        => AddItem(label, MenuFlags.String);

    /// <summary>
    /// Adds a new menu item to this menu using the provided text as its label and bitmap handle as its icon.
    /// </summary>
    /// <param name="label">The text of the new menu item.</param>
    /// <param name="bitmap">A handle to the bitmap to display alongside the label.</param>
    /// <returns>A <see cref="MenuItem"/> instance representing the newly added menu item.</returns>
    public MenuItem AddItem(string label, IntPtr bitmap)
    {
        var newItem = AddItem(label);

        SetItemBitmap(newItem, bitmap);
        
        return newItem;
    }

    private MenuItem AddItem(string label, MenuFlags flags)
    {
        // The menu item identifier must at least be 1, as a value of 0 is returned in the event nothing is clicked on
        // the menu (or if an error occurs).
        var newItem = new MenuItem(_items.Count + 1, label);

        if (!User32.AppendMenu(_menu, flags, (uint)newItem.Id, newItem.Label))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        _items.Add(newItem);

        return newItem;
    }
    
    /// <summary>
    /// Removes a previously added menu item from this menu.
    /// </summary>
    /// <param name="menuItem">The menu item to remove from this menu.</param>
    public void RemoveItem(MenuItem menuItem)
    {
        Require.NotNull(menuItem, nameof(menuItem));

        if (!User32.RemoveMenu(_menu, (uint) menuItem.Id, 0))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        _items.Remove(menuItem);
    }

    /// <summary>
    /// Enables the provided menu item, allowing the user to select it.
    /// </summary>
    /// <param name="menuItem">The menu item to enable.</param>
    public void EnableItem(MenuItem menuItem)
    {
        Require.NotNull(menuItem, nameof(menuItem));

        User32.EnableMenuItem(_menu, (uint) menuItem.Id, MenuFlags.Enabled);
    }

    /// <summary>
    /// Disables the provided menu item, preventing the user from selecting it.
    /// </summary>
    /// <param name="menuItem">The menu item to disable.</param>
    /// <remarks>
    /// The Win32 menu API is supposed to allow us to disable a menu item without graying it out by specifying
    /// <see cref="MenuFlags.Disabled"/> instead of <see cref="MenuFlags.Grayed"/>; however, from my observations,
    /// both of these flags result in the same outcome: the item is disabled AND grayed out. So, there's no way
    /// to disable without also graying the menu item out as well.
    /// </remarks>
    public void DisableItem(MenuItem menuItem)
    {
        Require.NotNull(menuItem, nameof(menuItem));

        User32.EnableMenuItem(_menu, (uint) menuItem.Id, MenuFlags.Grayed);
    }

    /// <summary>
    /// Opens this menu at the position of the mouse cursor.
    /// </summary>
    /// <returns>
    /// If a selection was made, the <see cref="MenuItem"/> instance representing the selected item; otherwise, null.
    /// </returns>
    public MenuItem? Open()
    {
        if (!User32.GetCursorPos(out POINT cursor))
            throw new Win32Exception(Marshal.GetLastWin32Error());
        
        return Open(cursor.x, cursor.y);
    }

    /// <summary>
    /// Opens this menu at the specified coordinates.
    /// </summary>
    /// <param name="x">The horizontal location of the menu, in screen coordinates.</param>
    /// <param name="y">The vertical location of the menu, in screen coordinates.</param>
    /// <returns>
    /// If a selection was made, the <see cref="MenuItem"/> instance representing the selected item; otherwise, null.
    /// </returns>
    public MenuItem? Open(int x, int y)
    {
        if (!User32.SetForegroundWindow(_windowWrapper.Handle))
            Logger.Warning(Strings.PopupNotInForeground); 

        var flags = User32.GetSystemMetrics(SystemMetric.MenuAlignment) == 0
            ? TrackMenuFlags.LeftAlign
            : TrackMenuFlags.RightAlign;

        flags |= TrackMenuFlags.ReturnCommand;

        var prcRect = new RECT();
        int result = User32.TrackPopupMenu(_menu,
                                           flags,
                                           x,
                                           y,
                                           0,
                                           _windowWrapper.Handle,
                                           ref prcRect);

        // Since we're using TrackMenuFlags.ReturnCommand, the return value will be zero if either nothing
        // was clicked or an error occurred. Unfortunately, if zero is returned, there is no way to discern
        // between the two. So, we just always assume the user made no selection.
        return result != 0 ? _items[result - 1] : null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _menu.Dispose();

        _disposed = true;
    }

    private void SetItemBitmap(MenuItem menuItem, IntPtr bitmap)
    {
        if (!User32.SetMenuItemBitmaps(_menu, (uint) menuItem.Id, 0, bitmap, bitmap))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }
}
