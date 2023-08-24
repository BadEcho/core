//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.MsdfGenerator;
using Xunit;

namespace BadEcho.Game.Tests;

public class MsdfGeneratorTests
{
    [Fact]
    public void Generate_CreatesAtlas_OutputFilesExist()
    {
        Assert.True(File.Exists(@"..\..\tests\Game.Tests\Content\Fonts\Lato-Regular.ttf"));

        DistanceFieldFontAtlas.Generate(new FontConfiguration
                           {
                               fontPath = @"..\..\tests\Game.Tests\Content\Fonts\Lato-Regular.ttf",
                               jsonPath = "Lato-layout.json",
                               outputPath = "Lato-atlas.png",
                               range = 4,
                               resolution = 64
                           });

        Assert.True(File.Exists("Lato-atlas.png"));
        Assert.True(File.Exists("Lato-layout.json"));
    }
}
