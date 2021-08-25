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

using System;
using System.Diagnostics;
using System.Globalization;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using BadEcho.Fenestra.Properties;
using BadEcho.Odin;
using BadEcho.Odin.Extensions;

namespace BadEcho.Fenestra
{
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
    /// </remarks>
    internal sealed class SteppedBinder : TransientBinder
    {
        private readonly Timer _targetStepTimer
            = new();

        private readonly Timer _sourceStepTimer
            = new();

        private readonly Stopwatch _sequenceStopwatch 
            = new();

        private readonly Dispatcher _dispatcher
            = Dispatcher.CurrentDispatcher;

        private readonly TimeSpan _steppingDuration;
        private readonly int _minimumSteps;
        private readonly object? _unsetTargetValue;

        private bool _steppingEnabled;
        private int _endingStepValue;
        private int _startingStepValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SteppedBinder"/> class.
        /// </summary>
        /// <param name="targetObject">The target dependency object containing the property to bind.</param>
        /// <param name="targetProperty">The target dependency property to bind.</param>
        /// <param name="options">Stepping sequence options related to timing for the stepped binder.</param>
        public SteppedBinder(DependencyObject targetObject,
                             DependencyProperty targetProperty,
                             SteppingOptions options)
            : base(targetObject, targetProperty, options.Binding)
        {
            Require.NotNull(options, nameof(options));
            
            if (options.SteppingDuration < TimeSpan.Zero)
                throw new ArgumentException(Strings.SteppingDurationCannotBeNegative, nameof(options));
            
            _steppingDuration = options.SteppingDuration;
            _minimumSteps = options.MinimumSteps;
            _sourceStepTimer.Elapsed += HandleSourceStepTimerTick;
            _targetStepTimer.Elapsed += HandleTargetStepTimerTick;
            _unsetTargetValue = targetProperty.DefaultMetadata.DefaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SteppedBinder"/> class.
        /// </summary>
        private SteppedBinder()
        { }

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
            object newValue = GetNextStepValue(value);

            base.WriteSourceValue(newValue);
        }

        /// <inheritdoc/>
        protected override void WriteTargetValue(object value)
        {
            object newValue = GetNextStepValue(value);

            base.WriteTargetValue(newValue);
        }

        private bool StepChanges(DependencyProperty changedProperty,
                                 DependencyProperty receivingProperty,
                                 object? unsetReceivingValue)
        {
            object receivingValue = GetValue(receivingProperty);

            if (receivingValue == unsetReceivingValue)
                return false;

            int changedStepValue = GetStepValue(changedProperty);
            int receivingStepValue = GetStepValue(receivingProperty);

            if (changedStepValue == receivingStepValue)
                return false;

            int numberOfSteps = Math.Abs(changedStepValue - receivingStepValue);

            if (numberOfSteps < _minimumSteps)
                return false;

            _startingStepValue = receivingStepValue;
            _endingStepValue = changedStepValue;
            _targetStepTimer.Interval = Math.Max(_steppingDuration.Divide(numberOfSteps).TotalMilliseconds, 1);

            _sequenceStopwatch.Start();

            return true;
        }

        private int GetStepValue(DependencyProperty property)
        {
            object value = GetValue(property);

            if (value is not IConvertible convertible)
            {
                throw new InvalidOperationException(
                    Strings.NotSteppablePropertyValue.InvariantFormat(property.Name));
            }

            return convertible.ToInt32(CultureInfo.CurrentCulture, NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign);
        }

        private object GetNextStepValue(object baseValue)
        {
            if (!_steppingEnabled)
                return baseValue;

            _sequenceStopwatch.Stop();

            // Calculate where in the step sequence we should be given the time it's taken to get here.
            double expectedStepDelta =
                (_endingStepValue - _startingStepValue) * _sequenceStopwatch.Elapsed.Divide(_steppingDuration);
            
            // The expected delta evaluates to infinity if the configured sequence duration is zero.
            int nextValue = double.IsInfinity(expectedStepDelta)
                ? _endingStepValue
                : _startingStepValue + (int) expectedStepDelta;

            _sequenceStopwatch.Start();

            // Ensure the time-corrected step value does not exceed the value marking the end of the sequence.
            return _endingStepValue > _startingStepValue
                ? Math.Min(_endingStepValue, nextValue)
                : Math.Max(_endingStepValue, nextValue);
        }

        private void StepSource()
        {
            if (GetStepValue(SourceProperty) == _endingStepValue)
            {
                _sequenceStopwatch.Reset();
                _sourceStepTimer.Stop();
            }
            else
                base.OnTargetChanged();
        }

        private void StepTarget()
        {
            if (GetStepValue(TargetProperty) == _endingStepValue)
            {
                _sequenceStopwatch.Reset();
                _targetStepTimer.Stop();
            }
            else
                base.OnSourceChanged();
        }

        private void HandleSourceStepTimerTick(object? sender, EventArgs e) 
            => _dispatcher.BeginInvoke(StepSource, DispatcherPriority.DataBind);

        private void HandleTargetStepTimerTick(object? sender, EventArgs e) 
            => _dispatcher.BeginInvoke(StepTarget, DispatcherPriority.DataBind);
    }
}