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

using BadEcho.Interop;
using BadEcho.Interop.Dialogs;
using Xunit;

namespace BadEcho.Tests.Interop;

public class TaskDialogTests
{
    private readonly TaskDialogConfiguration _configuration;
    private readonly TaskDialogButton _okButton = TaskDialogButton.OK;
    private readonly TaskDialogButton _cancelButton = TaskDialogButton.Cancel;

    public TaskDialogTests()
    {
        _configuration = new TaskDialogConfiguration
                         {
                             CollapsedLabel = "Expand",
                             ExpandedLabel = "Collapse",
                             ExpandedText = "Here there be more text.",
                             FooterText = "Feet Text",
                             FooterIcon = TaskDialogIcon.Information,
                             Text = "Main text be here.",
                             InstructionText = "Respect Mah Authoritah!",
                             Title = "A Nice Title",
                             PushButtons =
                             {
                                 _okButton,
                                 _cancelButton
                             }
                         };
    }

    [Fact]
    public void Validate_TwoCheckedRadioButtons_ThrowsException()
    {
        var first = new TaskDialogRadioButton("First");
        var second = new TaskDialogRadioButton("Second");

        first.Checked = true;
        second.Checked = true;

        _configuration.RadioButtons.Add(first);
        _configuration.RadioButtons.Add(second);

        Assert.Throws<InvalidOperationException>(() => TaskDialog.Show(_configuration));
    }

    [Fact]
    public void Show_ClickOKAfterCreated_ReturnsOK()
    {
        _configuration.Created += ClickOKButton;

        TaskDialogButton result = TaskDialog.Show(_configuration);

        Assert.Equal(_okButton, result);
    }

    [Fact]
    public void Show_ClickCommandLinkAfterCreated_ReturnsCommandLink()
    {
        _configuration.PushButtons.Clear();

        var commandLink = new TaskDialogCommandLink("Command Me", "Extra stuff");

        _configuration.Created += ClickCommandLink;
        _configuration.PushButtons.Add(commandLink);

        TaskDialogButton result = TaskDialog.Show(_configuration);

        Assert.Equal(commandLink, result);

        void ClickCommandLink(object? sender, EventArgs e)
        {
            commandLink.Click();
        }
    }

    [Fact]
    public void Show_ClickCancelAfterCreated_ReturnsCancel()
    {
        _configuration.Created += ClickCancelButton;

        TaskDialogButton result = TaskDialog.Show(_configuration);

        Assert.Equal(_cancelButton, result);

        void ClickCancelButton(object? sender, EventArgs e)
        {
            _cancelButton.Click();
        }
    }

    [Fact]
    public void Show_LotsOfButtons_ReturnsOK()
    {
        _configuration.Created += ClickOKButton;

        _configuration.PushButtons.Add(TaskDialogButton.Abort);
        _configuration.PushButtons.Add(TaskDialogButton.Retry);
        _configuration.PushButtons.Add(TaskDialogButton.Ignore);
        _configuration.PushButtons.Add(TaskDialogButton.Yes);
        _configuration.PushButtons.Add(TaskDialogButton.No);
        _configuration.PushButtons.Add(TaskDialogButton.Close);
        _configuration.PushButtons.Add(TaskDialogButton.Help);
        _configuration.PushButtons.Add(TaskDialogButton.TryAgain);
        _configuration.PushButtons.Add(TaskDialogButton.Continue);

        TaskDialogButton result = TaskDialog.Show(_configuration);

        Assert.Equal(_okButton, result);
    }

    [Fact]
    public void Show_ClickCustomAfterCreated_ReturnsCustom()
    {
        var custom = new TaskDialogButton("I am Special");
        
        _configuration.Created += ClickCustomButton;
        _configuration.PushButtons.Add(custom);

        TaskDialogButton result = TaskDialog.Show(_configuration);

        Assert.Equal(custom, result);

        void ClickCustomButton(object? sender, EventArgs e)
        {
            custom.Click();
        }
    }

    [Fact]
    public void Click_CustomButtonNotInConfiguration_ThrowsException()
    {
        var custom = new TaskDialogButton("I am Special");
        
        _configuration.Created += ClickCustomButton;

        Assert.Throws<InvalidOperationException>(() => TaskDialog.Show(_configuration));

        void ClickCustomButton(object? sender, EventArgs e)
        {
            custom.Click();
        }
    }

    [Fact]
    public void Checked_NestedRadioButtonClicks_ThrowsException()
    {
        var firstRadio = new TaskDialogRadioButton("A");
        var secondRadio = new TaskDialogRadioButton("B");

        firstRadio.Checked = true;

        secondRadio.CheckedChanged += CheckFirstRadio;
        firstRadio.CheckedChanged += CheckSecondRadioAfterFirst;

        _configuration.RadioButtons.Add(firstRadio);
        _configuration.RadioButtons.Add(secondRadio);
        _configuration.Created += CheckSecondRadio;

        Assert.Throws<InvalidOperationException>(() => TaskDialog.Show(_configuration));

        void CheckSecondRadio(object? sender, EventArgs e)
        {
            secondRadio.Checked = true;
        }

        void CheckFirstRadio(object? sender, EventArgs e)
        {
            firstRadio.Checked = true;
        }

        void CheckSecondRadioAfterFirst(object? sender, EventArgs e)
        {
            secondRadio.Checked = true;
        }
    }

    [Fact]
    public void Checked_FirstCheckedInitiallyThenSecond_OthersUnchecked()
    {
        var firstRadio = new TaskDialogRadioButton("A");
        var secondRadio = new TaskDialogRadioButton("B");

        firstRadio.Checked = true;

        _configuration.RadioButtons.Add(firstRadio);
        _configuration.RadioButtons.Add(secondRadio);

        _configuration.Created += ClickSecondRadio;

        Assert.True(firstRadio.Checked);
        Assert.False(secondRadio.Checked);

        TaskDialog.Show(_configuration);
        
        Assert.False(firstRadio.Checked);
        Assert.True(secondRadio.Checked);

        void ClickSecondRadio(object? sender, EventArgs e)
        {
            secondRadio.Checked = true;

            _okButton.Click();
        }
    }

    [Fact]
    public void Text_ChangeCustomBeforeShown_IsValid()
    {
        var custom = new TaskDialogButton("Hello")
                     {
                         Text = "Bye"
                     };

        Assert.Equal("Bye", custom.Text);
    }
    
    [Fact]
    public void Text_ChangeStandard_ThrowsException() 
        => Assert.Throws<InvalidOperationException>(() => _okButton.Text = "Hello");

    [Fact]
    public void Text_ChangeRadio_IsValid()
    {
        var radio = new TaskDialogRadioButton("Hello")
                    {
                        Text = "Bye"
                    };

        Assert.Equal("Bye", radio.Text);
    }

    [Fact]
    public void Click_HelpButton_HelpInvoked()
    {
        bool helpInvoked = false;
        TaskDialogButton helpButton = TaskDialogButton.Help;
        _configuration.PushButtons.Add(helpButton);

        _configuration.Created += ClickHelpButton;
        _configuration.HelpRequested += HelpOut;

        TaskDialog.Show(_configuration);

        Assert.True(helpInvoked);

        void ClickHelpButton(object? sender, EventArgs e)
        {
            helpButton.Click();
        }

        void HelpOut(object? sender, EventArgs e)
        {
            helpInvoked = true;
            _okButton.Click();
        }
    }

    [Fact]
    public void Show_WithOwner_IsValid()
    {
        using (WindowHandle window = TestWindow.Create("Show_WithOwner_IsValid"))
        {
            _configuration.Owner = window;
            _configuration.StartupLocation = TaskDialogStartupLocation.CenterOwner;

            _configuration.Created += ClickOKButton;

            TaskDialog.Show(_configuration);
        }
    }

    [Fact]
    public void Click_DoesNotCloseDialog_DialogNotClosed()
    {
        bool stayedOpen = false;

        _okButton.ClosesDialog = false;
        _configuration.Created += ClickOKButton;
        _okButton.Clicked += ClickCancelButton;
        _cancelButton.Clicked += CloseDialog;

        TaskDialog.Show(_configuration);

        Assert.True(stayedOpen);

        void ClickCancelButton(object? sender, EventArgs e)
        {
            _cancelButton.Click();
        }

        void CloseDialog(object? sender, EventArgs e)
        {
            stayedOpen = true;
        }
    }

    [Fact]
    public void Navigate_AfterCreated_CreateFired()
    {
        bool navigated = false;
        _okButton.ClosesDialog = false;
        var okButton = TaskDialogButton.OK;

        var secondConfiguration = new TaskDialogConfiguration
                                  {
                                      CollapsedLabel = "Expand",
                                      ExpandedLabel = "Collapse",
                                      ExpandedText = "Here there be more text.",
                                      FooterText = "Feet Text",
                                      FooterIcon = TaskDialogIcon.Information,
                                      Text = "Main text be here.",
                                      InstructionText = "Respect Mah Authoritah!",
                                      Title = "Another Nice Title",
                                      PushButtons = { okButton }
                                  };

        secondConfiguration.Created += CloseSecond;
        _configuration.Created += NavigateToSecond;

        TaskDialog.Show(_configuration);

        Assert.True(navigated);

        void NavigateToSecond(object? sender, EventArgs e)
        {
            _configuration.Navigate(secondConfiguration);
        }

        void CloseSecond(object? sender, EventArgs e)
        {
            Assert.NotNull(secondConfiguration.Host);
            Assert.Equal(secondConfiguration, secondConfiguration.Host.AttachedConfiguration);
            Assert.Null(_configuration.Host);

            navigated = true;
            okButton.Click();
        }
    }

    [Fact]
    public void ProgressBar_SimulateLoading_IsValid()
    {
        var progressBar = new TaskDialogProgressBar
                          {
                              Minimum = 0,
                              Maximum = 100,
                              State = TaskDialogProgressBarState.Normal
                          };

        _configuration.IsExpanded = true;
        _configuration.ProgressBar = progressBar;
        _configuration.ExpansionMode = TaskDialogExpansionMode.ExpandFooter;
        _configuration.Created += async (_, _) =>
        {
            await foreach (int val in SimulateLoadAsync())
            {
                progressBar.Value = val;
                _configuration.ExpandedText = $"Progress: {val} %";
            }

            _okButton.Click();
        };

        TaskDialog.Show(_configuration);

        Assert.Equal(100, progressBar.Value);

        static async IAsyncEnumerable<int> SimulateLoadAsync()
        {
            await Task.Delay(100);

            for (int i = 0; i <= 100; i++)
            {
                yield return i;

                await Task.Delay(50);
            }
        }
    }

    [Fact]
    public void Enabled_SetToFalse_IsValid()
    {
        var radio = new TaskDialogRadioButton("Hello");

        _configuration.RadioButtons.Add(radio);
        _configuration.Created += DisableButtons;

        TaskDialog.Show(_configuration);

        Assert.False(_cancelButton.Enabled);
        Assert.False(radio.Enabled);

        void DisableButtons(object? sender, EventArgs e)
        {
            _cancelButton.Enabled = false;
            radio.Enabled = false;

            _okButton.Click();
        }
    }

    [Fact]
    public void IsElevated_SetToTrue_IsValid()
    {
        _okButton.IsElevated = true;

        _configuration.Created += ClickOKButton;

        TaskDialog.Show(_configuration);

        Assert.True(_okButton.IsElevated);
    }

    private void ClickOKButton(object? sender, EventArgs e)
    {
        _okButton.Click();
    }
}
