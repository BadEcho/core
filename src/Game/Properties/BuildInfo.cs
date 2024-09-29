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

namespace BadEcho.Game.Properties;

/// <summary>
/// Provides compile time constants involved with the build process.
/// </summary>
public static class BuildInfo
{
    /// <summary>
    /// The full public key corresponding to the private key used when signing assemblies with a strong name.
    /// </summary>
    public const string PublicKey =
        "00240000048000009400000006020000002400005253413100040000010001009d742c9fd689807db63691ee2f4fcb8aff5e9d271ed49ede6b823efc7f6f6d124e30e8b7d28b3ab37fb58f309493cb12bf5fefe5051941da0a9ae432f6aec81f881a69f624fc6d5fcf9d7eb7017479c8a3188a2619dfadf1801e91f3fbb0458e21977e61c23b20e5820fdfff60ba3486695a85c56571561f7a1dd1dcd028afa0";
}
