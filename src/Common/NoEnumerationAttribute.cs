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

namespace BadEcho;

/// <summary>
/// Indicates that an <see cref="System.Collections.IEnumerable"/>, passed in as a parameter, will not be
/// enumerated by the method.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class NoEnumerationAttribute : Attribute;