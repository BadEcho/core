// -----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Interop;
using Xunit;

namespace BadEcho.Tests.Interop;

public class PopupMenuTests
{
    [Fact]
    public void AddItem_AfterRemoval_IdsAreUnique()
    {
        using var menu = new PopupMenu(new FakeWindowWrapper());

        MenuItem first = menu.AddItem("First");
        MenuItem second = menu.AddItem("Second");
        MenuItem third = menu.AddItem("Third");

        menu.RemoveItem(second);

        MenuItem fourth = menu.AddItem("Fourth");

        Assert.NotEqual(first.Id, fourth.Id);
        Assert.NotEqual(third.Id, fourth.Id);
    }

    private sealed class FakeWindowWrapper : WindowWrapper
    {
        public FakeWindowWrapper()
            : base(WindowHandle.InvalidHandle)
        { }
    }
}
