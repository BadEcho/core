//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace BadEcho.Game.Tests;

/// <summary>
/// Provides a content processor context stub to use for unit testing content processors.
/// </summary>
internal sealed class TestContentProcessorContext : ContentProcessorContext
{
    public TestContentProcessorContext()
    {
        if (!Directory.Exists(IntermediateDirectory))
            Directory.CreateDirectory(IntermediateDirectory);
    }

    /// <inheritdoc />
    public override void AddDependency(string filename)
    { }

    /// <inheritdoc />
    public override void AddOutputFile(string filename)
    { }

    /// <inheritdoc />
    public override TOutput BuildAndLoadAsset<TInput, TOutput>(ExternalReference<TInput> sourceAsset,
                                                               string processorName,
                                                               OpaqueDataDictionary processorParameters,
                                                               string importerName)
    {
        return (TOutput) RuntimeHelpers.GetUninitializedObject(typeof(TOutput));
    }

    /// <inheritdoc />
    public override ExternalReference<TOutput> BuildAsset<TInput, TOutput>(ExternalReference<TInput> sourceAsset,
                                                                           string processorName,
                                                                           OpaqueDataDictionary processorParameters,
                                                                           string importerName,
                                                                           string assetName)
    {
        return new ExternalReference<TOutput>();
    }

    /// <inheritdoc />
    public override TOutput Convert<TInput, TOutput>(TInput input, string processorName, OpaqueDataDictionary processorParameters)
    {
        return (TOutput) RuntimeHelpers.GetUninitializedObject(typeof(TOutput));
    }

    /// <inheritdoc />
    public override string BuildConfiguration
        => string.Empty;

    /// <inheritdoc />
    public override string IntermediateDirectory
        => Path.Combine(GetContentPath(), "obj\\tests");

    /// <inheritdoc />
    public override ContentBuildLogger Logger
        => new PipelineBuildLogger();

    /// <inheritdoc />
    public override ContentIdentity SourceIdentity
        => new();

    /// <inheritdoc />
    public override string OutputDirectory
        => "Content";

    /// <inheritdoc />
    public override string OutputFilename
        => string.Empty;

    /// <inheritdoc />
    public override OpaqueDataDictionary Parameters
        => [];

    /// <inheritdoc />
    public override TargetPlatform TargetPlatform 
        => TargetPlatform.DesktopGL;

    /// <inheritdoc />
    public override GraphicsProfile TargetProfile
        => GraphicsProfile.HiDef;

    private static string GetContentPath([CallerFilePath] string rootPath = "")
        => $"{Path.GetDirectoryName(rootPath)}\\Content\\";
}
