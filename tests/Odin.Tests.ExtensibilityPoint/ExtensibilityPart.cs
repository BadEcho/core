//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Composition;

namespace BadEcho.Odin.Tests.ExtensibilityPoint
{
    [Export(typeof(IExtensibilityPart))]
    public sealed class ExtensibilityPart : IExtensibilityPart
    { }
}
