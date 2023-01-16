//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Xunit;

namespace BadEcho.Presentation.Tests;

public class NameOfTests
{
    public string TextSizeProperty
    { get; set; } = string.Empty;

    [Fact]
    public void ReadDependencyPropertyName_ValidName_NameWithoutProperty() 
        => Assert.Equal("TextSize", NameOf.ReadDependencyPropertyName(() => TextSizeProperty));
}