//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using BadEcho.Vision.Windows;

namespace BadEcho.Vision;

/// <summary>
/// Provides the Vision application.
/// </summary>
public partial class App
{
    /// <inheritdoc/>
    protected override void OnStartup(StartupEventArgs e)
    {
        var window = new VisionWindow();
        var contextAssembler = new VisionContextAssembler();

        window.AssembleContext(contextAssembler);

        base.OnStartup(e);

        window.Show();
    }
}