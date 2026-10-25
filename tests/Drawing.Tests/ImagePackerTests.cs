// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Drawing;
using SkiaSharp;
using System.Drawing;

namespace Drawing.Tests;

public class ImagePackerTests : IDisposable
{
    private readonly string _directory
        = Path.Combine(Path.GetTempPath(), "BadEcho.Drawing.Tests+", Guid.NewGuid().ToString("N"));

    public ImagePackerTests()
    {
        Directory.CreateDirectory(_directory);
    }

    [Fact]
    public void Pack_NonSquareMaximum_OutputWithinMaximum()
    {
        string image = CreateImage("wide.png", 60, 10, SKColors.Red);
        var packer = new ImagePacker();

        packer.Pack([image], 64, 16);

        Assert.Equal(new Size(64, 16), packer.OutputSize);
        AssertSavedSize(packer, packer.OutputSize);
    }

    [Fact]
    public void Pack_NonPowerOfTwoMaximum_OutputWithinMaximum()
    {
        string image = CreateImage("square.png", 50, 50, SKColors.Red);
        var packer = new ImagePacker();

        packer.Pack([image], 60, 60);

        Assert.Equal(new Size(60, 60), packer.OutputSize);
        AssertSavedSize(packer, packer.OutputSize);
    }

    public void Dispose()
    {
        Directory.Delete(_directory, true);
    }

    private string CreateImage(string fileName, int width, int height, SKColor color)
    {
        string path = Path.Combine(_directory, fileName);

        using var bitmap = new SKBitmap(width, height);
        bitmap.Erase(color);

        using SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        using FileStream stream = File.Create(path);
        data.SaveTo(stream);

        return path;
    }

    private void AssertSavedSize(ImagePacker packer, Size expected)
    {
        string outputPath = Path.Combine(_directory, $"{Guid.NewGuid():N}.png");

        packer.Save(outputPath);

        using SKBitmap saved = SKBitmap.Decode(outputPath);

        Assert.Equal(expected, new Size(saved.Width, saved.Height));
    }
}
