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

using BadEcho.Properties;

namespace BadEcho.Interop.Dialogs;

/// <summary>
/// Provides a task dialog push button control.
/// </summary>
public class TaskDialogButton : TaskDialogControl
{
    private readonly TaskDialogResult _result;

    private bool _enabled = true;
    private bool _isElevated;
    private string _text;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskDialogButton"/> class.
    /// </summary>
    /// <param name="text">The custom button's text.</param>
    public TaskDialogButton(string text)
    {
        Require.NotNullOrEmpty(text, nameof(text));

        _text = text;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskDialogButton"/> class.
    /// </summary>
    /// <param name="result">
    /// An enumeration value specifying the standard dialog result returned if this button is clicked.
    /// </param>
    internal TaskDialogButton(TaskDialogResult result)
    {
        IsStandard = true;
        _result = result;

        _text = result.ToString();
        Id = (int) result;
    }

    /// <summary>
    /// Occurs when this button has been clicked.
    /// </summary>
    public event EventHandler? Clicked;

    /// <summary>
    /// Creates a standard <see cref="TaskDialogButton"/> instance representing an "OK" button.
    /// </summary>
    public static TaskDialogButton OK 
        => new(TaskDialogResult.OK);

    /// <summary>
    /// Creates a standard <see cref="TaskDialogButton"/> instance representing a "Cancel" button.
    /// </summary>
    /// <remarks>
    /// Adding this button to a task dialog adds a close button to the dialog's title bar and will cause the dialog to respond to
    /// cancel actions (Alt-F4 and Escape).
    /// </remarks>
    public static TaskDialogButton Cancel
        => new(TaskDialogResult.Cancel);

    /// <summary>
    /// Creates a standard <see cref="TaskDialogButton"/> instance representing an "Abort" button.
    /// </summary>
    public static TaskDialogButton Abort
        => new(TaskDialogResult.Abort);

    /// <summary>
    /// Creates a standard <see cref="TaskDialogButton"/> instance representing a "Retry" button.
    /// </summary>
    public static TaskDialogButton Retry
        => new(TaskDialogResult.Retry);

    /// <summary>
    /// Creates a standard <see cref="TaskDialogButton"/> instance representing an "Ignore" button.
    /// </summary>
    public static TaskDialogButton Ignore
        => new(TaskDialogResult.Ignore);

    /// <summary>
    /// Creates a standard <see cref="TaskDialogButton"/> instance representing a "Yes" button.
    /// </summary>
    public static TaskDialogButton Yes
        => new(TaskDialogResult.Yes);

    /// <summary>
    /// Creates a standard <see cref="TaskDialogButton"/> instance representing a "No" button.
    /// </summary>
    public static TaskDialogButton No
        => new(TaskDialogResult.No);

    /// <summary>
    /// Creates a standard <see cref="TaskDialogButton"/> instance representing a "Close" button.
    /// </summary>
    public static TaskDialogButton Close
        => new(TaskDialogResult.Close);

    /// <summary>
    /// Creates a standard <see cref="TaskDialogButton"/> instance representing a "Help" button.
    /// </summary>
    public static TaskDialogButton Help
        => new(TaskDialogResult.Help);

    /// <summary>
    /// Creates a standard <see cref="TaskDialogButton"/> instance representing a "Try Again" button.
    /// </summary>
    public static TaskDialogButton TryAgain
        => new(TaskDialogResult.TryAgain);

    /// <summary>
    /// Creates a standard <see cref="TaskDialogButton"/> instance representing a "Continue" button.
    /// </summary>
    public static TaskDialogButton Continue
        => new(TaskDialogResult.Continue);
    
    /// <summary>
    /// Gets a value indicating if the button is a standard Windows dialog button (e.g., OK, Cancel, Yes, No).
    /// </summary>
    public bool IsStandard
    { get; }
    
    /// <summary>
    /// Gets or sets a value indicating if clicking on this button should result in the dialog closing and returning this button
    /// as the dialog's result. This is set to true by default.
    /// </summary>
    public bool ClosesDialog
    { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating if the button is enabled.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (Attached && IsInitialized) 
                Host.SetEnabled(this, value);

            _enabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating if the button requires elevation and should therefore have a UAC shield icon displayed.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public bool IsElevated
    {
        get => _isElevated;
        set
        {
            if (Attached && IsInitialized) 
                Host.SetElevationRequiredState(this, value);

            _isElevated = value;
        }
    }

    /// <summary>
    /// Gets or sets the button's text.
    /// </summary>
    /// <remarks>
    /// Only nonstandard buttons can have their text changed, and only while the dialog is not showing.
    /// </remarks>
    public string Text
    {
        get => _text;
        set
        {
            if (IsStandard)
                throw new InvalidOperationException(Strings.TaskDialogCannotChangeStandardButtonText);

            EnsureUnattached();

            _text = value;
        }
    }

    /// <summary>
    /// Forces the button to be clicked from code.
    /// </summary>
    public void Click()
    {
        EnsureAttachedAndInitialized();

        Host.ClickButton(this);
    }
    
    /// <summary>
    /// Process a click on the button, generating a <see cref="Clicked"/> event.
    /// </summary>
    /// <returns>Value indicating if the dialog should be closed and this button be returned as the dialog's result.</returns>
    internal bool ProcessClick()
    {
        OnClicked(EventArgs.Empty);

        return ClosesDialog;
    }

    /// <summary>
    /// Gets the text of the button formatted for display.
    /// </summary>
    /// <returns>The button's text formatted for display.</returns>
    internal virtual string GetText()
        => Text;

    /// <summary>
    /// Gets the flag that specifies this button's type.
    /// </summary>
    /// <returns>The <see cref="TaskDialogButtonFlags"/> value that specifies this button's type.</returns>
    /// <remarks>
    /// This is only valid for standard buttons; custom buttons do not have a corresponding <see cref="TaskDialogButtonFlags"/> value.
    /// </remarks>>
    internal TaskDialogButtonFlags GetStandardButtonFlag()
    {
        if (!IsStandard)
            throw new InvalidOperationException(Strings.TaskDialogNoFlagForCustomButton);

        return _result switch
        {
            TaskDialogResult.OK => TaskDialogButtonFlags.OK,
            TaskDialogResult.Cancel => TaskDialogButtonFlags.Cancel,
            TaskDialogResult.Abort => TaskDialogButtonFlags.Abort,
            TaskDialogResult.Retry => TaskDialogButtonFlags.Retry,
            TaskDialogResult.Ignore => TaskDialogButtonFlags.Ignore,
            TaskDialogResult.Yes => TaskDialogButtonFlags.Yes,
            TaskDialogResult.No => TaskDialogButtonFlags.No,
            TaskDialogResult.Close => TaskDialogButtonFlags.Close,
            TaskDialogResult.Help => TaskDialogButtonFlags.Help,
            TaskDialogResult.TryAgain => TaskDialogButtonFlags.TryAgain,
            TaskDialogResult.Continue => TaskDialogButtonFlags.Continue,
            _ => default
        };
    }

    /// <inheritdoc/>
    private protected override TaskDialogFlags AttachCore(int id = 0)
    {
        if (id != 0 && !IsStandard)
            Id = id;

        return base.AttachCore(id);
    }

    /// <inheritdoc/>
    private protected override void DetachCore()
    {
        if (!IsStandard)
            Id = 0;

        base.DetachCore();
    }

    /// <inheritdoc/>
    private protected override void InitializeCore()
    {
        if (!_enabled)
            Enabled = _enabled;

        if (_isElevated)
            IsElevated = _isElevated;

        base.InitializeCore();
    }

    private void OnClicked(EventArgs e)
        => Clicked?.Invoke(this, e);
}
