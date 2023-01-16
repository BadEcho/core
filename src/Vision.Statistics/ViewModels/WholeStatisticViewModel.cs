//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
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
/// Provides a view model that facilitates the display of an individual whole statistic exported from an Omnified game.
/// </summary>
internal sealed class WholeStatisticViewModel : StatisticViewModel<WholeStatistic>
{
    private int _value;
    private bool _isCritical;

    /// <summary>
    /// Initializes a new instance of the <see cref="WholeStatisticViewModel"/> class.
    /// </summary>
    /// <param name="statistic">The whole statistic to bind to the view model.</param>
    public WholeStatisticViewModel(WholeStatistic statistic)
    {
        Require.NotNull(statistic, nameof(statistic));

        Bind(statistic);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WholeStatisticViewModel"/> class.
    /// </summary>
    public WholeStatisticViewModel()
    { }

    /// <summary>
    /// Gets or sets a value indicating if updates to the bound statistic are critical events.
    /// </summary>
    public bool IsCritical
    {
        get => _isCritical;
        set => NotifyIfChanged(ref _isCritical, value);
    }

    /// <summary>
    /// Gets or sets the displayed numeric value for the bound statistic.
    /// </summary>
    public int Value
    {
        get => _value;
        set => NotifyIfChanged(ref _value, value);
    }

    /// <inheritdoc/>
    protected override void OnBinding(WholeStatistic model)
    {
        base.OnBinding(model);

        IsCritical = model.IsCritical;
        Value = model.Value;
    }

    /// <inheritdoc/>
    protected override void OnUnbound(WholeStatistic model)
    {
        base.OnUnbound(model);

        IsCritical = false;
        Value = default;
    }
}