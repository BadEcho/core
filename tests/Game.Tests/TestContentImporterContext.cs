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

using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace BadEcho.Game.Tests;

/// <summary>
/// Provides a content processor context stub to use for unit testing content importers.
/// </summary>
internal sealed class TestContentImporterContext : ContentImporterContext
{
    /// <inheritdoc />
    public override void AddDependency(string filename)
    { }

    /// <inheritdoc />
    public override string IntermediateDirectory
        => "Content";

    /// <inheritdoc />
    public override ContentBuildLogger Logger
        => new PipelineBuildLogger();

    /// <inheritdoc />
    public override string OutputDirectory 
        => "Content";
}
