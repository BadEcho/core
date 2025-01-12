//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Drawing;
using BadEcho.Properties;
using SkiaSharp;

namespace BadEcho.Drawing;

/// <summary>
/// Generates an image comprising multiple other images, packed together as tightly as practicable.
/// </summary>
public sealed class ImagePacker
{
    private readonly Dictionary<string, SKBitmap> _images = [];
    private readonly Dictionary<string, Size> _imageSizes = [];
    private readonly Dictionary<string, Rectangle> _packedAreas = [];

    private List<string> _imagePaths = [];
    
    private SKBitmap? _outputImage;
    
    /// <summary>
    /// Gets a mapping between individual image file names and the region of the generated image they've
    /// been packed into.
    /// </summary>
    public IDictionary<string, Rectangle> PackedAreas
        => _packedAreas;

    /// <summary>
    /// Gets or sets the spacing to insert between individual packed images.
    /// </summary>
    public int Spacing
    { get; set; }

    /// <summary>
    /// Gets the size of the output image containing the packed images.
    /// </summary>
    public Size OutputSize
    { get; private set; }
    
    /// <summary>
    /// Packs the specified images into a single image within the specified constraints.
    /// </summary>
    /// <param name="imagePaths">Paths to the images to pack.</param>
    /// <param name="maximumWidth">The maximum width of the generated packed image.</param>
    /// <param name="maximumHeight">The maximum height of the generated packed image.</param>
    /// 
    public void Pack(IEnumerable<string> imagePaths, int maximumWidth, int maximumHeight)
    {
        Require.NotNull(imagePaths, nameof(imagePaths));
        
        _images.Clear();
        _imageSizes.Clear();
        _packedAreas.Clear();
        
        var maximumSize = new Size(maximumWidth, maximumHeight);
        OutputSize = maximumSize;
        _imagePaths = imagePaths.ToList();

        foreach (string image in _imagePaths)
        {
            using (var fileStream = new SKFileStream(image))
            {
                var bitmap = SKBitmap.Decode(fileStream);

                _images.Add(image, bitmap);
                _imageSizes.Add(image, new Size(bitmap.Width, bitmap.Height));
            }
        }

        _imagePaths.Sort((firstImage, secondImage) =>
        {
            Size firstSize = _imageSizes[firstImage];
            Size secondSize = _imageSizes[secondImage];

            int comparison = secondSize.Width.CompareTo(firstSize.Width);

            return comparison != 0 ? comparison : secondSize.Height.CompareTo(firstSize.Height);
        });

        PackRectangles();

        _outputImage = CreateOutputImage();

        var keys = new string[_packedAreas.Keys.Count];

        _packedAreas.Keys.CopyTo(keys, 0);

        foreach (string key in keys)
        {
            Size size = _imageSizes[key];
            Rectangle packedArea = _packedAreas[key];

            packedArea.Width = size.Width;
            packedArea.Height = size.Height;

            _packedAreas[key] = packedArea;
        }
        
        _images.Clear();
        _imageSizes.Clear();
    }

    /// <summary>
    /// Saves the generated packed image to the specified path.
    /// </summary>
    /// <param name="outputPath">The path to save the packed image to.</param>
    public void Save(string outputPath)
    {
        if (_outputImage == null)
            throw new InvalidOperationException(Strings.NoPackedImageGenerated);

        using (SKData imageStream = _outputImage.Encode(SKEncodedImageFormat.Png, 100))
        {
            using (FileStream fileStream = File.Create(outputPath))
            {
                imageStream.SaveTo(fileStream);
            }
        }
    }

    private static int FindNextPowerOfTwo(int k)
    {
        k--;

        for (int i = 1; i < sizeof(int) * 8; i <<= 1)
        {
            k |= k >> i;
        }

        return k + 1;
    }

    private void PackRectangles()
    {
        Dictionary<string, Rectangle> packedAreas = [];

        Size smallestSize = new(_imageSizes.Min(kv => kv.Value.Width),
                                _imageSizes.Min(kv => kv.Value.Height));

        Size sizeToAttempt = OutputSize;
        bool shrinkVertical = false;

        while (true)
        {
            packedAreas.Clear();

            if (!AttemptPack(sizeToAttempt, packedAreas))
            {
                if (_packedAreas.Count == 0)
                    throw new InvalidOperationException(Strings.PackedImageSizeTooSmall);

                if (shrinkVertical)
                    return;

                shrinkVertical = true;
                sizeToAttempt = new Size(smallestSize.Width + Spacing * 2, smallestSize.Height + Spacing * 2);
                
                continue;
            }

            _packedAreas.Clear();

            foreach (var packedArea in packedAreas)
            {
                _packedAreas.Add(packedArea.Key, packedArea.Value);
            }

            sizeToAttempt = AdjustSize(shrinkVertical);

            if (sizeToAttempt == OutputSize)
            {
                if (shrinkVertical)
                    return;

                shrinkVertical = true;
            }

            OutputSize = sizeToAttempt;

            if (!shrinkVertical)
                sizeToAttempt.Width -= smallestSize.Width;

            sizeToAttempt.Height -= smallestSize.Height;
        }
    }

    private bool AttemptPack(Size sizeToAttempt, Dictionary<string, Rectangle> packedAreas)
    {
        var rectanglePacker = new RectanglePacker(sizeToAttempt);

        foreach (string image in _imagePaths)
        {
            Size imageSize = _imageSizes[image];
            Size imageSizeWithPadding = new (imageSize.Width + Spacing, imageSize.Height + Spacing);

            if (!rectanglePacker.TryPack(imageSizeWithPadding, out Point origin))
                return false;

            packedAreas.Add(image, new Rectangle(origin, imageSizeWithPadding));
        }

        return true;
    }
    
    private Size AdjustSize(bool shrinkVertical)
    {
        var sizeToAttempt = new Size(_packedAreas.Max(kv => kv.Value.Right),
                                     _packedAreas.Max(kv => kv.Value.Bottom));
        if (!shrinkVertical)
            sizeToAttempt.Width -= Spacing;

        sizeToAttempt.Height -= Spacing;

        sizeToAttempt = new Size(FindNextPowerOfTwo(sizeToAttempt.Width),
                                 FindNextPowerOfTwo(sizeToAttempt.Height));

        int maxDimension = Math.Max(sizeToAttempt.Width, sizeToAttempt.Height);

        sizeToAttempt = new Size(maxDimension, maxDimension);

        return sizeToAttempt;
    }

    private SKBitmap CreateOutputImage()
    {
        var outputImage 
            = new SKBitmap(OutputSize.Width, OutputSize.Height, SKImageInfo.PlatformColorType, SKAlphaType.Unpremul);

        foreach (string image in _imagePaths)
        {
            Rectangle sourceArea = _packedAreas[image];
            SKBitmap bitmap = _images[image];

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    outputImage.SetPixel(sourceArea.X + x, sourceArea.Y + y, bitmap.GetPixel(x, y));
                }
            }
        }

        return outputImage;
    }
}
