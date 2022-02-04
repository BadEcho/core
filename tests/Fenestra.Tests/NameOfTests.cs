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

using Xunit;

namespace BadEcho.Fenestra.Tests;

public class NameOfTests
{
    public string TextSizeProperty
    { get; set; } = string.Empty;

    [Fact]
    public void ReadDependencyPropertyName_ValidName_NameWithoutProperty() 
        => Assert.Equal("TextSize", NameOf.ReadDependencyPropertyName(() => TextSizeProperty));
}