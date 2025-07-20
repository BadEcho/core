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

using Xunit;

namespace BadEcho.Tests;

/// <summary>
/// Provides an attribute that, when applied to a method, indicates that it is a fact that
/// should be run the test runner, unless it is being run by a GitHub Action.
/// </summary>
internal sealed class SkipOnGitHubFactAttribute : FactAttribute
{
    public SkipOnGitHubFactAttribute()
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_RUN_ID")))
            Skip = "Ignored on GitHub Actions runners";
    }
}
