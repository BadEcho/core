// -----------------------------------------------------------------------
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

using Xunit;

namespace BadEcho.MsdfGenerator.Tests;

public class DistanceFieldFontAtlasTests
{
    [Fact]
    public void Generate_CreatesAtlas_OutputFilesExist()
    {
        var fontPath = "Lato-Regular.ttf";
        Assert.True(File.Exists(fontPath));

        DistanceFieldFontAtlas.Generate(new FontConfiguration
                                        {
                                            FontPath = "Lato-Regular.ttf",
                                            JsonPath = "Lato-layout.json",
                                            OutputPath = "Lato-atlas.png",
                                            Range = 4,
                                            Resolution = 64
                                        });

        Assert.True(File.Exists("Lato-atlas.png"));
        Assert.True(File.Exists("Lato-layout.json"));
    }
}