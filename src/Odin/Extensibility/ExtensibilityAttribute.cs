//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides an attribute that specifies that a type, property, field, or method provides a particular export
    /// to Odin's Extensibility framework.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
    public class ExtensibilityAttribute : Attribute
    { }
}
