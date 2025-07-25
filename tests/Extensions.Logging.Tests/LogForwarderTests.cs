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

using Logger = BadEcho.Logging.Logger;

namespace BadEcho.Extensions.Logging.Tests;

public class LogForwarderTests
{
    [Fact]
    public void Debug()
    {
        AssertEventLogged(() => Logger.Debug("Debugging stuff."));
    }
    
    [Fact]
    public void Info()
    {
        AssertEventLogged(() => Logger.Info("Info stuff."));
    }

    [Fact]
    public void Warning()
    {
        AssertEventLogged(() => Logger.Warning("A warning."));
    }

    [Fact]
    public void Error()
    {
        AssertEventLogged(() => Logger.Error("Oops!"));
    }

    [Fact]
    public void Error_ArgumentException()
    {
        try
        {
            throw new ArgumentException("This is not a real parameter. What are you smoking, friend?");
        }
        catch (ArgumentException ex)
        {
            AssertEventLogged(() => Logger.Error("Big error.", ex));
        }
    }

    [Fact]
    public void Critical()
    {
        AssertEventLogged(() => Logger.Critical("Horrific."));
    }


    [Fact]
    public void Critical_ArgumentException()
    {
        try
        {
            throw new ArgumentException("This is not a real parameter. What are you smoking, friend?");
        }
        catch (ArgumentException ex)
        {
            AssertEventLogged(() => Logger.Critical("Big error.", ex));
        }
    }

    private void AssertEventLogged(Action loggingAction)
    {
        Assert.Raises<EventArgs>(handler => TestLoggerProvider.Logged += handler,
                                 handler => TestLoggerProvider.Logged -= handler,
                                 loggingAction);
    }
}
