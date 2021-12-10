//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Odin
{
    /// <summary>
    /// Indicates that an <see cref="System.Collections.IEnumerable"/>, passed in as a parameter, will not be
    /// enumerated by the method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NoEnumerationAttribute : Attribute
    { }
}