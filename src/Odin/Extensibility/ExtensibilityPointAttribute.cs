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
    /// Provides an attribute that specifies that an assembly is an Extensibility point and therefore should be considered as
    /// a candidate source for exported parts.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Typically the assemblies from which exported parts are sourced from are disconnected assemblies, not referenced by the
    /// host assembly, and also physically isolated from other binaries in their own directory in order to cut down on the types
    /// we have to sift through during discovery.
    /// </para>
    /// <para>
    /// Sometimes, however, we might want to take advantage of the Extensibility framework's part discovery and loading capabilities
    /// in order to consume parts originating from referenced assemblies that are located outside of an isolated plugin directory.
    /// We refer to these types of assemblies as Extensibility points, and decorating them with this attribute will allow the Extensibility
    /// framework to bring in exported parts from them.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ExtensibilityPointAttribute : Attribute
    { }
}