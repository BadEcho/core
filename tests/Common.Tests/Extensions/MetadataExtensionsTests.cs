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

using BadEcho.Extensibility;
using Xunit;

namespace BadEcho.Tests.Extensions;

/// <suppressions>
/// ReSharper disable UsageOfDefaultStructEquality
/// </suppressions>
public class MetadataExtensionsTests
{
    [Fact]
    public void BuildMetadata_Interface()
    {
        var expectedMetadata = new Dictionary<string, Type>
                               {
                                   {nameof(IMetadata.FirstProperty), typeof(int)},
                                   {nameof(IMetadata.SecondProperty), typeof(string)}
                               };

        var metadata = MetadataExtensions.BuildMetadata(typeof(IMetadata));

        Assert.True(expectedMetadata.SequenceEqual(metadata));
    }

    [Fact]
    public void BuildMetadata_Null()
    {
        var metadata = MetadataExtensions.BuildMetadata(null);

        Assert.True(!metadata.Any());
    }

    private interface IMetadata
    {
        int FirstProperty { get; }
        string SecondProperty { get; }
    }
}