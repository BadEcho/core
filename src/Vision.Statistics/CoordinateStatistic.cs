//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Omnified.Vision.Statistics
{
    /// <summary>
    /// Provides an individual statistic exported from an Omnified game concerning a coordinate
    /// triplet value.
    /// </summary>
    public sealed class CoordinateStatistic : Statistic
    {
        /// <summary>
        /// Gets or sets the value for the X coordinate.
        /// </summary>
        public float X
        { get; set; }

        /// <summary>
        /// Gets or sets the value for the Y coordinate.
        /// </summary>
        public float Y
        { get; set; }

        /// <summary>
        /// Gets or sets the value for the Z coordinate.
        /// </summary>
        public float Z
        { get; set; }
    }
}
