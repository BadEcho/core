//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Odin.Extensibility;

namespace BadEcho.Omnified.Vision.Extensibility
{
    /// <summary>
    /// Defines a snap-in module granting vision to Omnified data.
    /// </summary>
    public interface IVisionModule : IFilterableFamily
    {
        /// <summary>
        /// Gets the direction the module will grow from its anchor point.
        /// </summary>
        ModuleGrowthDirection GrowthDirection { get; }
    }
}
