//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Interop;
using Xunit;

namespace BadEcho.Tests.Interop;

public class ShellScalingTests
{
    [Fact]
    public void GetDpiForMonitor_DefaultDpiType_IsValid()
    {
        var monitors = UnmanagedHelper.EnumerateMonitors();

        Assert.Equal(ResultHandle.Success,
                     ShellScaling.GetDpiForMonitor(monitors.First(), MonitorDpiType.Effective, out _, out _));
    }
}