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

using Microsoft.Xna.Framework.Content.Pipeline;

namespace BadEcho.Game.Pipeline;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to an asset's importation and processing into the
/// content pipeline.
/// </summary>
internal static class ContentContextExtensions
{
    private static readonly List<string> _OutputPaths = [];

    /// <summary>
    /// Logs a message during the importation of an asset to the content pipeline.
    /// </summary>
    /// <param name="context">The current game asset importer's context.</param>
    /// <param name="message">The message to log.</param>
    public static void Log(this ContentImporterContext context, string message)
    {
        Require.NotNull(context, nameof(context));

        context.Logger.LogMessage(message);
    }

    /// <summary>
    /// Logs a message during the processing of an asset to the content pipeline.
    /// </summary>
    /// <param name="context">The current game asset's processor's context.</param>
    /// <param name="message">The message to log.</param>
    public static void Log(this ContentProcessorContext context, string message)
    {
        Require.NotNull(context, nameof(context));

        context.Logger.LogMessage(message);
    }

    /// <summary>
    /// Builds a path to an intermediate directory that mirrors the source path to an asset.
    /// </summary>
    /// <param name="context">The current game asset's processor's context.</param>
    /// <param name="sourcePath">The source path to an asset to get an intermediate directory for.</param>
    /// <returns>A path to an intermediate directory that mirrors <c>sourcePath</c>.</returns>
    public static string ResolveIntermediatePath(this ContentProcessorContext context, string sourcePath)
    {
        Require.NotNull(context, nameof(context));

        string basePath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}";
        sourcePath = $"{Path.GetDirectoryName(sourcePath) ?? string.Empty}{Path.DirectorySeparatorChar}";

        if (!Uri.TryCreate(sourcePath, UriKind.Absolute, out Uri? uri))
            return basePath;

        uri = new Uri(basePath).MakeRelativeUri(uri);
        string pathToSource = Uri.UnescapeDataString(uri.ToString());

        return Path.Combine(context.IntermediateDirectory, pathToSource);
    }

    /// <summary>
    /// Builds a unique path to an output directory for an asset that was generated during the processing of another asset.
    /// </summary>
    /// <param name="context">The current game asset's processor's context.</param>
    /// <param name="outputFileName">The file name for the asset that was generated during processing of another asset.</param>
    /// <returns>An output path to use when building the generated asset whose name is <c>outputFileName</c>.</returns>
    public static string ResolveOutputPath(this ContentProcessorContext context, string outputFileName)
    {
        Require.NotNull(context, nameof(context));

        string baseOutputPath = Path.Combine(Path.GetDirectoryName(context.OutputFilename) ?? string.Empty,
                                             Path.GetFileNameWithoutExtension(outputFileName));
        int assetIndex = 0;
        string outputPath;

        do
        {
            outputPath = $"{baseOutputPath}_{assetIndex}";
            assetIndex++;
        } while (_OutputPaths.Contains(outputPath));

        _OutputPaths.Add(outputPath);

        return outputPath;
    }
}
