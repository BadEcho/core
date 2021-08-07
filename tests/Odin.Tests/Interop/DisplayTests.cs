//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Odin.Interop;
using Xunit;

namespace BadEcho.Odin.Tests.Interop
{
    public class DisplayTests
    {
        [Fact]
        public void Displays_NotEmpty()
        {
            var displays = Display.Devices;

            Assert.NotEmpty(displays);
        }
    }
}
