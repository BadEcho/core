//-----------------------------------------------------------------------
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
//-----------------------------------------------------------------------

namespace BadEcho.Interop.Dialogs;

/// <summary>
/// Provides a task dialog push button control displayed as a command link.
/// </summary>
public sealed class TaskDialogCommandLink : TaskDialogButton
{
    private string? _note;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskDialogCommandLink"/> class.
    /// </summary>
    /// <param name="text">The command link's main text.</param>
    /// <param name="note">That command link's note, which is used to further describe what the command does.</param>
    public TaskDialogCommandLink(string text, string note) 
        : this(text)
    {
        _note = note;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskDialogCommandLink"/> class.
    /// </summary>
    /// <param name="text">The command link's main text.</param>
    public TaskDialogCommandLink(string text)
        : base(text)
    { }

    /// <summary>
    /// Gets or sets the command link's note, which appears below the main text and further describes what the command does.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public string? Note
    {
        get => _note;
        set
        {
            EnsureUnattached();

            _note = value;
        }
    }

    /// <inheritdoc/>
    internal override string GetText()
    {
        string text = base.GetText()
                          .Replace("\r\n", "\r", StringComparison.OrdinalIgnoreCase)
                          .Replace("\n", "\r", StringComparison.OrdinalIgnoreCase);

        if (!string.IsNullOrEmpty(Note))
            text += $"\n{Note}";

        return text;
    }
}
