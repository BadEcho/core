//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel.Composition;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Specifies when and how a part must be instantiated by Odin's Extensibility framework.
    /// </summary>
    public enum ImportPolicy
    {
        /// <summary>
        /// The part supports a <see cref="CreationPolicy.Any"/> creation policy.
        /// </summary>
        Any,
        /// <summary>
        /// The part supports a <see cref="CreationPolicy.Shared"/> creation policy.
        /// </summary>
        Shared,
        /// <summary>
        /// The part supports a <see cref="CreationPolicy.NonShared"/> creation policy.
        /// </summary>
        NonShared,
        /// <summary>
        /// The part supports initialization only through an exported part factory.
        /// </summary>
        FactoryOnly
    }
}
