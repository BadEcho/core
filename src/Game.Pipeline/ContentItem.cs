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

using BadEcho.Extensions;
using BadEcho.Game.Pipeline.Properties;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace BadEcho.Game.Pipeline;

/// <summary>
/// Provides typed raw data for a game asset.
/// </summary>
/// <typeparam name="T">The type of asset data described by the content.</typeparam>
public abstract class ContentItem<T> : ContentItem, IContentItem
{
    private readonly Dictionary<string, ContentItem> _references = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentItem{T}"/> class.
    /// </summary>
    /// <param name="asset">The configuration data for the game asset.</param>
    protected ContentItem(T asset)
        => Asset = asset;

    /// <summary>
    /// Gets the configuration data for the game asset.
    /// </summary>
    public T Asset 
    { get; }

    /// <inheritdoc/>
    public void AddReference<TContent>(ContentProcessorContext context, 
                                       string sourcePath, 
                                       OpaqueDataDictionary processorParameters)
    {
        AddReference<TContent>(context, sourcePath, processorParameters, string.Empty);
    }

    /// <inheritdoc/>
    public void AddReference<TContent>(ContentProcessorContext context, 
                                       string sourcePath, 
                                       OpaqueDataDictionary processorParameters,
                                       string outputPath)
    {
        Require.NotNull(context, nameof(context));

        var sourceAsset = new ExternalReference<TContent>(sourcePath);

        var reference =
            context.BuildAsset<TContent, TContent>(sourceAsset,
                                                   string.Empty,
                                                   processorParameters,
                                                   string.Empty,
                                                   outputPath);
        _references.Add(sourcePath, reference);
    }

    /// <inheritdoc/>
    public ExternalReference<TContent> GetReference<TContent>(string filename)
    {
        if (!_references.TryGetValue(filename, out ContentItem? contentItem))
            throw new ArgumentException(Strings.NoReferenceInContentItem.InvariantFormat(filename), nameof(filename));

        return (ExternalReference<TContent>) contentItem;
    }
}
