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

using BadEcho.Properties;

namespace BadEcho.Interop.Dialogs;

/// <summary>
/// Provides a task dialog radio button control.
/// </summary>
public sealed class TaskDialogRadioButton : TaskDialogControl
{
    private bool _enabled = true;
    private bool _ignoreClicks;
    private bool _checked;
    private string _text;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskDialogRadioButton"/> class.
    /// </summary>
    /// <param name="text">The radio button's text.</param>
    public TaskDialogRadioButton(string text)
    {
        Require.NotNullOrEmpty(text, nameof(text));

        _text = text;
    }

    /// <summary>
    /// Occurs when the checked state of this radio button has changed.
    /// </summary>
    public event EventHandler? CheckedChanged;

    /// <summary>
    /// Gets or sets a value indicating if the radio button is enabled.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (Attached && IsInitialized)
            {
                Host.SetEnabled(this, value);
            }

            _enabled = value;
        }
    }

    /// <summary>
    /// Gets or sets the radio button's text.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public string Text
    {
        get => _text;
        set
        {
            EnsureUnattached();
            
            _text = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating if the radio button is checked.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public bool Checked
    {
        get => _checked;
        set
        {
            if (!Attached)
            {
                _checked = value;
                return;
            }

            if (Host.RadioButtonClicksBeingHandled > 0)
                throw new InvalidOperationException(Strings.TaskDialogCannotSetCheckedWhileHandlingClick);

            // You can't programmatically uncheck an attached radio button. The only way to uncheck it is to check another.
            if (!value)
                throw new InvalidOperationException(Strings.TaskDialogCannotUncheckRadioButton);

            // Before sending a task dialog message to click the radio button, we set our control up to ignore the clicked notification
            // and then raise the associated events afterwards. This avoids any chance of another button click message being sent while
            // already handling a click notification (which causes the task dialog API to bug out and issue endless notifications).
            _ignoreClicks = true;

            try
            {
                Host.ClickButton(this);
            }
            finally
            {
                _ignoreClicks = false;
            }

            Host.RadioButtonClicksBeingHandled++;

            try
            {
                Click();
            }
            finally
            {
                Host.RadioButtonClicksBeingHandled--;
            }
        }
    }

    /// <summary>
    /// Checks the radio button, generating a <see cref="CheckedChanged"/> event.
    /// </summary>
    internal void Click()
    {
        EnsureAttached();

        if (_ignoreClicks)
            return;

        if (_checked)
            return;

        _checked = true;

        foreach (TaskDialogRadioButton radioButton in HostConfiguration.RadioButtons)
        {
            if (radioButton != this)
            {
                radioButton._checked = false;
                radioButton.OnCheckedChanged(EventArgs.Empty);
            }
        }

        OnCheckedChanged(EventArgs.Empty);
    }

    /// <inheritdoc/>
    private protected override TaskDialogFlags AttachCore(int id = 0)
    {
        Id = id;

        return base.AttachCore(id);
    }

    /// <inheritdoc/>
    private protected override void DetachCore()
    {
        Id = 0;

        base.DetachCore();
    }

    /// <inheritdoc/>
    private protected override void InitializeCore()
    {
        if (!_enabled)
            Enabled = _enabled;

        base.InitializeCore();
    }

    private void OnCheckedChanged(EventArgs e)
        => CheckedChanged?.Invoke(this, e);
}
