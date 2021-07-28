//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Odin;

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
        /// Initializes a new instance of the <see cref="CoordinateStatisticViewModel"/> class.
        /// </summary>
        /// <param name="statistic">The coordinate statistic to bind to the view model.</param>
        public CoordinateStatisticViewModel(CoordinateStatistic statistic)
        {
            Require.NotNull(statistic, nameof(statistic));

            Bind(statistic);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateStatisticViewModel"/> class.
        /// </summary>
        public CoordinateStatisticViewModel()
        { }

        /// <summary>
        /// Gets or sets the displayed x-coordinate value for the bound statistic.
        /// </summary>
        public float X
        {
            get => _x;
            set => NotifyIfChanged(ref _x, value);
        }

        /// <summary>
        /// Gets or sets the displayed y-coordinate value for the bound statistic.
        /// </summary>
        public float Y
        {
            get => _y;
            set => NotifyIfChanged(ref _y, value);
        }

        /// <summary>
        /// Gets or sets the displayed z-coordinate value for the bound statistic.
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
        protected override void OnUnbound(CoordinateStatistic model)
        {
            base.OnUnbound(model);

            X = default;
            Y = default;
            Z = default;
        }
    }
}
