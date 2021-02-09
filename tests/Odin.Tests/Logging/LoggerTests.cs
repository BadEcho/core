//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.Tracing;
using BadEcho.Odin.Logging;
using Xunit;

namespace BadEcho.Odin.Tests.Logging
{
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
        public void Info()
        {
            AssertEventWritten(() => Logger.Info("Hello there. This is an informational message."));
        }

        [Fact]
        public void Warning()
        {
            AssertEventWritten(
                () => Logger.Warning("Beware. All signs point to the likelihood that you are going to die horribly soon."));
        }

        [Fact]
        public void Error()
        {
            AssertEventWritten(() => Logger.Error("This does not compute. DOES NOT COMPUTE."));
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
        public void Critical()
        {
            AssertEventWritten(() => Logger.Critical("The computer has been unplugged from the wall!"));
        }

        [Fact]
        public void Critical_ArgumentException()
        {
            var argumentException =
                new ArgumentException("The specified identifier exists, but you were supposed to get it wrong! IDIOT!",
                                      "identifierValue");

            AssertEventWritten(() => Logger.Critical("We're doomed.", argumentException));
        }

        private void AssertEventWritten(Action loggingAction)
        {
            Assert.Raises<EventWrittenEventArgs>(handler => _listener.EventWritten += handler,
                                                 handler => _listener.EventWritten -= handler,
                                                 loggingAction);
        }
    }
}
