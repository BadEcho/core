//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Odin;

namespace BadEcho.Omnified.Vision.Statistics.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display of an individual fractional statistic exported from an Omnified game.
/// </summary>
internal sealed class FractionalStatisticViewModel : StatisticViewModel<FractionalStatistic>
{
    private int _currentValue;
    private int _maximumValue;
    private double _percentageValue;
    private string _primaryBarColor = string.Empty;
    private string _secondaryBarColor = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="FractionalStatisticViewModel"/> class.
    /// </summary>
    /// <param name="statistic">The fractional statistic to bind to the view model.</param>
    public FractionalStatisticViewModel(FractionalStatistic statistic)
    {
        Require.NotNull(statistic, nameof(statistic));

        Bind(statistic);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FractionalStatisticViewModel"/> class.
    /// </summary>
    public FractionalStatisticViewModel()
    { }

    /// <summary>
    /// Gets or sets the displayed current numeric value for the bound statistic.
    /// </summary>
    public int CurrentValue
    {
        get => _currentValue;
        set
        {
            if (NotifyIfChanged(ref _currentValue, value))
                CalculatePercentage();
        }
    }

    /// <summary>
    /// Gets or sets the displayed maximum numeric value for the bound statistic.
    /// </summary>
    public int MaximumValue
    {
        get => _maximumValue;
        set
        {
            if (NotifyIfChanged(ref _maximumValue, value))
                CalculatePercentage();
        }
    }

    /// <summary>
    /// Gets or sets the bound statistic's value in the form of a percentage, which is calculated by dividing the
    /// <see cref="CurrentValue"/> by the <see cref="MaximumValue"/>.
    /// </summary>
    /// <remarks>
    /// This property is implemented fully with its own private backing field instead of a simple, read-only calculated value
    /// (as the summary above may have led you to believe) since we need property change notification in place so the controls
    /// bound to this update properly.
    /// </remarks>
    public double PercentageValue
    {
        get => _percentageValue;
        set => NotifyIfChanged(ref _percentageValue, value);
    }

    /// <summary>
    /// Gets or sets the primary (first half of a gradient) color that represents the bound statistic visually.
    /// </summary>
    public string PrimaryBarColor
    {
        get => _primaryBarColor;
        set => NotifyIfChanged(ref _primaryBarColor, value);
    }

    /// <summary>
    /// Gets or sets the secondary (second half of a gradient) color that represents the bound statistic visually.
    /// </summary>
    public string SecondaryBarColor
    {
        get => _secondaryBarColor;
        set => NotifyIfChanged(ref _secondaryBarColor, value);
    }

    /// <inheritdoc/>
    protected override void OnBinding(FractionalStatistic model)
    {
        base.OnBinding(model);

        CurrentValue = model.CurrentValue;
        MaximumValue = model.MaximumValue;
        PrimaryBarColor = model.PrimaryBarColor;
        SecondaryBarColor = model.SecondaryBarColor;
    }

    /// <inheritdoc/>
    protected override void OnUnbound(FractionalStatistic model)
    {
        base.OnUnbound(model);

        CurrentValue = default;
        MaximumValue = default;
        PrimaryBarColor = string.Empty;
        SecondaryBarColor = string.Empty;
    }

    private void CalculatePercentage()
    {
        var percentageValue = (double) CurrentValue / MaximumValue;
        // Negative percentages will cause gradient space to flip, which will cause the (what should be) transparent fill to be 
        // solid color instead.
        PercentageValue = Math.Max(percentageValue, 0.0);
    }
}