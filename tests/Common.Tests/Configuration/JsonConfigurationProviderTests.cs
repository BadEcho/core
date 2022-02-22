//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

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

        Assert.NotNull(configurationProvider.GetConfiguration<FakeData>());
    }

    [Fact]
    public void GetConfiguration_SectionName()
    {
        var configurationProvider = new FakeProvider();
        var single = configurationProvider.GetConfiguration<FakeExtensionData>("single");

        Assert.NotNull(single);
    }

    [Fact]
    public void GetConfiguration_ExtensionData()
    {
        var configurationProvider = new FakeExtensionProvider();
        var extensibleData = configurationProvider.GetConfiguration<FakeData>();
        var firstGroup = extensibleData.Group?.GetConfiguration<FakeExtensionData>("first");

        Assert.NotNull(firstGroup);

        var secondGroup = extensibleData.Group?.GetConfiguration<FakeExtensionDataImpl>("second");

        Assert.NotNull(secondGroup);
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
        public ExtensionDataStore<FakeExtensionData>? Group { get; set; }
    }

    private class FakeExtensionData
    { }

    private class FakeExtensionDataImpl : FakeExtensionData
    { }
}