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

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.Marshalling;
using BadEcho.Properties;

namespace BadEcho.Interop.Dialogs;

/// <summary>
/// Provides configuration for a page of content hosted by a task dialog.
/// </summary>
[NativeMarshalling(typeof(TaskDialogConfigurationMarshaller))]
public sealed class TaskDialogConfiguration
{
    private const int STARTING_CUSTOM_BUTTON_ID = 100;
    private const int STARTING_RADIO_BUTTON_ID = 1;

    private readonly List<TaskDialogButton> _pushButtons = [];
    private readonly List<TaskDialogRadioButton> _radioButtons = [];
    
    private EventHandler<EventArgs<int>>? _ticked;

    private WindowHandle? _owner;
    
    private TaskDialogProgressBar? _progressBar;

    private TaskDialogIcon _mainIcon;
    private TaskDialogIcon _footerIcon;

    private string? _text;
    private string? _title;
    private string? _footerText;
    private string? _instructionText;
    private string? _verificationText;
    private bool _verificationChecked;
    
    private string? _collapsedLabel;
    private string? _expandedLabel;
    private string? _expandedText;

    private bool _pendingTextUpdate;
    private bool _pendingInstructionTextUpdate;
    private bool _pendingExpandedTextUpdate;
    private bool _pendingFooterTextUpdate;

    private int _width;
    private bool _isInitialized;

    private TaskDialogFlags _flags;
    private TaskDialogFlags _controlFlags;

    /// <summary>
    /// Occurs after this configuration has been attached to a task dialog and the UI elements represented by this configuration
    /// have been created, but not yet displayed.
    /// </summary>
    public event EventHandler? Created;

    /// <summary>
    /// Occurs when the task dialog has destroyed the UI elements represented by this configuration.
    /// </summary>
    public event EventHandler? Closed;

    /// <summary>
    /// Occurs when the user has pressed F1, in a desperate attempt to seek help.
    /// </summary>
    public event EventHandler? HelpRequested;

    /// <summary>
    /// Occurs when the user has clicked on a link.
    /// </summary>
    public event EventHandler<EventArgs<string>>? HyperlinkClicked;

    /// <summary>
    /// Occurs when the checked state of the task dialog's verification check box has changed.
    /// </summary>
    public event EventHandler<EventArgs<bool>>? VerificationCheckedChanged;

    /// <summary>
    /// Occurs when the task dialog's internal timer has elapsed, approximately every 200 milliseconds.
    /// </summary>
    /// <remarks>The event arguments contain the number of milliseconds since this event was last fired.</remarks>
    public event EventHandler<EventArgs<int>>? Ticked
    {
        add
        {
            if (_ticked == null) 
                SetFlag(TaskDialogFlags.UseCallbackTimer, true);

            _ticked += value;
        }
        remove
        {
            _ticked -= value;

            if (_ticked == null)
                SetFlag(TaskDialogFlags.UseCallbackTimer, false);
        }
    }

    /// <summary>
    /// Gets or sets the handle to the window owning this dialog.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public WindowHandle? Owner
    {
        get => _owner;
        set
        {
            EnsureUnattached();

            _owner = value;
        }
    }
    
    /// <summary>
    /// Gets the collection of push buttons to be displayed on the dialog.
    /// </summary>
    /// <remarks>
    /// Modifying this while the dialog is showing will have no effect (probably a bad idea anyways).
    /// </remarks>
    public ICollection<TaskDialogButton> PushButtons
        => _pushButtons;

    /// <summary>
    /// Gets the collection of radio buttons to be displayed on the dialog.
    /// </summary>
    /// <remarks>
    /// Modifying this while the dialog is showing will have no effect (probably a bad idea anyways).
    /// </remarks>
    public ICollection<TaskDialogRadioButton> RadioButtons
        => _radioButtons;

    /// <summary>
    /// Gets or sets a progress bar control to display on the dialog.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public TaskDialogProgressBar? ProgressBar
    {
        get => _progressBar;
        set
        {
            EnsureUnattached();

            _progressBar = value;
        }
    }

    /// <summary>
    /// Gets or sets the default button for the dialog.
    /// </summary>
    /// <remarks>Modifying this while the dialog is showing will have no effect.</remarks>
    public TaskDialogButton? DefaultButton
    { get; set; }

    /// <summary>
    /// Gets or sets the icon displayed in the prominent, upper-left portion of the dialog.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public TaskDialogIcon MainIcon
    {
        get => _mainIcon;
        set
        {
            if (Attached) 
                Host.UpdateIcon(TaskDialogIconType.Main, (IntPtr) value);

            _mainIcon = value;
        }
    }

    /// <summary>
    /// Gets or sets the icon displayed in the footer of the dialog.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public TaskDialogIcon FooterIcon
    {
        get => _footerIcon;
        set
        {
            if (Attached) 
                Host.UpdateIcon(TaskDialogIconType.Footer, (IntPtr) value);

            _footerIcon = value;
        }
    }

    /// <summary>
    /// Gets or sets the title for the window of the dialog.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public string? Title
    {
        get => _title;
        set
        {
            if (Attached) 
                Host.UpdateTitle(value);

            _title = value;
        }
    }

    /// <summary>
    /// Gets or sets the primary content of the dialog.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public string? Text
    {
        get => _text;
        set
        {
            if (Attached)
            {
                if (!_isInitialized)
                    _pendingTextUpdate = true;
                else
                    Host.UpdateTextElement(TaskDialogTextElement.Content, value);
            }

            _text = value;
        }
    }

    /// <summary>
    /// Gets or sets the main instruction (the large, title-like text that appears near the top) of the dialog.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public string? InstructionText
    {
        get => _instructionText;
        set
        {
            if (Attached)
            {
                if (!_isInitialized)
                    _pendingInstructionTextUpdate = true;
                else
                    Host.UpdateTextElement(TaskDialogTextElement.MainInstruction, value);
            }

            _instructionText = value;
        }
    }

    /// <summary>
    /// Gets or sets the text that appears in the footer area of the dialog.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public string? FooterText
    {
        get => _footerText;
        set
        {
            if (Attached)
            {
                if (!_isInitialized)
                    _pendingFooterTextUpdate = true;
                else
                    Host.UpdateTextElement(TaskDialogTextElement.Footer, value);
            }

            _footerText = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating if the expanded area of the dialog is visible.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public bool IsExpanded
    {
        get => _flags.HasFlag(TaskDialogFlags.ExpandedByDefault);
        set => SetFlag(TaskDialogFlags.ExpandedByDefault, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="TaskDialogExpansionMode"/> value specifying where the expanded area of the dialog
    /// is to be displayed.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public TaskDialogExpansionMode ExpansionMode
    {
        get
        {
            return _flags.HasFlag(TaskDialogFlags.ExpandFooterArea)
                ? TaskDialogExpansionMode.ExpandFooter
                : TaskDialogExpansionMode.ExpandContent;
        }
        set => SetFlag(TaskDialogFlags.ExpandFooterArea, value == TaskDialogExpansionMode.ExpandFooter);
    }
    
    /// <summary>
    /// Gets or sets the expander button's text when the expander is in the collapsed state.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public string? CollapsedLabel
    {
        get => _collapsedLabel;
        set
        {
            EnsureUnattached();

            _collapsedLabel = value;
        }
    }

    /// <summary>
    /// Gets or sets the expander button's text when the expander is in the expanded state.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public string? ExpandedLabel
    {
        get => _expandedLabel;
        set
        {
            EnsureUnattached();

            _expandedLabel = value;
        }
    }

    /// <summary>
    /// Gets or sets the text to be displayed in the dialog's expanded area.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public string? ExpandedText
    {
        get => _expandedText;
        set
        {
            if (Attached)
            {
                if (!_isInitialized)
                    _pendingExpandedTextUpdate = true;
                else
                    Host.UpdateTextElement(TaskDialogTextElement.ExpandedInformation, value);
            }
            
            _expandedText = value;
        }
    }

    /// <summary>
    /// Gets or sets the text to be displayed alongside the dialog's verification check box.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public string? VerificationText
    {
        get => _verificationText;
        set
        {
            EnsureUnattached();

            _verificationText = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog's verification check box is in the checked state.
    /// </summary>
    /// <remarks>This can be changed while the dialog is showing.</remarks>
    public bool VerificationChecked
    {
        get => _verificationChecked;
        set
        {
            if (Attached)
            {
                Host.UpdateCheckBox(value);
            }

            _verificationChecked = value;
        }
    }

    /// <summary>
    /// Gets or sets <see cref="TaskDialogStartupLocation"/> value specifying where the dialog will appear on screen upon creation.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>

    public TaskDialogStartupLocation StartupLocation
    {
        get
        {
            return _flags.HasFlag(TaskDialogFlags.PositionRelativeToWindow)
                ? TaskDialogStartupLocation.CenterOwner
                : TaskDialogStartupLocation.CenterScreen;
        }
        set => SetFlag(TaskDialogFlags.PositionRelativeToWindow, value == TaskDialogStartupLocation.CenterOwner);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog can be closed using Alt-F4, Escape, or by clicking the title bar's close button.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public bool AllowCancel
    {
        get => _flags.HasFlag(TaskDialogFlags.AllowCancel);
        set => SetFlag(TaskDialogFlags.AllowCancel, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether text and controls are displayed reading right to left.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public bool RightToLeftLayout
    {
        get => _flags.HasFlag(TaskDialogFlags.RightToLeftLayout);
        set => SetFlag(TaskDialogFlags.RightToLeftLayout, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog can be minimized.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public bool CanBeMinimized
    {
        get => _flags.HasFlag(TaskDialogFlags.CanBeMinimized);
        set => SetFlag(TaskDialogFlags.CanBeMinimized, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the width of the dialog is determined by the width of its content area.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public bool SizeToContent
    {
        get => _flags.HasFlag(TaskDialogFlags.SizeToContent);
        set => SetFlag(TaskDialogFlags.SizeToContent, value);
    }

    /// <summary>
    /// Gets or sets the width of the dialog. Leave this unset for the ideal width to be automatically calculated.
    /// </summary>
    /// <remarks>This cannot be changed while the dialog is showing.</remarks>
    public int Width
    {
        get => _width;
        set
        {
            EnsureUnattached();

            _width = value;
        }
    }
    
    /// <summary>
    /// Gets the dialog this configuration is attached to.
    /// </summary>
    internal TaskDialog? Host
    { get; private set; }
    
    /// <summary>
    /// Gets the default radio button for the dialog, if any.
    /// </summary>
    internal TaskDialogRadioButton? DefaultRadioButton
    { get; private set; }

    /// <summary>
    /// Gets flags specifying the behavior of the dialog.
    /// </summary>
    internal TaskDialogFlags Flags
        => _flags | _controlFlags;

    /// <summary>
    /// Gets a value indicating if this configuration is attached to a dialog.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Host))]
    private bool Attached
        => Host != null;

    /// <summary>
    /// Navigates to a new page of content within the current dialog.
    /// </summary>
    /// <param name="configuration">The configuration describing the new page of content to navigate to.</param>
    public void Navigate(TaskDialogConfiguration configuration)
    {
        Require.NotNull(configuration, nameof(configuration));

        if (!Attached)
            throw new InvalidOperationException(Strings.TaskDialogNavigateRequiresAttachment);
        
        Host.Navigate(configuration);
    }

    /// <summary>
    /// Validates the configuration.
    /// </summary>
    internal void Validate()
    {
        bool hasCustomButtons = false, hasCommandLinks = false;
        bool foundDefaultButton = DefaultButton == null;

        foreach (TaskDialogButton button in PushButtons)
        {
            if (button is TaskDialogCommandLink)
                hasCommandLinks = true;
            else if (!button.IsStandard)
                hasCustomButtons = true;

            if (button == DefaultButton)
                foundDefaultButton = true;
        }

        if (hasCustomButtons && hasCommandLinks)
            throw new InvalidOperationException(Strings.TaskDialogCannotMixCustomButtonsAndLinks);

        if (RadioButtons.Count(r => r.Checked) > 1)
            throw new InvalidOperationException(Strings.TaskDialogCannotCheckMultipleRadioButtons);

        if (!foundDefaultButton)
            throw new InvalidOperationException(Strings.TaskDialogDefaultButtonNotInCollection);
    }

    /// <summary>
    /// Attaches this configuration to the dialog in preparation for marshalling, initialization, and display.
    /// </summary>
    /// <param name="host">The task dialog instance to attach to.</param>
    internal void Attach(TaskDialog host)
    {
        Require.NotNull(host, nameof(host));

        Host = host;

        _pendingInstructionTextUpdate = _pendingTextUpdate = false;

        int nextCustomButtonId = STARTING_CUSTOM_BUTTON_ID;

        foreach (TaskDialogButton button in PushButtons)
        {
            _controlFlags |= button.Attach(host, button.IsStandard ? 0 : nextCustomButtonId++);
        }

        if (PushButtons.OfType<TaskDialogCommandLink>().Any())
            _controlFlags |= TaskDialogFlags.UseCommandLinks;

        int nextRadioButtonId = STARTING_RADIO_BUTTON_ID;

        foreach (TaskDialogRadioButton radioButton in RadioButtons)
        {
            _controlFlags |= radioButton.Attach(host, nextRadioButtonId++);

            if (radioButton.Checked)
                DefaultRadioButton = radioButton;
        }

        if (DefaultRadioButton == null)
            _controlFlags |= TaskDialogFlags.NoDefaultRadioButton;

        if (ProgressBar != null)
            _controlFlags |= ProgressBar.Attach(host);
    }

    /// <summary>
    /// Detaches this configuration from the hosting task dialog.
    /// </summary>
    internal void Detach()
    {
        foreach (TaskDialogButton button in PushButtons)
        {
            button.Detach();
        }
        
        foreach (TaskDialogRadioButton radioButton in RadioButtons)
        {
            radioButton.Detach();
        }

        ProgressBar?.Detach();

        _controlFlags = default;

        Host = null;
        _isInitialized = false;
    }

    /// <summary>
    /// Performs the initialization logic required after dialog creation.
    /// </summary>
    internal void Initialize()
    {
        _isInitialized = true;

        if (_pendingInstructionTextUpdate)
        {
            InstructionText = _instructionText;
            _pendingInstructionTextUpdate = false;
        }

        if (_pendingTextUpdate)
        {
            Text = _text;
            _pendingTextUpdate = false;
        }

        if (_pendingFooterTextUpdate)
        {
            FooterText = _footerText;
            _pendingFooterTextUpdate = false;
        }

        if (_pendingExpandedTextUpdate)
        {
            ExpandedText = _expandedText;
            _pendingExpandedTextUpdate = false;
        }

        foreach (TaskDialogButton button in PushButtons)
        {
            button.Initialize();
        }
        
        foreach (TaskDialogRadioButton radioButton in RadioButtons)
        {
            radioButton.Initialize();
        }

        ProgressBar?.Initialize();
    }

    /// <summary>
    /// Raises the <see cref="Created"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> instance containing the event data.</param>
    internal void OnCreated(EventArgs e)
        => Created?.Invoke(this, e);
    
    /// <summary>
    /// Raises the <see cref="Closed"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> instance containing the event data.</param>
    internal void OnClosed(EventArgs e)
        => Closed?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="HelpRequested"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> instance containing the event data.</param>
    internal void OnHelpRequested(EventArgs e)
        => HelpRequested?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="HyperlinkClicked"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs{T}"/> instance containing the URL clicked.</param>
    internal void OnHyperlinkClicked(EventArgs<string> e)
        => HyperlinkClicked?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="Ticked"/> event.
    /// </summary>
    /// <param name="e">
    /// An <see cref="EventArgs{T}"/> instance containing the number of milliseconds since the event was last fired.
    /// </param>
    internal void OnTicked(EventArgs<int> e)
        => _ticked?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="VerificationCheckedChanged"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs{T}"/> instance containing checked state.</param>
    internal void OnVerificationCheckedChange(EventArgs<bool> e)
        => VerificationCheckedChanged?.Invoke(this, e);

    private void SetFlag(TaskDialogFlags flag, bool value)
    {
        EnsureUnattached();

        if (value)
            _flags |= flag;
        else
            _flags &= ~flag;
    }

    private void EnsureUnattached()
    {
        if (Attached)
            throw new InvalidOperationException(Strings.TaskDialogChangeRequiresDetachment);
    }
}
