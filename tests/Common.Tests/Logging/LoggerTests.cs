//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.Tracing;
using BadEcho.Logging;
using Xunit;

namespace BadEcho.Tests.Logging;

/// <suppressions>
/// ReSharper disable LocalizableElement
/// ReSharper disable NotResolvedInText
/// </suppressions>
public class LoggerTests : IDisposable
{
    private readonly EventListenerStub _listener;

    public LoggerTests()
    {
        _listener = new EventListenerStub();

        _listener.EnableEvents(LogSource.Instance, EventLevel.LogAlways);
    }
        
    public void Dispose()
    {
        _listener.Dispose();
    }

    [Fact]
    public void Debug()
    {
        AssertEventWritten(() => Logger.Debug("The answer to life is 42."));
    }

    [Fact]
    public void Debug_Disabled()
    {
        DisableEventSource();
        AssertEventNotWritten(() => Logger.Debug("Nothing."));
    }

    [Fact]
    public void Info()
    {
        AssertEventWritten(() => Logger.Info("Hello there. This is an informational message."));
    }
        
    [Fact]
    public void Info_Disabled()
    {
        DisableEventSource();
        AssertEventNotWritten(() => Logger.Info("Nope."));
    }

    [Fact]
    public void Warning()
    {
        AssertEventWritten(
            () => Logger.Warning("Beware. All signs point to the likelihood that you are going to die horribly soon."));
    }

    [Fact]
    public void Warning_Disabled()
    {
        DisableEventSource();
        AssertEventNotWritten(() => Logger.Warning("Hello"));
    }

    [Fact]
    public void Error()
    {
        AssertEventWritten(() => Logger.Error("This does not compute. DOES NOT COMPUTE."));
    }

    [Fact]
    public void Error_Disabled()
    {
        DisableEventSource();
        AssertEventNotWritten(() => Logger.Error("Can't see this."));
    }

    [Fact]
    public void Error_ArgumentException()
    {
        var argumentException =
            new ArgumentException("The specified identifier does not exist. What kind of IDIOT are you? IDIOT!",
                                  "identifierValue");

        AssertEventWritten(() => Logger.Error("We messed up.", argumentException));
    }

    [Fact]
    public void Error_ArgumentException_Disabled()
    {
        DisableEventSource();
        AssertEventNotWritten(() => Logger.Error("No mess.", new Exception()));
    }

    [Fact]
    public void Critical()
    {
        AssertEventWritten(() => Logger.Critical("The computer has been unplugged from the wall!"));
    }

    [Fact]
    public void Critical_Disabled()
    {
        DisableEventSource();
        AssertEventNotWritten(() => Logger.Critical("Nope nope nope."));
    }

    [Fact]
    public void Critical_ArgumentException()
    {
        var argumentException =
            new ArgumentException("The specified identifier exists, but you were supposed to get it wrong! IDIOT!",
                                  "identifierValue");

        AssertEventWritten(() => Logger.Critical("We're doomed.", argumentException));
    }

    [Fact]
    public void Critical_ArgumentException_Disabled()
    {
        DisableEventSource();
        AssertEventNotWritten(() => Logger.Critical("Not doomed!", new Exception()));
    }

    private void AssertEventWritten(Action loggingAction)
    {
        Assert.Raises<EventWrittenEventArgs>(handler => _listener.EventWritten += handler,
                                             handler => _listener.EventWritten -= handler,
                                             loggingAction);
    }

    private void AssertEventNotWritten(Action loggingAction)
    {
        bool eventRaised = false;

        _listener.EventWritten += EventWrittenHandler;

        loggingAction();

        _listener.EventWritten -= EventWrittenHandler;

        Assert.False(eventRaised);

        void EventWrittenHandler(object? sender, EventWrittenEventArgs e)
        {
            eventRaised = true;
        }
    }

    private void DisableEventSource()
    {
        _listener.DisableEvents(LogSource.Instance);
        Logger.DisableDefaultListener();
    }

    private sealed class EventListenerStub : EventListener;
}