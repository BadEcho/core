//-----------------------------------------------------------------------
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
//-----------------------------------------------------------------------

namespace BadEcho;

/// <summary>
/// Indicates that disposal of the value returned by the method must be handled at the use site.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class MustDisposeResourceAttribute : Attribute;
