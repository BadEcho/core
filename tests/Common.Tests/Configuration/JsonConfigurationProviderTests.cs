// -----------------------------------------------------------------------
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
// -----------------------------------------------------------------------

using BadEcho.Configuration;
using Xunit;

namespace BadEcho.Tests.Configuration;

/// <suppressions>
/// ReSharper disable UnusedAutoPropertyAccessor.Local
/// </suppressions>
public class JsonConfigurationProviderTests
{
    private const string TEST_FILE = "testProvider.json";

    [Fact]
    public void GetConfiguration_RootSection()
    {
        var configurationProvider = new FakeProvider();
        var root = configurationProvider.GetConfiguration<FakeData>();

        Assert.NotNull(root);
        Assert.Equal("hi", root.SomeData);
    }

    [Fact]
    public void GetConfiguration_SectionName()
    {
        var configurationProvider = new FakeProvider();
        var single = configurationProvider.GetConfiguration<FakeExtensionData>("single");

        Assert.NotNull(single);
        Assert.Equal("two", single.One);
        Assert.Equal("eighty", single.Three);
    }

    [Fact]
    public void GetConfiguration_ExtensionData()
    {
        var configurationProvider = new FakeExtensionProvider();
        var extensibleData = configurationProvider.GetConfiguration<FakeData>();
        var firstGroup = extensibleData.Group?.GetConfiguration<FakeExtensionData>("first");
        
        Assert.NotNull(firstGroup);
        Assert.Equal("forty", firstGroup.One);
        Assert.Equal("eighty", firstGroup.Three);

        var secondGroup = extensibleData.Group?.GetConfiguration<FakeExtensionDataImpl>("second");

        Assert.NotNull(secondGroup);
        Assert.Equal("two", secondGroup.One);
        Assert.Equal("one", secondGroup.Three);
        Assert.Equal("three", secondGroup.Eight);
    }

    private class FakeProvider : JsonConfigurationProvider
    {
        protected override string SettingsFile
            => TEST_FILE;
    }

    private class FakeExtensionProvider : JsonConfigurationProvider<FakeExtensionData>
    {
        protected override string SettingsFile
            => TEST_FILE;
    }

    private class FakeData
    {
        public string? SomeData { get; init; }

        public ExtensionDataStore<FakeExtensionData>? Group { get; set; }
    }

    private class FakeExtensionData
    {
        public string? One { get; init; }
        public string? Three { get; init; }
    }

    private class FakeExtensionDataImpl : FakeExtensionData
    {
        public string? Eight { get; init; }
    }
}