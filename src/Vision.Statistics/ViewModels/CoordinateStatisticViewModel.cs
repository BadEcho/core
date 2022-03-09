//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Vision.Statistics.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display of an individual coordinate triplet statistic exported from an
/// Omnified game.
/// </summary>
internal sealed class CoordinateStatisticViewModel : StatisticViewModel<CoordinateStatistic>
{
    private double _x;
    private double _y;
    private double _z;

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
    public double X
    {
        get => _x;
        set => NotifyIfChanged(ref _x, value);
    }

    /// <summary>
    /// Gets or sets the displayed y-coordinate value for the bound statistic.
    /// </summary>
    public double Y
    {
        get => _y;
        set => NotifyIfChanged(ref _y, value);
    }

    /// <summary>
    /// Gets or sets the displayed z-coordinate value for the bound statistic.
    /// </summary>
    public double Z
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