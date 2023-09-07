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

using BadEcho.Extensions;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;
using System.Resources;
using BadEcho.Game.Properties;

namespace BadEcho.Game.Effects;

/// <summary>
/// Provides access to shader effect resources.
/// </summary>
public static class Shaders
{
    private static readonly ResourceManager _Manager = new("BadEcho.Game.Effects.Shaders",
                                                           typeof(Shaders).Assembly);
    /// <summary>
    /// Gets the data for a <see cref="SpriteBatch"/> effect that allows control over the alpha channel
    /// of all sprites in a batch.
    /// </summary>
    public static byte[] AlphaSpriteEffect
        => GetStreamBytes(nameof(AlphaSpriteEffect));

    private static byte[] GetStreamBytes(string name)
    {
        UnmanagedMemoryStream stream
            = _Manager.GetStream(name, CultureInfo.InvariantCulture)
              ?? throw new BadImageFormatException(Strings.EffectMissingResource.InvariantFormat(name));

        return stream.ToArray();
    }
}