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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadEcho.Game;

/// <summary>
/// Defines an element that supports basic input processing.
/// </summary>
public interface IInputElement
{
    /// <summary>
    /// Gets a value indicating whether the mouse pointer is located over the element.
    /// </summary>
    bool IsMouseOver { get; }



    // Move other input related properties here.
}
