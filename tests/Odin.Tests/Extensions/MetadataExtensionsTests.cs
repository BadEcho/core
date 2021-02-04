//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using BadEcho.Odin.Extensibility;
using Xunit;

namespace BadEcho.Odin.Tests.Extensions
{
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
}
