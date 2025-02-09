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
/// Provides a task dialog progress bar control.
/// </summary>
public sealed class TaskDialogProgressBar : TaskDialogControl
{
    private TaskDialogProgressBarState _state;
    private TaskDialogProgressBarState _initialState;
    private int _animationSpeed;
    private int _maximum = 100;
    private int _minimum;
    private int _value;

    /// <summary>
    /// Gets or sets the time, in milliseconds, between marquee animation updates.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public int AnimationSpeed
    {
        get => _animationSpeed;
        set
        {
            EnsureValidState();
            
            _animationSpeed = value;

            if (IsInitialized && IsMarquee(State))
                UpdateState(State);
        }
    }

    /// <summary>
    /// Gets or sets the maximum value of this progress bar.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public int Maximum
    {
        get => _maximum;
        set
        {
            EnsureValidState();

            if (value < 0 || value <= Minimum)
                throw new ArgumentOutOfRangeException(nameof(value), Strings.TaskDialogProgressBarMaximumOutOfRange);

            if (Attached && IsInitialized && !IsMarquee(State))
                Host.SetProgressBarRange(Minimum, value);

            _maximum = value;
        }
    }

    /// <summary>
    /// Gets or sets the minimum value of this progress bar.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public int Minimum
    {
        get => _minimum;
        set
        {
            EnsureValidState();
            
            if (value < 0 || value >= Maximum)
                throw new ArgumentOutOfRangeException(nameof(value), Strings.TaskDialogProgressBarMinimumOutOfRange);

            if (Attached && IsInitialized && !IsMarquee(State))
                Host.SetProgressBarRange(value, Maximum);
            
            _minimum = value;
        }
    }

    /// <summary>
    /// Gets or sets the state of this progress bar.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public TaskDialogProgressBarState State
    {
        get => _state;
        set
        {
            EnsureValidState();

            if (Attached && value == TaskDialogProgressBarState.None)
                throw new ArgumentException(Strings.TaskDialogProgressBarInvalidState, nameof(value));

            TaskDialogProgressBarState previousState = State;
            _state = value;

            if (IsInitialized)
                UpdateState(previousState);
        }
    }

    /// <summary>
    /// Gets or sets the current value of this progress bar.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public int Value
    {
        get => _value;
        set
        {
            EnsureValidState();

            if (Attached && IsInitialized && !IsMarquee(State))
            {
                Host.SetProgressBarPosition(value);

                if (State != TaskDialogProgressBarState.Normal)
                    Host.SetProgressBarPosition(value);
            }

            _value = value;
        }
    }

    /// <inheritdoc/>
    private protected override TaskDialogFlags AttachCore(int id = 0)
    {
        TaskDialogFlags flags = base.AttachCore(id);

        bool isMarquee = IsMarquee(State);

        _initialState = isMarquee ? TaskDialogProgressBarState.MarqueePaused : TaskDialogProgressBarState.Normal;

        flags |= isMarquee ? TaskDialogFlags.ShowMarqueeProgressBar : TaskDialogFlags.ShowProgressBar;

        return flags;
    }

    /// <inheritdoc/>
    private protected override void InitializeCore()
    {
        UpdateState(_initialState, true);

        base.InitializeCore();
    }

    private static bool IsMarquee(TaskDialogProgressBarState state)
        => state is TaskDialogProgressBarState.Marquee or TaskDialogProgressBarState.MarqueePaused;

    private void UpdateState(TaskDialogProgressBarState previousState, bool initializing = false)
    {
        EnsureAttached();

        bool isMarquee = IsMarquee(State);
        bool changeMode = IsMarquee(previousState) != isMarquee;

        if (changeMode)
        {
            if (isMarquee && previousState != TaskDialogProgressBarState.Normal) 
                Host.SetProgressBarState(TaskDialogProgressBarState.Normal);

            Host.SetMarqueeProgressBar(isMarquee);
        }

        if (isMarquee)
            Host.SetProgressBarMarquee(State == TaskDialogProgressBarState.Marquee, AnimationSpeed);
        else
        {
            Host.SetProgressBarState(State);

            if (initializing || changeMode)
            {
                Host.SetProgressBarRange(Minimum, Maximum);
                Value = _value;
            }
        }

    }

    private void EnsureValidState()
    {
        if (Attached && State == TaskDialogProgressBarState.None)
            throw new InvalidOperationException(Strings.TaskDialogCannotModifyUninitializedProgressBar);
    }
}
