//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using BadEcho.Fenestra.Properties;
using BadEcho.Odin;

namespace BadEcho.Fenestra
{
    /// <summary>
    /// Provides a freezable object that facilitates the realization of the impermanent state shared between source and target in
    /// a incremental or decremental fashion.
    /// </summary>
    /// <remarks>
    /// In other words, this is an object that will cause changes in a source numeric value to be propagated to a target property in
    /// a stepped fashion. The target property's value will be incremented or decremented until it reaches the new source property's value,
    /// with some slight delay introduced between steps in order to give it a visually pleasing effect.
    /// </remarks>
    internal sealed class SteppedBinder : TransientBinder
    {
        private readonly DispatcherTimer _targetStepTimer
            = new();
        private readonly DispatcherTimer _sourceStepTimer
            = new();

        private readonly TimeSpan _steppingDuration;

        private bool _isIncremental;
        private int _currentStepValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SteppedBinder"/> class.
        /// </summary>
        /// <param name="targetObject">The target dependency object containing the property to bind.</param>
        /// <param name="targetProperty">The target dependency property to bind.</param>
        /// <param name="binding">The underlying binding to augment.</param>
        /// <param name="steppingDuration">The total duration of a binding update stepping sequence.</param>
        public SteppedBinder(DependencyObject targetObject,
                             DependencyProperty targetProperty,
                             Binding binding,
                             TimeSpan steppingDuration)
            : this(targetObject, targetProperty, binding, steppingDuration, EnsureBinding(binding).Mode)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SteppedBinder"/> class.
        /// </summary>
        /// <param name="targetObject">The target dependency object containing the property to bind.</param>
        /// <param name="targetProperty">The target dependency property to bind.</param>
        /// <param name="binding">The underlying binding to augment.</param>
        /// <param name="steppingDuration">The total duration of a binding update stepping sequence.</param>
        public SteppedBinder(DependencyObject targetObject,
                             DependencyProperty targetProperty,
                             MultiBinding binding,
                             TimeSpan steppingDuration)
            : this(targetObject, targetProperty, binding, steppingDuration, EnsureBinding(binding).Mode)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SteppedBinder"/> class.
        /// </summary>
        /// <param name="targetObject">The target dependency object containing the property to bind.</param>
        /// <param name="targetProperty">The target dependency property to bind.</param>
        /// <param name="binding">The underlying binding to augment.</param>
        /// <param name="steppingDuration">The total duration of a binding update stepping sequence.</param>
        /// <param name="mode">The mode of binding being used.</param>
        private SteppedBinder(DependencyObject targetObject,
                              DependencyProperty targetProperty,
                              BindingBase binding,
                              TimeSpan steppingDuration,
                              BindingMode mode)
            : base(targetObject, targetProperty, binding, mode)
        {
            _steppingDuration = steppingDuration;

            _sourceStepTimer.Tick += HandleSourceStepTimerTick;
            _targetStepTimer.Tick += HandleTargetStepTimerTick;
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

            object targetValue = GetValue(TargetProperty);

            if (targetValue == null)
            {
                base.OnSourceChanged();
                return;
            }

            int sourceStepValue = GetSourceStepValue();
            int targetStepValue = GetTargetStepValue();

            LoadSteppingSequence(targetStepValue, sourceStepValue);

            _targetStepTimer.Start();
        }

        /// <inheritdoc/>
        protected override void OnTargetChanged()
        {
            _targetStepTimer.Stop();

            object sourceValue = GetValue(TargetProperty);

            if (sourceValue == null)
            {
                base.OnTargetChanged();
                return;
            }

            int sourceStepValue = GetSourceStepValue();
            int targetStepValue = GetTargetStepValue();

            LoadSteppingSequence(sourceStepValue, targetStepValue);

            _sourceStepTimer.Start();
        }

        /// <inheritdoc/>
        protected override void WriteSourceValue(object value)
        {
            int newValue = GetNextStepValue();

            base.WriteSourceValue(newValue);
        }

        /// <inheritdoc/>
        protected override void WriteTargetValue(object value)
        {
            int newValue = GetNextStepValue();

            base.WriteTargetValue(newValue);
        }

        private static TBinding EnsureBinding<TBinding>([NotNull]TBinding binding)
            where TBinding : BindingBase
        {
            Require.NotNull(binding, nameof(binding));

            return binding;
        }

        private int GetNextStepValue() 
            => _isIncremental ? _currentStepValue++ : _currentStepValue--;

        private int GetSourceStepValue()
        {
            object sourceValue = GetValue(SourceProperty);

            if (sourceValue is not IConvertible sourceConvertible)
                throw new InvalidOperationException(Strings.NotSteppableSourceValue);

            return sourceConvertible.ToInt32(CultureInfo.CurrentCulture);
        }

        private int GetTargetStepValue()
        {
            object targetValue = GetValue(TargetProperty);

            if (targetValue is not IConvertible targetConvertible)
                throw new InvalidOperationException(Strings.NotSteppableTargetValue);

            return targetConvertible.ToInt32(CultureInfo.CurrentCulture);
        }

        private void LoadSteppingSequence(int startingStepValue, int endingStepValue)
        {
            int numberOfSteps = Math.Abs(endingStepValue - startingStepValue);

            _isIncremental = endingStepValue > startingStepValue;
            _currentStepValue = startingStepValue;
            _targetStepTimer.Interval = _steppingDuration.Divide(numberOfSteps);
        }

        private void HandleSourceStepTimerTick(object? sender, EventArgs e)
        {
            if (GetSourceStepValue() == _currentStepValue)
                _sourceStepTimer.Stop();
            else
                base.OnTargetChanged();
        }

        private void HandleTargetStepTimerTick(object? sender, EventArgs e)
        {
            if (GetTargetStepValue() == _currentStepValue)
                _targetStepTimer.Stop();
            else
                base.OnSourceChanged();
        }
    }
}
