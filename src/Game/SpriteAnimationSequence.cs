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

namespace BadEcho.Game;

/// <summary>
/// Provides the frames in an animation sequence for a sprite.
/// </summary>
/// <param name="Name">The name of the animation sequence.</param>
/// <param name="StartFrame">The index of the first frame in the animation sequence.</param>
/// <param name="EndFrame">The index of the last frame in the animation sequence.</param>
/// <param name="Duration">The amount of time each frame in the animation sequence should be displayed.</param>
public sealed record SpriteAnimationSequence(string Name, int StartFrame, int EndFrame, TimeSpan Duration);