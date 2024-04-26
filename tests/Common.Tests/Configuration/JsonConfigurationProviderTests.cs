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

    private class FakeExtensionData;
    private class FakeExtensionDataImpl : FakeExtensionData;
}