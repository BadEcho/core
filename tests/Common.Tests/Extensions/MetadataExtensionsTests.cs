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

using BadEcho.Extensibility;
using Xunit;

namespace BadEcho.Tests.Extensions;

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