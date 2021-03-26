//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Xunit;

namespace BadEcho.Fenestra.Tests
{
    public class NameOfTests
    {
        public string TextSizeProperty
        { get; set; } = string.Empty;

        [Fact]
        public void ReadDependencyPropertyName_ValidName_NameWithoutProperty()
        {
            Assert.Equal("TextSize", NameOf.ReadDependencyPropertyName(() => TextSizeProperty));
        }
    }
}