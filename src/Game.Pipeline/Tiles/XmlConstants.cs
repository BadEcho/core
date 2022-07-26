//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides a set of constants related to the TMX map XML file format.
/// </summary>
internal static class XmlConstants
{
    /// <summary>
    /// The name for attributes defining the name of a TMX-related entity.
    /// </summary>
    internal static string NameAttribute
        => "name";

    /// <summary>
    /// The name for attributes defining the path to external files containing further configuration data.
    /// </summary>
    internal static string SourceAttribute
        => "source";

    /// <summary>
    /// The name for attributes defining the width of a TMX-related entity.
    /// </summary>
    internal static string WidthAttribute
        => "width";

    /// <summary>
    /// The name for attributes defining the height of a TMX-related entity.
    /// </summary>
    internal static string HeightAttribute
        => "height";

    /// <summary>
    /// The name for attributes defining the width of tiles.
    /// </summary>
    internal static string TileWidthAttribute
        => "tilewidth";

    /// <summary>
    /// The name for attributes defining the height of tiles.
    /// </summary>
    internal static string TileHeightAttribute
        => "tileheight";

    /// <summary>
    /// The name for attributes defining the horizontal offset of a TMX-related entity from the tile map's origin.
    /// </summary>
    internal static string OffsetXAttribute
        => "offsetx";

    /// <summary>
    /// The name for attributes defining the vertical offset of a TMX-related entity from the tile map's origin.
    /// </summary>
    internal static string OffsetYAttribute
        => "offsety";

    /// <summary>
    /// The name for attributes defining the visibility of a TMX-related entity.
    /// </summary>
    internal static string VisibleAttribute
        => "visible";

    /// <summary>
    /// The name for attributes defining the opacity of a TMX-related entity.
    /// </summary>
    internal static string OpacityAttribute
        => "opacity";

    /// <summary>
    /// The name for elements defining image data.
    /// </summary>
    internal static string ImageElement
        => "image";
}
