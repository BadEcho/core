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

using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using BadEcho.Presentation.Properties;
using BadEcho.Extensions;

namespace BadEcho.Presentation;

/// <summary>
/// Provides a freezable object that facilitates the realization of the impermanent state shared between source and target in
/// an incremental or decremental fashion.
/// </summary>
/// <remarks>
/// <para>
/// In other words, this is an object that will cause changes to a source property to be propagated to a target property in
/// a stepped fashion. The target property's value will be incremented or decremented in a sequential fashion until it reaches
/// the new source property's value, with some slight delay introduced between steps in order to give it a visually pleasing
/// effect. In light of the progressive nature of values assigned during this sequence, the source and target properties
/// must be able to both be assigned and provide numeric values.
/// </para>
/// <para>
/// The delay introduced between steps is determined by the total amount of time the binder is configured to allow for a stepping
/// sequence to take. Given the complexity in dealing with the external overhead that is part and parcel with the complicated data
/// binding and visual presentation systems offered by WPF, this binder attempts its best to complete the sequence within the allowed
/// time frame. In order to achieve this, the binder maintains its own measurement of the time elapsed in a sequence's execution, allowing
/// for it to propagate values between source and target that fall inline with where the binder ought to be in the sequence.
/// </para>
/// <para>
/// By default, a stepped binder outputs <see cref="Double"/> floating-point values. To enable <see cref="Int32"/> support,
/// the provided <see cref="SteppingOptions"/> instance must have <see cref="SteppingOptions.IsInteger"/> set to true.
/// </para>
/// </remarks>
internal sealed class SteppedBinder : TransientBinder, IDisposable
{
    private readonly System.Timers.Timer _targetStepTimer
        = new();

    private readonly System.Timers.Timer _sourceStepTimer
        = new();

    private readonly Stopwatch _sequenceStopwatch 
        = new();

    private readonly Dispatcher _dispatcher
        = Dispatcher.CurrentDispatcher;

    private readonly TimeSpan _steppingDuration;
    private readonly bool _isInteger;
    private readonly int _minimumSteps;
    private readonly double _stepAmount;
    private readonly object? _unsetTargetValue;

    private bool _steppingEnabled;
    private double _endingStepValue;
    private double _startingStepValue;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SteppedBinder"/> class.
    /// </summary>
    /// <param name="targetObject">The target dependency object containing the property to bind.</param>
    /// <param name="targetProperty">The target dependency property to bind.</param>
    /// <param name="options">Stepping sequence options related to the timing of the stepped binder.</param>
    public SteppedBinder(DependencyObject targetObject,
                         DependencyProperty targetProperty,
                         SteppingOptions options)
        : base(targetObject, targetProperty, options)
    {
        Require.NotNull(options, nameof(options));
            
        if (options.SteppingDuration < TimeSpan.Zero)
            throw new ArgumentException(Strings.SteppingDurationCannotBeNegative, nameof(options));
            
        _steppingDuration = options.SteppingDuration;
        _minimumSteps = options.MinimumSteps;
        _isInteger = options.IsInteger;
        _stepAmount = options.StepAmount;
        _unsetTargetValue = targetProperty.DefaultMetadata.DefaultValue;

        _sourceStepTimer.Elapsed += HandleSourceStepTimerTick;
        _targetStepTimer.Elapsed += HandleTargetStepTimerTick;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SteppedBinder"/> class.
    /// </summary>
    private SteppedBinder()
    { }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _targetStepTimer.Dispose();
        _sourceStepTimer.Dispose();

        _disposed = true;
    }

    /// <inheritdoc/>
    protected override Freezable CreateBinder()
        => new SteppedBinder();

    /// <inheritdoc/>
    protected override void OnSourceChanged()
    {
        _sourceStepTimer.Stop();

        _steppingEnabled = Binding != null
            && Binding.DoBindingAction(() => StepChanges(SourceProperty, TargetProperty, _unsetTargetValue));

        if (!_steppingEnabled)
        {
            base.OnSourceChanged();
            return;
        }
            
        _targetStepTimer.Start();
    }

    /// <inheritdoc/>
    protected override void OnTargetChanged()
    {
        _targetStepTimer.Stop();

        _steppingEnabled = Binding != null
            && Binding.DoBindingAction(() => StepChanges(TargetProperty, SourceProperty, null));

        if (!_steppingEnabled)
        {
            base.OnTargetChanged();
            return;
        }

        _sourceStepTimer.Start();
    }

    /// <inheritdoc/>
    protected override void WriteSourceValue(object value)
    {
        object nextValue = GetNextWritableValue(value, SourceProperty.Name, _sourceStepTimer.Interval);
            
        base.WriteSourceValue(nextValue);
    }

    /// <inheritdoc/>
    protected override void WriteTargetValue(object value)
    {
        object nextValue = GetNextWritableValue(value, TargetProperty.Name, _targetStepTimer.Interval);
            
        base.WriteTargetValue(nextValue);
    }

    private static double ConvertPropertyValue(object? value, string propertyName)
    {
        if (value == null)
            return default;

        if (value is not IConvertible convertible)
        {
            throw new InvalidOperationException(
                Strings.NotSteppablePropertyValue.InvariantFormat(propertyName));
        }

        return convertible.ToDouble(CultureInfo.CurrentCulture);
    }

    private bool StepChanges(DependencyProperty changedProperty,
                             DependencyProperty receivingProperty,
                             object? unsetReceivingValue)
    {
        object receivingValue = GetValue(receivingProperty);

        if (receivingValue == unsetReceivingValue)
            return false;

        double changedStepValue = GetStepValue(changedProperty);
        double receivingStepValue = GetStepValue(receivingProperty);

        if (changedStepValue.ApproximatelyEquals(receivingStepValue))
            return false;

        int numberOfSteps = (int) (Math.Abs(changedStepValue - receivingStepValue) / _stepAmount);

        if (numberOfSteps < _minimumSteps)
            return false;

        _startingStepValue = receivingStepValue;
        _endingStepValue = changedStepValue;
        _targetStepTimer.Interval = Math.Max(_steppingDuration.Divide(Math.Max(numberOfSteps,1)).TotalMilliseconds, 1);

        _sequenceStopwatch.Start();

        return true;
    }

    private double GetStepValue(DependencyProperty property)
    {
        object? value = GetValue(property);

        return ConvertPropertyValue(value, property.Name);
    }

    private object GetNextWritableValue(object baseValue, string propertyName, double stepInterval)
    {
        double nextStepValue;

        if (!_steppingEnabled)
            nextStepValue = ConvertPropertyValue(baseValue, propertyName);
        else
        {
            _sequenceStopwatch.Stop();

            bool steppingUpwards = _endingStepValue > _startingStepValue;

            // Calculate where in the step sequence we should be given the time it's taken to get here.
            double expectedStepDelta =
                (_endingStepValue - _startingStepValue) * _sequenceStopwatch.Elapsed.Divide(_steppingDuration);

            // The expected delta evaluates to infinity if the configured sequence duration is zero.
            double nextValue = double.IsInfinity(expectedStepDelta)
                ? _endingStepValue
                : _startingStepValue + expectedStepDelta;

            _sequenceStopwatch.Start();

            // Ensure the time-corrected step value does not exceed the value marking the end of the sequence.
            nextStepValue = steppingUpwards
                ? Math.Min(_endingStepValue, nextValue)
                : Math.Max(_endingStepValue, nextValue);

            if (_isInteger)
            {   // If the step value needs to be an integer, we perform the rounding here first, as simply casting our step value
                // to an integer will result in the truncation of all decimal digits, this may lead to inaccurate effective stepping durations.
                double roundedStepValue = Math.Round(nextStepValue, MidpointRounding.AwayFromZero);
                TimeSpan nextStepTime = _sequenceStopwatch.Elapsed + TimeSpan.FromMilliseconds(stepInterval);

                // If our rounded value is such that the next step value actually ends up being the target, final value, we will actually
                // allow the typecast truncation to occur, so long as that another step interval doesn't exceed the desired duration of the
                // entire stepping sequence. This is to prevent the stepping sequence from finishing too early.
                if (!roundedStepValue.ApproximatelyEquals(_endingStepValue) || nextStepTime > _steppingDuration)
                    nextStepValue = roundedStepValue;
            }
        }

        // Although it looks like we could replace the following lines with a single ternary conditional operator,
        // don't forget attempting to do so will cause the boxed value type to always be double, as ternary conditional
        // operators must return the same type of object from both of its branches.
        if (_isInteger)
            return (int) nextStepValue;
            
        return nextStepValue;
    }

    private void StepSource()
    {
        if (GetStepValue(SourceProperty).ApproximatelyEquals(_endingStepValue))
        {
            _sequenceStopwatch.Reset();
            _sourceStepTimer.Stop();
        }
        else
            base.OnTargetChanged();
    }

    private void StepTarget()
    {
        if (GetStepValue(TargetProperty).ApproximatelyEquals(_endingStepValue))
        {
            _sequenceStopwatch.Reset();
            _targetStepTimer.Stop();
        }
        else
            base.OnSourceChanged();
    }

    private void HandleSourceStepTimerTick(object? sender, EventArgs e) 
        => _dispatcher.BeginInvoke(() => Binding?.DoBindingAction(StepSource), DispatcherPriority.DataBind);

    private void HandleTargetStepTimerTick(object? sender, EventArgs e) 
        => _dispatcher.BeginInvoke(() => Binding?.DoBindingAction(StepTarget), DispatcherPriority.DataBind);
}