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
using Xunit;

namespace BadEcho.Tests.Interop;

public class WindowWrapperTests
{
    private readonly FakeWindowWrapper _wrapper = new();

    private bool _firstCalled, _secondCalled, _thirdCalled;

    [Fact]
    public void AddCallback_ThreeCallbacks_AllInvokedInOrder()
    {
        _wrapper.AddCallback(FirstCallback);
        _wrapper.AddCallback(SecondCallback);
        _wrapper.AddCallback(ThirdCallback);

        _wrapper.InvokeCallbacks();

        Assert.True(_thirdCalled);
    }

    [Fact]
    public void RemoveCallback_ThreeThenTwoCallbacks_TwoInvokedInOrder()
    {
        _wrapper.AddCallback(FirstCallback);
        _wrapper.AddCallback(SecondCallback);
        _wrapper.AddCallback(ThirdCallback);

        _wrapper.RemoveCallback(ThirdCallback);

        _wrapper.InvokeCallbacks();

        Assert.False(_thirdCalled);
    }

    private ProcedureResult FirstCallback(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        Assert.False(_secondCalled);
        Assert.False(_thirdCalled);

        _firstCalled = true;

        return new ProcedureResult(IntPtr.Zero, false);
    }

    private ProcedureResult SecondCallback(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        Assert.True(_firstCalled);
        Assert.False(_thirdCalled);

        _secondCalled = true;

        return new ProcedureResult(IntPtr.Zero, false);
    }

    private ProcedureResult ThirdCallback(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        Assert.True(_firstCalled);
        Assert.True(_secondCalled);

        _thirdCalled = true;

        return new ProcedureResult(IntPtr.Zero, false);
    }

    private sealed class FakeWindowWrapper : WindowWrapper
    {
        public FakeWindowWrapper() 
            : base(WindowHandle.InvalidHandle)
        {
        }

        public void InvokeCallbacks() 
            => WindowProcedure(IntPtr.Zero, 1, new IntPtr(2), new IntPtr(3));
    }
}
