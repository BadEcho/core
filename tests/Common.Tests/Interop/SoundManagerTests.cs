// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Interop.Audio;
using Xunit;

namespace BadEcho.Tests.Interop;

public class SoundManagerTests
{
    [SkipOnGitHubFact]
    public void Mute_DefaultInput_ReturnsTrue()
    {
        var soundManager = new SoundManager();
        
        soundManager.DefaultInputDevice?.Mute = true;
        Assert.True(soundManager.DefaultInputDevice?.Mute);

        soundManager.DefaultInputDevice?.Mute = false;
        Assert.False(soundManager.DefaultInputDevice?.Mute);
    }
}
