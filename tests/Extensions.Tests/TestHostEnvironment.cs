// -----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace BadEcho.Extensions.Tests;

internal sealed class TestHostEnvironment : IHostEnvironment
{
    public TestHostEnvironment(string contentRootPath)
    {
        ContentRootPath = contentRootPath;
    }

    public string EnvironmentName
    { get; set; } = Environments.Production;

    public string ApplicationName
    { get; set; } = "BadEcho.Extensions.Tests";

    public string ContentRootPath
    { get; set; }

    public IFileProvider ContentRootFileProvider
    { get; set; } = new NullFileProvider();
}