//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using BadEcho.Odin.Extensibility;
using BadEcho.Omnified.Vision.Extensibility;

namespace BadEcho.Omnified.Vision.Statistics
{
    /// <summary>
    /// Provides a snap-in module granting vision to Omnified game statistics data.
    /// </summary>
    [FilterableFamily(FAMILY_ID, NAME)]
    public sealed class StatisticsModule : IVisionModule
    {
        private const string FAMILY_ID = "7191A268-4E04-431E-9806-F3228961637B";
        private const string NAME = "Statistics";

        /// <inheritdoc/>
        public Guid FamilyId
            => new(FAMILY_ID);

        /// <inheritdoc/>
        public string Name
            => NAME;

        /// <inheritdoc/>
        public AnchorPointLocation DefaultLocation
            => AnchorPointLocation.TopLeft;

        /// <inheritdoc/>
        public GrowthDirection GrowthDirection
            => GrowthDirection.Vertical;

        /// <inheritdoc/>
        public string MessageFile
            => "statistics.json";

        /// <inheritdoc/>
        public bool ProcessNewMessagesOnly
            => false;
    }
}
