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

namespace BadEcho.Interop;

/// <summary>
/// Specifies the dots per inch setting for a thread, process, or window.
/// </summary>
internal enum DpiAwareness
{
    /// <summary>
    /// Invalid DPI awareness.
    /// </summary>
    Invalid = -1,
    /// <summary>
    /// DPI unaware. The process does not scale for DPI changes and is always assumed to have a scale factor of 100%.
    /// </summary>
    Unaware = 0,
    /// <summary>
    /// System DPI unaware. The process does not scale for DPI changes, but will query the DPI once and use that value
    /// for the lifetime of the process.
    /// </summary>
    SystemAware = 1,
    /// <summary>
    /// Per monitor DPI aware. The process checks for the DPI when it is created and adjusts the scale factor whenever
    /// the DPI changes.
    /// </summary>
    PerMonitorAware = 2
}
