//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Interop;
using Xunit;

namespace BadEcho.Tests.Interop;

public class ActivationContextTests
{
    [Fact]
    public void GetManifestResourceStream_AppManifest_ReturnsValid()
    {
        using (var stream = typeof(ActivationScope).Assembly.GetManifestResourceStream("BadEcho.Properties.app.manifest"))
        {
            Assert.NotNull(stream);
        }
    }
}
