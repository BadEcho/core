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

using System.Runtime.InteropServices;
using BadEcho.Properties;

namespace BadEcho.Interop.Dialogs;

/// <summary>
/// Provides a task dialog, which is a dialog box fancier than your run-of-the-mill message box used to display information
/// and receive simple input from the user.
/// </summary>
public sealed class TaskDialog : IDisposable
{
    private static readonly WindowMessage _ButtonClickContinuationEvent
        = User32.RegisterWindowMessage("TaskDialog.ButtonClickContinuationEvent");
    
    private readonly Queue<TaskDialogConfiguration> _pendingNavigations = new();
    private readonly WindowSubclass _subclass;

    private TaskDialogButton? _selectedButton;
    private bool _ignoreButtonClicks;
    private bool _isBeingDestroyed;
    private IntPtr _window;
    private GCHandle _handle;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskDialog"/> class.
    /// </summary>
    private TaskDialog()
    {
        _subclass = new WindowSubclass(WindowProcedure);
    }

    /// <summary>
    /// Gets a pointer to this task dialog, used as the callback data so that the callback function can access
    /// this instance.
    /// </summary>
    internal IntPtr Handle
        => (IntPtr) _handle;

    /// <summary>
    /// The <see cref="TaskDialogConfiguration"/> instance this task dialog is currently attached, whose UI elements
    /// are either currently or in the process of being displayed.
    /// </summary>
    internal TaskDialogConfiguration? AttachedConfiguration
    { get; private set; }

    /// <summary>
    /// Gets or sets the number of radio button click events presently being handled.
    /// </summary>
    /// <remarks>
    /// Used to prevent navigation and instances of recursive radio button click notifications from being issued
    /// due to changes to the <see cref="TaskDialogRadioButton.Checked"/> property while already handling a
    /// <see cref="TaskDialogNotification.RadioButtonClicked"/> callback.
    /// </remarks>
    internal int RadioButtonClicksBeingHandled
    { get; set; }

    /// <summary>
    /// Creates and displays a task dialog using the provided configuration.
    /// </summary>
    /// <param name="configuration">
    /// A <see cref="TaskDialogConfiguration"/> that defines the content and behavior of the task dialog.
    /// </param>
    /// <returns>
    /// The result of the task dialog in the form of the <see cref="TaskDialogButton"/> clicked by the user to close the dialog.
    /// </returns>
    public static TaskDialogButton Show(TaskDialogConfiguration configuration)
    {
        Require.NotNull(configuration, nameof(configuration));

        using (var taskDialog = new TaskDialog())
        {
            return taskDialog.ShowDialog(configuration);
        }
    }

    private TaskDialogButton ShowDialog(TaskDialogConfiguration configuration)
    {
        _handle = GCHandle.Alloc(this);

        configuration.Validate();
        configuration.Attach(this);
        AttachedConfiguration = configuration;

        try
        {
            int selectedButtonId;
            ResultHandle hResult;

            // Normally, any program wishing to use the Task Dialog API needs to enable Common Controls v6 via an
            // application manifest. We lift this burden from the users' shoulders by calling all relevant entry points
            // within an activation context that uses the manifest embedded in this library (which has Common Controls v6 enabled).
            using (ActivationScope.Create())
            {   
                hResult = ComCtl32.TaskDialogIndirect(ref configuration,
                                                      out selectedButtonId,
                                                      out _,
                                                      out _);
            }

            if (hResult.Failed())
                throw hResult.GetException();

            if (selectedButtonId == _selectedButton?.Id)
                return _selectedButton;

            return AttachedConfiguration.PushButtons.FirstOrDefault(b => b.Id == selectedButtonId)
                   ?? new TaskDialogButton((TaskDialogResult) selectedButtonId);
        }
        finally
        {
            _ignoreButtonClicks = false;
            _isBeingDestroyed = false;

            AttachedConfiguration.Detach();
            AttachedConfiguration = null;

            foreach (TaskDialogConfiguration pendingNavigation in _pendingNavigations)
            {
                pendingNavigation.Detach();
            }

            _pendingNavigations.Clear();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {   // The subclass window handle are normally cleaned up by the callback function when handling the Destroyed notification,
        // but we'll do so here as well in case an error occured.
        _subclass.Dispose();
        
        if (_handle.IsAllocated)
            _handle.Free();
    }
    
    /// <summary>
    /// Navigates this task dialog to a new page of content.
    /// </summary>
    /// <param name="configuration">The configuration describing the new page of content to navigate to.</param>
    internal void Navigate(TaskDialogConfiguration configuration)
    {   // Native API has issues handling navigation requests within the context of handling a radio button clicked notification.
        if (RadioButtonClicksBeingHandled > 0)
            throw new InvalidOperationException(Strings.TaskDialogCannotNavigateWhileHandlingClicked);

        // Allowing navigation to occur while the window is closing or already closed results in very strange behavior.
        if (_selectedButton != null || _isBeingDestroyed)
            throw new InvalidOperationException(Strings.TaskDialogCannotNavigateWhileClosed);

        configuration.Validate();
        configuration.Attach(this);

        // We can't initialize the new configuration or detach from the current one until receiving the notification that navigation
        // has completed. The native task dialog behaves as if the previous configuration is still in effect until this occurs.
        _pendingNavigations.Enqueue(configuration);

        SendTaskDialogMessage(TaskDialogMessage.NavigatePage, 0, configuration);
    }

    /// <summary>
    /// Simulates the action of a radio button click in the dialog.
    /// </summary>
    /// <param name="button">The radio button to click.</param>
    internal void ClickButton(TaskDialogRadioButton button) 
        => SendTaskDialogMessage(TaskDialogMessage.ClickRadioButton, button.Id, IntPtr.Zero);

    /// <summary>
    /// Configures whether or not a push button requires elevation and should therefore have a UAC shield icon displayed.
    /// </summary>
    /// <param name="button">The push button to update.</param>
    /// <param name="required">Value indicating if UAC elevation is required.</param>
    internal void SetElevationRequiredState(TaskDialogButton button, bool required)
        => SendTaskDialogMessage(TaskDialogMessage.SetButtonElevationRequiredState, button.Id, required ? 1 : 0);

    /// <summary>
    /// Enables or disables a push button in the dialog.
    /// </summary>
    /// <param name="button">The push button to update.</param>
    /// <param name="enabled">Value indicating if the button should be enabled.</param>
    internal void SetEnabled(TaskDialogButton button, bool enabled) 
        => SendTaskDialogMessage(TaskDialogMessage.EnableButton, button.Id, enabled ? 1 : 0);

    /// <summary>
    /// Enables or disables a radio button in the dialog.
    /// </summary>
    /// <param name="button">The radio button to update.</param>
    /// <param name="enabled">Value indicating if the button should be enabled.</param>
    internal void SetEnabled(TaskDialogRadioButton button, bool enabled) 
        => SendTaskDialogMessage(TaskDialogMessage.EnableRadioButton, button.Id, enabled ? 1 : 0);

    /// <summary>
    /// Sets the state of the progress bar in the dialog.
    /// </summary>
    /// <param name="state">An enumeration value specifying the state of the dialog's progress bar.</param>
    internal void SetProgressBarState(TaskDialogProgressBarState state)
        => SendTaskDialogMessage(TaskDialogMessage.SetProgressBarState, (int) state, 0);

    /// <summary>
    /// Sets whether the progress bar in the dialog should be displayed in marquee mode.
    /// </summary>
    /// <param name="isMarquee">Value indicating if the progress bar should be displayed in marquee mode.</param>
    internal void SetMarqueeProgressBar(bool isMarquee)
        => SendTaskDialogMessage(TaskDialogMessage.SetMarqueeProgressBar, isMarquee ? 1 : 0, 0);

    /// <summary>
    /// Starts and stops the marquee display of the progress bar in the dialog as well as its speed.
    /// </summary>
    /// <param name="isDisplayed">Value indicating if the marquee display should be on or off.</param>
    /// <param name="animationSpeed">The time, in milliseconds, between marquee animation updates.</param>
    internal void SetProgressBarMarquee(bool isDisplayed, int animationSpeed)
        => SendTaskDialogMessage(TaskDialogMessage.SetProgressBarMarquee, isDisplayed ? 1 : 0, animationSpeed);

    /// <summary>
    /// Sets the minimum and maximum values for the progress bar in the dialog.
    /// </summary>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    internal void SetProgressBarRange(int minimum, int maximum)
        => SendTaskDialogMessage(TaskDialogMessage.SetProgressBarRange, 0, (maximum << 16) + minimum);

    /// <summary>
    /// Sets the position, or value, of the progress bar in the dialog.
    /// </summary>
    /// <param name="position">The new position.</param>
    internal void SetProgressBarPosition(int position)
        => SendTaskDialogMessage(TaskDialogMessage.SetProgressBarPosition, position, 0);

    /// <summary>
    /// Updates an icon in the dialog.
    /// </summary>
    /// <param name="type">An enumeration value specifying the type of icon being updated.</param>
    /// <param name="icon">Pointer to the icon's resource identifier.</param>
    internal void UpdateIcon(TaskDialogIconType type, IntPtr icon)
        => SendTaskDialogMessage(TaskDialogMessage.UpdateIcon, (int) type, icon);

    /// <summary>
    /// Sets the dialog window's title.
    /// </summary>
    /// <param name="title">The title to use.</param>
    internal void UpdateTitle(string? title)
    {
        title ??= string.Empty;

        if (_window != IntPtr.Zero)
            User32.SetWindowText(_window, title);
    }

    /// <summary>
    /// Updates a text element in the dialog.
    /// </summary>
    /// <param name="element">An enumeration value specifying the text element being updated.</param>
    /// <param name="text">The new text.</param>
    internal unsafe void UpdateTextElement(TaskDialogTextElement element, string? text)
    {
        text ??= string.Empty;

        fixed (char* pText = text)
        {
            SendTaskDialogMessage(TaskDialogMessage.UpdateElementText, (int) element, (IntPtr) pText);
        }
    }

    /// <summary>
    /// Updates the checked state of the dialog's verification check box.
    /// </summary>
    /// <param name="isChecked">Value indicating the checked state.</param>
    internal void UpdateCheckBox(bool isChecked)
        => SendTaskDialogMessage(TaskDialogMessage.ClickVerification, isChecked ? 1 : 0, 0);

    /// <summary>
    /// Callback that processes notifications sent from the task dialog.
    /// </summary>
    /// <param name="hWnd">Handle to the task dialog window.</param>
    /// <param name="notification">An enumeration value specifying the notification being sent.</param>
    /// <param name="wParam">Additional notification information.</param>
    /// <param name="lParam">Additional notification information.</param>
    /// <returns>A return value specific to the notification being processed.</returns>
    internal ResultHandle TaskDialogCallbackProc(IntPtr hWnd, 
                                                 TaskDialogNotification notification, 
                                                 IntPtr wParam,
                                                 IntPtr lParam)
    {
        // We go ahead and subclass the window as early as possible, which will be following the first notification received.
        if (_window == IntPtr.Zero)
        {
            _subclass.Attach(new WindowHandle(hWnd, false));
            _window = hWnd;
        }

        if (AttachedConfiguration == null)
            throw new InvalidOperationException(Strings.TaskDialogPrematureDetachmentDuringCallback);

        switch (notification)
        {
            case TaskDialogNotification.Created:
                // Now that the dialog has been created, we can complete initialization of the various control settings
                // that cannot be committed prior to an active dialog instance being displayed.
                AttachedConfiguration.Initialize();
                AttachedConfiguration.OnCreated(EventArgs.Empty);

                break;

            case TaskDialogNotification.Navigated:
                // With navigation complete, we can detach from our previous configured content and initialize the now active configuration.
                AttachedConfiguration.Detach();
                AttachedConfiguration = _pendingNavigations.Dequeue();

                AttachedConfiguration.Initialize();
                AttachedConfiguration.OnCreated(EventArgs.Empty);
                break;

            case TaskDialogNotification.ButtonClicked:

                if (_ignoreButtonClicks)
                    return ResultHandle.False;

                // Workaround for behavior where a click notification is sent twice when pressing a button's access key.
                if (User32.PostMessage(_window, _ButtonClickContinuationEvent, IntPtr.Zero, IntPtr.Zero)) 
                    _ignoreButtonClicks = true;

                int buttonId = wParam.ToInt32();
                TaskDialogButton? button = AttachedConfiguration.PushButtons.FirstOrDefault(b => b.Id == buttonId);

                if (button == null)
                {   // Someone needs to settle down with their configuration voodoo if a button not in our configuration was clicked.
                    _selectedButton = new TaskDialogButton((TaskDialogResult) buttonId);
                }
                else
                {
                    // This will be false if the button isn't configured to close the dialog.
                    bool applyButtonAsResult = button.Click(); 

                    if (applyButtonAsResult)
                        _selectedButton = button;
                    else
                        return ResultHandle.False; // This will prevent the dialog from closing.
                }

                break;

            case TaskDialogNotification.RadioButtonClicked:

                int radioButtonId = wParam.ToInt32();
                TaskDialogRadioButton? radioButton = AttachedConfiguration.RadioButtons.FirstOrDefault(b => b.Id == radioButtonId);

                if (radioButton == null)
                    throw new InvalidOperationException(Strings.TaskDialogUnknownRadioButtonClicked);

                RadioButtonClicksBeingHandled++;

                try
                {
                    radioButton.Click();
                }
                finally
                {
                    RadioButtonClicksBeingHandled--;
                }

                break;

            case TaskDialogNotification.Destroyed:
                _isBeingDestroyed = true;
                AttachedConfiguration.OnClosed(EventArgs.Empty);

                _subclass.Dispose();
                
                // Prevent the sending of any more messages to the dialog after receiving the Destroyed notification.
                _window = IntPtr.Zero;

                break;

            case TaskDialogNotification.Help:
                AttachedConfiguration.OnHelpRequested(EventArgs.Empty);

                break;

            case TaskDialogNotification.HyperlinkClicked:
                string? url = Marshal.PtrToStringUni(lParam);

                if (!string.IsNullOrEmpty(url))
                    AttachedConfiguration.OnHyperlinkClicked(new EventArgs<string>(url));

                break;

            case TaskDialogNotification.Timer:
                AttachedConfiguration.OnTicked(new EventArgs<int>(wParam.ToInt32()));

                break;

            case TaskDialogNotification.VerificationClicked:
                AttachedConfiguration.OnVerificationCheckedChange(new EventArgs<bool>(wParam != IntPtr.Zero));

                break;
        }

        return ResultHandle.Success;
    }
    
    private void SendTaskDialogMessage(TaskDialogMessage message, int wParam, long lParam)
    {
        if (_window == IntPtr.Zero)
            throw new InvalidOperationException(Strings.NoTaskDialogWindowListening);

        ComCtl32.SendTaskDialogMessage(_window, message, wParam, lParam);
    }

    private void SendTaskDialogMessage(TaskDialogMessage message, int wParam, TaskDialogConfiguration configuration)
    {
        if (_window == IntPtr.Zero)
            throw new InvalidOperationException(Strings.NoTaskDialogWindowListening);

        ComCtl32.SendTaskDialogMessage(_window, message, wParam, ref configuration);
    }

    private ProcedureResult WindowProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        IntPtr lResult = IntPtr.Zero;
        bool handled = false;
        var message = (WindowMessage) msg;

        if (_ButtonClickContinuationEvent == message)
        {   // By the time we receive this event, we'll have weeded out any duplicate button click notifications
            // sent due to use of a button's access key.
            handled = true;

            _ignoreButtonClicks = false;
        }

        return new ProcedureResult(lResult, handled);
    }
}
