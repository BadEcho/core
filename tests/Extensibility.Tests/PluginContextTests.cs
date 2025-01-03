//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensibility.Hosting;
using Xunit;

namespace BadEcho.Extensibility.Tests;

/// <suppressions>
/// ReSharper disable UnusedAutoPropertyAccessor.Local
/// ReSharper disable AssignNullToNotNullAttribute
/// </suppressions>
public class PluginContextTests
{
    private readonly PluginContext _context;

    public PluginContextTests()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "testPlugins");
        var strategy = new GlobalPluginContextStrategy(path);

        _context = new PluginContext(strategy);
    }

    [Fact]
    public void Inject_PluggablePart_FakePartsImported()
    {
        var pluggablePart = new PluggablePart();

        _context.Inject(pluggablePart);

        Assert.NotNull(pluggablePart.FakeParts);
        Assert.NotEmpty(pluggablePart.FakeParts);
    }

    [Fact]
    public void Load_IFakePart_NotEmpty()
    {
        var parts = _context.Load<IFakePart>();

        Assert.NotEmpty(parts);
    }

    [Fact]
    public void Load_NonExistentLazy_NotNullButEmpty()
    {
        var parts = _context.Load<Lazy<ICloneable>>();

        Assert.NotNull(parts);
        Assert.Empty(parts);
    }
}