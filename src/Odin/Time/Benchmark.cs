//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace BadEcho.Odin.Time
{
    /// <summary>
    /// Provides a set of operations that aid in the measurement of code execution time.
    /// </summary>
    public static class Benchmark
    {
        /// <summary>
        /// Measures the time it takes to execute the provided action.
        /// </summary>
        /// <param name="action">The action to measure execution time for.</param>
        /// <returns>The time required for <c>action</c> to execute.</returns>
        public static TimeSpan MeasureTime(Action action)
        {
            Require.NotNull(action, nameof(action));

            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();

            return stopwatch.Elapsed;
        }
    }
}
