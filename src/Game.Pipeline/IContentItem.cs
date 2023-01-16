//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework.Content.Pipeline;

namespace BadEcho.Game.Pipeline;

/// <summary>
/// Defines raw data for a game asset.
/// </summary>
public interface IContentItem
{
    /// <summary>
    /// Builds the specified external asset and adds a reference to it from this content.
    /// </summary>
    /// <typeparam name="TContent">The type of asset data being referenced by this content.</typeparam>
    /// <param name="context">The current content processing context.</param>
    /// <param name="filename">The path to the asset file being referenced.</param>
    /// <param name="processorParameters">Optional parameters used during the building of the external asset.</param>
    void AddReference<TContent>(ContentProcessorContext context, string filename, OpaqueDataDictionary processorParameters);

    /// <summary>
    /// Retrieves a previously referenced external asset from this content.
    /// </summary>
    /// <typeparam name="TContent">The type of externally referenced asset data.</typeparam>
    /// <param name="filename">The path to the externally referenced asset file.</param>
    /// <returns>A <see cref="ExternalReference{TContent}"/> instance for the referenced asset build from data found at <c>filename</c>.</returns>
    /// <exception cref="ArgumentException">
    /// <c>filename</c> does not point to an asset that was previously built by and added as a reference to this content.
    /// </exception>
    ExternalReference<TContent> GetReference<TContent>(string filename);
}
