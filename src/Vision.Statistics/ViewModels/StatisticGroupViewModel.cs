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
/// Provides a view model that facilitates the display of a grouping of similar statistics exported from an Omnified
/// game.
/// </summary>
internal sealed class StatisticGroupViewModel : StatisticViewModel<StatisticGroup>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticGroupViewModel"/> class.
    /// </summary>
    /// <param name="group">The statistic group to bind to the view model.</param>
    public StatisticGroupViewModel(StatisticGroup group)
    {
        Require.NotNull(group, nameof(group));

        Bind(group);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticGroupViewModel"/> class.
    /// </summary>
    public StatisticGroupViewModel()
    { }
        
    /// <summary>
    /// Gets a collection view model which the individual statistics comprising this group are bound to.
    /// </summary>
    public StatisticsViewModel Statistics
    { get; init; } = new();

    /// <inheritdoc/>
    protected override void OnBinding(StatisticGroup model)
    {
        base.OnBinding(model);

        Statistics.Bind(model.Statistics);
    }

    /// <inheritdoc/>
    protected override void OnUnbound(StatisticGroup model)
    {
        base.OnUnbound(model);

        Statistics.Unbind();
    }
}