// -----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Interop;
using Xunit;

namespace BadEcho.Tests.Interop;

/// <summary>
/// These methods are less for testing the unmanaged functions in question (which one should assume Microsoft themselves have tested),
/// and more for testing my P/Invoke signatures.
/// </summary>
public class Gdi32Tests
{
    [Fact]
    public void GetDeviceCaps_PpiHeight_ReturnsValid()
    {
        using (DeviceContextHandle deviceContext = User32.GetDC(WindowHandle.InvalidHandle))
        {
            Assert.False(deviceContext.IsInvalid);

            Assert.True(Gdi32.GetDeviceCaps(deviceContext, DeviceInformation.PpiHeight) > 0);
        }
    }

    [Fact]
    public void GetStockObject_NullBrush_ReturnsValid()
    {
        IntPtr hNullBrush = Gdi32.GetStockObject(Gdi32.StockObjectBrushNull);

        Assert.NotEqual(IntPtr.Zero, hNullBrush);
    }
}
