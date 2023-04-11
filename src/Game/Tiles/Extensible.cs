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

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides tile-related content that supports custom property attribution.
/// </summary>
public abstract class Extensible
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Extensible"/> class.
    /// </summary>
    /// <param name="customProperties">The content's custom properties.</param>
    protected Extensible(CustomProperties customProperties)
    {
        Require.NotNull(customProperties, nameof(customProperties));

        CustomProperties = customProperties;
    }

    /// <summary>
    /// Gets the content's custom properties.
    /// </summary>
    public CustomProperties CustomProperties
    { get; }
}
