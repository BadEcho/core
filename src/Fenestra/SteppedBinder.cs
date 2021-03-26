using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using BadEcho.Odin.Extensions;

namespace BadEcho.Fenestra
{
    /// <summary>
    /// Provides a freezable object that facilitates the realization of the impermanent state shared between source and target in
    /// a incremental or decremental fashion.
    /// </summary>
    /// <remarks>
    /// In other words, this is an object that will cause changes in a source numeric value to be propagated to the target property in
    /// a stepped fashion. The target property's value will be incremented or decremented until it reaches the new source property's value,
    /// with some slight delay introduced between steps in order to give it a visually pleasing effect.
    /// </remarks>
    /// TODO: Finish class.
    internal sealed class SteppedBinder : TransientBinder
    {
        private readonly DispatcherTimer _targetStepTimer;
        private readonly DispatcherTimer _sourceStepTimer;
        private readonly TimeSpan _steppingDuration;
        private readonly BindingMode _mode;
        private readonly object? _targetDefaultValue;

        private bool _isIncremental;
        private int _currentStepValue;

        private SteppedBinder(DependencyObject targetObject,
                              DependencyProperty targetProperty,
                              BindingBase binding,
                              TimeSpan steppingDuration,
                              BindingMode mode)
            : base(targetObject, targetProperty, binding, mode)
        {
            _mode = mode;
            _steppingDuration = steppingDuration;
            _targetDefaultValue = targetProperty.PropertyType.GetDefaultValue();

            _sourceStepTimer = new DispatcherTimer();
            _targetStepTimer = new DispatcherTimer();
            
            _sourceStepTimer.Tick += HandleSourceStepTimerTick;
            _targetStepTimer.Tick += HandleTargetStepTimerTick;
        }

        protected override void OnSourceChanged()
        {
            _sourceStepTimer.Stop();

            object targetValue = GetValue(TargetProperty);

            if (targetValue == _targetDefaultValue)
            {
                base.OnSourceChanged();
                return;
            }

            if (targetValue is not IConvertible targetConvertible)
            {
                
            }

            object sourceValue = GetValue(SourceProperty);

            if (sourceValue is not IConvertible sourceConvertible)
            {

            }

            int targetNumber = targetConvertible.ToInt32(CultureInfo.CurrentCulture);
            int sourceNumber = sourceConvertible.ToInt32(CultureInfo.CurrentCulture);
            int numberOfSteps = Math.Abs(sourceNumber - targetNumber);

            _isIncremental = sourceNumber > targetNumber;
            _currentStepValue = targetNumber;
            _targetStepTimer.Interval = _steppingDuration.Divide(numberOfSteps);

            _targetStepTimer.Start();
        }

        protected override void OnTargetChanged()
        {

        }

        private void HandleSourceStepTimerTick(object? sender, EventArgs e)
        {
            _sourceStepTimer.Stop();

            base.OnTargetChanged();
        }

        private void HandleTargetStepTimerTick(object? sender, EventArgs e)
        {
            _targetStepTimer.Stop();

            base.OnSourceChanged();
        }
    }
}
