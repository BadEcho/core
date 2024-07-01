//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using BadEcho.Extensions;
using BadEcho.Game.Fonts;
using BadEcho.MsdfGenerator;
using BadEcho.Game.Pipeline.Properties;
using BadEcho.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace BadEcho.Game.Pipeline.Fonts;

/// <summary>
/// Provides a processor of multi-channel signed distance field font asset data for the content pipeline.
/// </summary>
[ContentProcessor(DisplayName = "Distance Field Font Processor - Bad Echo")]
public sealed class DistanceFieldFontProcessor : ContentProcessor<DistanceFieldFontContent, DistanceFieldFontContent>
{
    private const string UNICODE_PROPERTY_NAME = "unicode";

    private static readonly JsonSerializerOptions _OutputFileOptions
        = new()
          {
              PropertyNameCaseInsensitive = true,
              Converters =
              {
                  new EdgeRectangleConverter(),
                  new JsonFlattenedObjectConverter<FontCharacteristics>(2) // "atlas": {...}, "metrics": {...}
              },
              TypeInfoResolver = new DefaultJsonTypeInfoResolver
                                 {
                                     Modifiers = { ConvertUnicodeProperties }
                                 }
          };

    /// <summary>
    /// Acts as a modifier to the resolved initial <see cref="JsonTypeInfo"/> contract which normalizes the differences between
    /// the externally generated JSON layout file and our object model.
    /// </summary>
    private static Action<JsonTypeInfo> ConvertUnicodeProperties
        => static typeInfo =>
        {
            foreach (var property in typeInfo.Properties)
            {   // "Unicode" would be a confusing property name for our character values.
                // We don't care about the FontLayoutKerning.Unicode<1,2> properties, because the FontLayoutKerning type
                // isn't present in the final output object model.
                if (property.Name.Equals(nameof(FontGlyph.Character), StringComparison.OrdinalIgnoreCase))
                    property.Name = UNICODE_PROPERTY_NAME;
                // Unicode character values are encoded as JSON numbers in the generated layout file;
                // .NET's own data type for UTF-16 code values is more immediately useful to us.
                // This covers the FontGlyph.Character and FontLayoutKerning.Unicode<1,2> properties.
                if (property.Name.Contains(UNICODE_PROPERTY_NAME, StringComparison.OrdinalIgnoreCase))
                    property.CustomConverter = new JsonIntFuncConverter<char>(i => (char) i, c => c);
            }
        };

    /// <inheritdoc/>
    public override DistanceFieldFontContent Process(DistanceFieldFontContent input, ContentProcessorContext context)
    {
        Require.NotNull(input, nameof(input));
        Require.NotNull(context, nameof(context));

        context.Log(Strings.ProcessingDistanceFieldFont.InvariantFormat(input.Identity.SourceFilename));

        string intermediatePath = context.ResolveIntermediatePath(input.Identity.SourceFilename);

        if (!Directory.Exists(intermediatePath))
            Directory.CreateDirectory(intermediatePath);

        string charsetPath = CreateCharacterSetFile(input.Asset, input.Name, intermediatePath);
        string atlasPath = Path.Combine(intermediatePath, $"{input.Name}-atlas.png");
        string jsonPath = Path.Combine(intermediatePath, $"{input.Name}-layout.json");

        var fontConfiguration = new FontConfiguration
                                {
                                    FontPath = input.Asset.FontPath,
                                    CharsetPath = charsetPath,
                                    OutputPath = atlasPath,
                                    JsonPath = jsonPath,
                                    Range = (uint) input.Asset.Range,
                                    Resolution = (uint) input.Asset.Resolution
                                };

        DistanceFieldFontAtlas.Generate(fontConfiguration);

        if (!File.Exists(atlasPath))
            throw new PipelineException(Strings.DistanceFieldFontNoOutput);

        if (!File.Exists(jsonPath))
            throw new PipelineException(Strings.DistanceFieldFontNoJsonOutput);

        DistanceFieldFontContent output = ProcessOutput(input, jsonPath);

        // Because we've generated the atlas texture asset in an intermediate directory, we need to explicitly provide
        // an output path and asset name, otherwise MonoGame will mangle the output path (i.e., the output path
        // will end up being something like bin\Platform\obj\Platform\*\).
        string outputPath = context.ResolveOutputPath(atlasPath);
        output.AtlasPath = atlasPath;
        output.AddReference<Texture2DContent>(context,
                                              atlasPath,
                                              new OpaqueDataDictionary
                                              {   // The default color key is similar to the colors used for distance fields.
                                                  { nameof(TextureProcessor.ColorKeyEnabled), false },
                                                  // Our atlas image is already in a premultiplied format.
                                                  // Formatting it again would corrupt it.
                                                  { nameof(TextureProcessor.PremultiplyAlpha), false }
                                              },
                                              outputPath);

        context.Log(Strings.ProcessingFinished.InvariantFormat(input.Identity.SourceFilename));

        return output;
    }
    
    private static DistanceFieldFontContent ProcessOutput(DistanceFieldFontContent input, string jsonPath)
    {
        var layoutFileContents = File.ReadAllText(jsonPath);

        var fontLayout = JsonSerializer.Deserialize<FontLayout>(layoutFileContents, _OutputFileOptions)
                         ?? throw new JsonException(Strings.DistanceFieldFontJsonIsNull);
        
        return new DistanceFieldFontContent(input.Asset)
               {
                   Name = input.Name,
                   Identity = input.Identity,
                   Characteristics = fontLayout.Characteristics,
                   Glyphs = fontLayout.Glyphs.ToDictionary(kv => kv.Character),
                   Kernings = fontLayout.Kerning.ToDictionary(kv => new CharacterPair(kv.Unicode1, kv.Unicode2),
                                                              kv => new KerningPair(kv.Unicode1, kv.Unicode2, kv.Advance))
               };
    }
    
    private static string CreateCharacterSetFile(DistanceFieldFontAsset asset, string fontName, string intermediatePath)
    {
        if (asset.CharacterSet == null)
            return string.Empty;

        // The character set file consists of a single string of characters, surrounded by quotes.
        // We will want to escape any double quotes found within this string, as well as any \ chars.
        var charset = new string(asset.CharacterSet.ToArray())
                      .Replace("\\", "\\\\", StringComparison.OrdinalIgnoreCase)
                      .Replace("\"", "\\\"", StringComparison.OrdinalIgnoreCase);

        string charsetPath = Path.Combine(intermediatePath, $"{fontName}-charset.txt");

        File.WriteAllText(charsetPath, $"\"{charset}\"");

        return charsetPath;
    }
}
