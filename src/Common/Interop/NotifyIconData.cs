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

using System.Runtime.InteropServices.Marshalling;

namespace BadEcho.Interop;

/// <summary>
/// Provides information that the system needs to display notifications in the
/// notification area.
/// </summary>
[NativeMarshalling(typeof(NotifyIconDataMarshaller))]
internal sealed class NotifyIconData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyIconData"/> class.
    /// </summary>
    /// <param name="window">
    /// A handle to the window that receives notifications associated with an icon in the
    /// notification area.
    /// </param>
    /// <param name="id">The unique identifier of the taskbar icon.</param>
    /// <param name="flags">
    /// Flags that either indicate which of the other members of the structure contain valid
    /// data or provide additional information to the tooltip as to how it should display.
    /// </param>
    public NotifyIconData(WindowHandle window, Guid id, NotifyIconFlags flags)
    {
        Require.NotNull(window, nameof(window));

        Window = window;
        Id = id;
        // It is implied that the Guid identifier member is valid given our constructor's
        // parameters.
        Flags = flags | NotifyIconFlags.Guid;
    }

    /// <summary>
    /// Gets a handle to the window that receives notifications associated with an icon
    /// in the notification area.
    /// </summary>
    public WindowHandle Window
    { get; }

    /// <summary>
    /// Gets the unique identifier of the taskbar icon.
    /// </summary>
    public Guid Id
    { get; }

    /// <summary>
    /// Gets flags that either indicate which of the other members of the structure contain
    /// valid data or provide additional information to the tooltip as to how it should display.
    /// </summary>
    public NotifyIconFlags Flags
    { get; }

    /// <summary>
    /// Gets or sets an application-defined message identifier.
    /// </summary>
    public uint CallbackMessage
    { get; set; }

    /// <summary>
    /// Gets or sets a handle to the icon to be added, modified, or deleted.
    /// </summary>
    public IconHandle? Icon
    { get; set; }

    /// <summary>
    /// Gets or sets the state of the icon.
    /// </summary>
    public uint State
    { get; set; }

    /// <summary>
    /// Gets or sets a value that specifies which bits of the <see cref="State"/> member
    /// are retrieved or modified.
    /// </summary>
    public uint StateMask
    { get; set; }

    /// <summary>
    /// Gets or sets either the timeout value, in milliseconds, for the notification, or a
    /// specification of which version of the Shell notification icon interface should be used.
    /// </summary>
    public uint TimeoutOrVersion
    { get; set; }

    /// <summary>
    /// Gets or sets flags that can be set to modify the behavior and appearance of a
    /// balloon notification.
    /// </summary>
    public NotifyIconInfoFlags InfoFlags
    { get; set; }

    /// <summary>
    /// Gets or sets A handle to a customized notification icon that should be used independently
    /// of the notification area icon.
    /// </summary>
    public IconHandle? BalloonIcon
    { get; set; }

    /// <summary>
    /// Gets or sets a string that specifies the text for a standard tooltip.
    /// </summary>
    public string? Tip
    { get; set; }

    /// <summary>
    /// Gets or sets a string that specifies the text to display in a balloon notification.
    /// </summary>
    public string? Info
    { get; set; }

    /// <summary>
    /// Gets or sets a string that specifies a title for a balloon notification.
    /// </summary>
    public string? InfoTitle
    { get; set; }
}
