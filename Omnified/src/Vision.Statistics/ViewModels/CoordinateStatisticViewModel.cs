//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Omnified.Vision.Statistics.ViewModels
{
    /// <summary>
    /// Provides a view model that facilitates the display of an individual coordinate triplet statistic exported from an
    /// Omnified game.
    /// </summary>
    internal sealed class CoordinateStatisticViewModel : StatisticViewModel<CoordinateStatistic>
    {
        private float _x;
        private float _y;
        private float _z;

        /// <summary>
        /// Gets or sets the displayed X coordinate value for the bound statistic.
        /// </summary>
        public float X
        {
            get => _x;
            set => NotifyIfChanged(ref _x, value);
        }

        /// <summary>
        /// Gets or sets the displayed Y coordinate value for the bound statistic.
        /// </summary>
        public float Y
        {
            get => _y;
            set => NotifyIfChanged(ref _y, value);
        }

        /// <summary>
        /// Gets or sets the displayed Z coordinate value for the bound statistic.
        /// </summary>
        public float Z
        {
            get => _z;
            set => NotifyIfChanged(ref _z, value);
        }

        /// <inheritdoc/>
        protected override void OnBinding(CoordinateStatistic model)
        {
            base.OnBinding(model);

            X = model.X;
            Y = model.Y;
            Z = model.Z;
        }

        /// <inheritdoc/>
        protected override void OnUnbind(CoordinateStatistic model)
        {
            base.OnUnbind(model);

            X = default;
            Y = default;
            Z = default;
        }
    }
}
