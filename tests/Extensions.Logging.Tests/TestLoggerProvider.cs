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

using Microsoft.Extensions.Logging;

namespace BadEcho.Extensions.Logging.Tests;

internal sealed class TestLoggerProvider : ILoggerProvider
{
    public static event EventHandler<EventArgs>? Logged;
    
    public void Dispose()
    { }

    public ILogger CreateLogger(string categoryName)
    {
        return new TestLogger();
    }

    private sealed class TestLogger : ILogger, IDisposable
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Logged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
            => this;

        public void Dispose()
        { }
    }
}
