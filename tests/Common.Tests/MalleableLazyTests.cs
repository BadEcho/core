// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace BadEcho.Tests;

public class MalleableLazyTests
{
    [Fact]
    public void Value_ValueTypeNotOverridden_ReturnsFactoryValue()
    {
        var lazy = new MalleableLazy<int>(() => 7);

        Assert.False(lazy.IsValueCreated);
        Assert.Equal(7, lazy.Value);
        Assert.True(lazy.IsValueCreated);
    }

    [Fact]
    public void Value_ValueTypeOverridden_ReturnsOverridingValue()
    {
        bool factoryCalled = false;

        var lazy = new MalleableLazy<int>(() =>
                   {
                       factoryCalled = true;
                       return 7;
                   })
                   {
                       Value = 3
                   };

        Assert.True(lazy.IsValueCreated);
        Assert.Equal(3, lazy.Value);
        Assert.False(factoryCalled);
    }

    [Fact]
    public void Value_ValueTypeOverriddenWithDefault_ReturnsDefault()
    {
        var lazy = new MalleableLazy<int>(() => 7)
                   {
                       Value = 0
                   };

        Assert.Equal(0, lazy.Value);
    }

    [Fact]
    public void Value_ReferenceTypeOverriddenWithNull_ReturnsNull()
    {
        var lazy = new MalleableLazy<string?>(() => "Lazy")
                   {
                       Value = null
                   };

        Assert.True(lazy.IsValueCreated);
        Assert.Null(lazy.Value);
    }

    [Fact]
    public void Value_OverriddenAfterCreation_ReturnsOverridingValue()
    {
        var lazy = new MalleableLazy<string>(() => "Lazy");

        Assert.Equal("Lazy", lazy.Value);

        lazy.Value = "Overridden";

        Assert.Equal("Overridden", lazy.Value);
    }

    [Fact]
    public void Value_OverriddenBeforeCreation_FactoryNeverRuns()
    {
        var lazy = new MalleableLazy<string>(() => throw new InvalidOperationException())
                   {
                       Value = "Overridden"
                   };

        Assert.Equal("Overridden", lazy.Value);
        Assert.True(lazy.IsValueCreated);
    }
}
