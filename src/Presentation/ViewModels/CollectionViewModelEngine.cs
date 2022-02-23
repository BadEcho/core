//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------  

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Windows.Threading;
using BadEcho.Presentation.Properties;
using BadEcho.Collections;
using BadEcho.Extensions;
using BadEcho.Logging;

namespace BadEcho.Presentation.ViewModels;

/// <summary>
/// Provides an engine used to power <see cref="ICollectionViewModel{TModel, TChildViewModel}"/> implementations through the
/// consolidation of functionalities of common interest.
/// </summary>
/// <typeparam name="TModel">
/// The type of item that can be bound to the <see cref="ICollectionViewModel{TModel, TChildViewModel}"/> view model powered
/// by this engine.
/// </typeparam>
/// <typeparam name="TChildViewModel">
/// The type of view model that the <see cref="ICollectionViewModel{TModel, TChildViewModel}"/> view model generates for its
/// children.
/// </typeparam>
internal sealed class CollectionViewModelEngine<TModel, TChildViewModel> : ViewModel<TChildViewModel>
    where TChildViewModel : class, IViewModel, IModelProvider<TModel>
{
    private readonly ConcurrentQueue<TChildViewModel> _bindingQueue = new();
    private readonly List<TModel> _processedModels = new();
    private readonly object _processedModelsLock = new();
    private readonly object _capacityEnforcementLock = new();

    private readonly ICollectionViewModel<TModel, TChildViewModel> _viewModel;
    private readonly ICollectionChangeStrategy<TChildViewModel> _changeStrategy;
    private readonly CollectionViewModelOptions _options;

    private DispatcherTimer _bindingTimer = new(DispatcherPriority.Background);
    private DispatcherTimer _capacityEnforcementTimer = new(DispatcherPriority.Background);

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionViewModelEngine{TModel,TChildViewModel}"/> class.
    /// </summary>
    /// <param name="viewModel">
    /// The <see cref="ICollectionViewModel{TModel, TChildViewModel}"/> typed view model being powered by this engine.
    /// </param>
    /// <param name="changeStrategy">
    /// The strategy behind how view models will be added to and removed from this engine's internal collection.
    /// </param>
    /// <param name="options">
    /// A <see cref="CollectionViewModelOptions"/> instance that configures the behavior of this engine.
    /// </param>
    public CollectionViewModelEngine(
        ICollectionViewModel<TModel, TChildViewModel> viewModel,
        ICollectionChangeStrategy<TChildViewModel> changeStrategy,
        CollectionViewModelOptions options)
    {
        Require.NotNull(viewModel, nameof(viewModel));
        Require.NotNull(changeStrategy, nameof(changeStrategy));
        Require.NotNull(options, nameof(options));

        if (options.ChildrenChangedHandler == null)
            throw new ArgumentException(Strings.CollectionViewModelEngineRequiresHandler, nameof(options));
            
        _viewModel = viewModel;
        _changeStrategy = changeStrategy;
        _options = options;
            
        _bindingTimer.Interval = options.BindingDelay;
        _bindingTimer.Tick += HandleBindingTimerTick;

        _capacityEnforcementTimer.Interval = options.CapacityEnforcementDelay;
        _capacityEnforcementTimer.Tick += HandleCapacityEnforcementTimerTick;

        Children = new AtomicObservableCollection<TChildViewModel>();

        var collectionChangePublisher = new CollectionPropertyChangePublisher<TChildViewModel>(Children);

        collectionChangePublisher.Changed += options.ChildrenChangedHandler + HandleChildrenChanged;
    }

    /// <summary>
    /// Gets a collection of child view models, each of which represent an individual item bound to an
    /// <see cref="ICollectionViewModel{TModel, TChildViewModel}"/> typed view model powered by this engine.
    /// </summary>
    public AtomicObservableCollection<TChildViewModel> Children
    { get; }

    /// <summary>
    /// Gets a value indicating if additions to the child view model collection should be delayed following new data bindings.
    /// </summary>
    private bool DelayBindings
        => _options.BindingDelay != default;

    /// <summary>
    /// Gets a value indicating if capacity enforcement actions on the child view model collection should be delayed following
    /// the addition of new items to the child view model collection.
    /// </summary>
    private bool DelayCapacityEnforcement
        => _options.CapacityEnforcementDelay != default;

    /// <summary>
    /// Gets the dispatcher in use by this engine and child view model collection it provides.
    /// </summary>
    private Dispatcher Dispatcher
        => _bindingTimer.Dispatcher;

    /// <summary>
    /// Changes the dispatcher currently in use by the engine to the provided one.
    /// </summary>
    /// <param name="dispatcher">The dispatcher that the engine should use to broadcast events on.</param>
    public void ChangeDispatcher(Dispatcher dispatcher)
    {
        Require.NotNull(dispatcher, nameof(dispatcher));

        if (Dispatcher == dispatcher)
            return;

        _bindingTimer.Stop();
        _bindingTimer.Tick -= HandleBindingTimerTick;

        _bindingTimer = new DispatcherTimer(DispatcherPriority.Background, dispatcher)
                        {
                            Interval = _options.BindingDelay
                        };

        _bindingTimer.Tick += HandleBindingTimerTick;

        _capacityEnforcementTimer.Stop();
        _capacityEnforcementTimer.Tick -= HandleCapacityEnforcementTimerTick;

        _capacityEnforcementTimer = new DispatcherTimer(DispatcherPriority.Background, dispatcher)
                                    {
                                        Interval = _options.CapacityEnforcementDelay
                                    };

        _capacityEnforcementTimer.Tick += HandleCapacityEnforcementTimerTick;

        Children.ChangeDispatcher(dispatcher);
    }

    /// <summary>
    /// Aids in the binding of the provided data to the <see cref="ICollectionViewModel{TModel, TChildViewModel}"/> view
    /// model powered by this engine, and adds the resulting child <typeparamref name="TChildViewModel"/> object
    /// to the child view model collection.
    /// </summary>
    /// <param name="model">The data to aid in binding to the view model being powered by this engine.</param>
    public void Bind(TModel model)
    {
        Require.NotNull(model, nameof(model));
        bool update = false;

        lock (_processedModelsLock)
        {
            // Delayed bound batch bindings as well as changes made directly to the view model's collection may arrive
            // here already processed, yet not bound to the view model. We'll want to skip out on any additional processing
            // and let the view model consuming this engine go ahead and complete its binding process.
            if (!_viewModel.IsBound(model) && _processedModels.Contains(model))
                return;

            if (_processedModels.Contains(model))
                update = true;
            else
                _processedModels.Add(model);
        }

        if (update)
        {
            _viewModel.UpdateChild(model);
            return;
        }

        TChildViewModel childViewModel = _viewModel.CreateChild(model);

        Bind(childViewModel);
    }

    /// <summary>
    /// Aids in the binding of the provided sequence of data to the <see cref="ICollectionViewModel{TModel, TChildViewModel}"/>
    /// view model powered by this engine, and adds the resulting children <typeparamref name="TChildViewModel"/>
    /// objects to the child view model collection.
    /// </summary>
    /// <param name="models">The sequence of data to aid in binding to the view model being powered by this engine.</param>
    public void Bind(IEnumerable<TModel> models)
    {
        Require.NotNull(models, nameof(models));

        if (_options.AsyncBatchBindings)
        {
            Task.Run(() => BindRunner(models.ToList()))
                .ContinueWith(ProcessFailedBinding,
                              CancellationToken.None,
                              TaskContinuationOptions.OnlyOnFaulted,
                              TaskScheduler.Default);
        }
        else
            BindRunner(models.ToList());
    }

    /// <summary>
    /// Aids in the unbinding of the provided data from the <see cref="ICollectionViewModel{TModel, TChildViewModel}"/> view
    /// model powered by this engine, and removes the associated child <typeparamref name="TChildViewModel"/> object
    /// from the child view model collection.
    /// </summary>
    /// <param name="model">The data to aid in unbinding from the view model being powered by this engine.</param>
    public void Unbind(TModel model)
    {
        Require.NotNull(model, nameof(model));

        lock (_processedModelsLock)
        {
            if (!_processedModels.Remove(model))
                return;
        }

        TChildViewModel? childToUnbind = _viewModel.FindChild<TChildViewModel>(model);

        if (null == childToUnbind)
        {
            PurgeZombies(model.AsEnumerable());
            return;
        }

        Unbind(childToUnbind);
    }

    /// <summary>
    /// Aids in the unbinding of the provided sequence of data from the <see cref="ICollectionViewModel{TModel, TChildViewModel}"/>
    /// view model powered by this engine, and removes the associated child <typeparamref name="TChildViewModel"/> objects
    /// from the child view model collection.
    /// </summary>
    /// <param name="models">The sequence of data to aid in unbinding from the view model powered by this engine.</param>
    public void Unbind(IEnumerable<TModel> models)
    {
        models = models.ToList();

        List<TModel> processedModels;

        lock (_processedModelsLock)
        {
            processedModels = models.Where(_processedModels.Remove).ToList();
        }

        List<TChildViewModel> childrenToUnbind = processedModels.Select(m => _viewModel.FindChild<TChildViewModel>(m))
                                                                .WhereNotNull()
                                                                .ToList();

        if (processedModels.Count != 0 && childrenToUnbind.Count == 0)
        {
            PurgeZombies(models);
            return;
        }

        if (childrenToUnbind.Count == 0)
            return;

        _changeStrategy.RemoveRange(_viewModel, childrenToUnbind);

        foreach (TChildViewModel childToUnbind in childrenToUnbind)
        {
            Unbind(childToUnbind);
        }
    }

    /// <summary>
    /// Aids in the unbinding of all data currently bound to the <see cref="ICollectionViewModel{TModel, TChildViewModel}"/> view model,
    /// and removes all child <typeparamref name="TChildViewModel"/> objects from the child view model collection.
    /// </summary>
    public void Clear()
    {
        lock (_processedModelsLock)
        {
            _processedModels.Clear();
        }

        List<TChildViewModel> childrenToUnbind = Children.ToList();

        Children.Clear();

        foreach (TChildViewModel childToUnbind in childrenToUnbind)
        {
            Unbind(childToUnbind);
        }
    }

    /// <summary>
    /// Searches for and returns the child view model responsible for representing the provided data within the
    /// <see cref="ICollectionViewModel{TModel, TChildViewModel}"/> view model being powered by this engine.
    /// </summary>
    /// <typeparam name="TChildViewModelImpl">The specific type of child view model to look for.</typeparam>
    /// <param name="model">The bound data of the child view model to search for.</param>
    /// <returns>
    /// The <typeparamref name="TChildViewModelImpl"/> instance that <c>model</c> is bound to, or null if nothing was found.
    /// </returns>
    public TChildViewModelImpl? FindChild<TChildViewModelImpl>(TModel model)
        where TChildViewModelImpl : TChildViewModel
    {
        try
        {
            return Children.OfType<TChildViewModelImpl>()
                           .SingleOrDefault(c => c.ActiveModel != null && c.ActiveModel.Equals<TModel>(model));
        }
        catch (InvalidOperationException ex)
        {
            Logger.Error(Strings.DuplicateModelInCollectionViewModel, ex);

            return Children.OfType<TChildViewModelImpl>()
                           .First(c => c.ActiveModel != null && c.ActiveModel.Equals<TModel>(model));
        }
    }

    /// <inheritdoc/>
    protected override void OnBinding(TChildViewModel viewModel)
    {
        // Child view models being bound in batches are added to the child view model collection silently prior to
        // their binding, unless binding is delayed, to avoid excessive collection change notifications being broadcast.
        if (Children.Contains(viewModel))
            return;

        if (!DelayBindings)
        {
            _changeStrategy.Add(_viewModel, viewModel);
            RequestEnforceCapacity();

            return;
        }

        _bindingQueue.Enqueue(viewModel);
        _bindingTimer.Start();
    }

    /// <inheritdoc/>
    protected override void OnUnbound(TChildViewModel viewModel)
    {
        _changeStrategy.Remove(_viewModel, viewModel);
    }

    private void BindRunner(IReadOnlyCollection<TModel> models)
    {
        List<TModel> newChildrenModels;
        List<TModel> existingChildrenModels;

        lock (_processedModelsLock)
        {
            newChildrenModels = models.Except(_processedModels).ToList();
            existingChildrenModels = models.Intersect(_processedModels).ToList();

            _processedModels.AddRange(newChildrenModels);
        }

        if (newChildrenModels.Count == 0 && existingChildrenModels.Count == 0)
            return;

        IReadOnlyCollection<TChildViewModel> createdChildren = BindNewChildren(newChildrenModels);
        BindExistingChildren(existingChildrenModels);

        if (!DelayBindings)
        {
            _changeStrategy.AddRange(_viewModel, createdChildren);
            RequestEnforceCapacity();
        }

        foreach (TChildViewModel createdChild in createdChildren)
        {
            Bind(createdChild);
        }
    }

    private IReadOnlyCollection<TChildViewModel> BindNewChildren(IList<TModel> models)
    {
        TChildViewModel[] createdChildren = new TChildViewModel[models.Count];
        if (models.Count >= _options.BindingParallelizationThreshold)
        {
            Parallel.For(0,
                         createdChildren.Length,
                         i => createdChildren[i] = _viewModel.CreateChild(models[i]));
        }
        else
        {
            for (int i = 0; i < models.Count; i++)
                createdChildren[i] = _viewModel.CreateChild(models[i]);
        }

        return createdChildren;
    }

    private void BindExistingChildren(IList<TModel> models)
    {
        if (models.Count >= _options.BindingParallelizationThreshold)
        {
            Parallel.For(0,
                         models.Count,
                         i => _viewModel.UpdateChild(models[i]));
        }
        else
        {
            foreach (var modelToUpdate in models)
                _viewModel.UpdateChild(modelToUpdate);
        }
    }

    private void PurgeZombies(IEnumerable<TModel> zombieModels)
    {
        List<TChildViewModel> zombieChildren = Children.Where(c => c.ActiveModel == null)
                                                       .ToList();
        if (0 != zombieChildren.Count) 
            _changeStrategy.RemoveRange(_viewModel, zombieChildren);

        foreach (TModel zombieModel in zombieModels)
        {
            if (!_viewModel.IsBound(zombieModel))
                continue;

            _viewModel.Unbind(zombieModel);
        }
    }

    private void RequestEnforceCapacity()
    {
        if (_options.Capacity <= 0)
            return;

        if (!DelayCapacityEnforcement || _options.CapacityEnforcementDelayLimit > 0 && Children.Count >= _options.CapacityEnforcementDelayLimit)
            EnforceCapacity();
        else
            _capacityEnforcementTimer.Start();
    }

    private void EnforceCapacity()
    {
        lock (_capacityEnforcementLock)
        {
            int exceededCount = Children.Count - _options.Capacity;

            _changeStrategy.TrimExcess(_viewModel, exceededCount);
        }
    }

    private void ProcessFailedBinding(Task failedBinding)
    {
        Exception? rootException = null;

        if (failedBinding.Exception != null)
            rootException = failedBinding.Exception.FindInnermostException();

        Dispatcher.BeginInvoke(() => throw new EngineException(Strings.FenestraDispatcherError, rootException),
                               DispatcherPriority.Send);
    }

    private void HandleChildrenChanged(object? sender, CollectionPropertyChangedEventArgs e)
    {
        if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            return;

        foreach (TChildViewModel childViewModel in e.OldItems)
        {
            if (childViewModel.ActiveModel == null)
                throw new InvalidOperationException(Strings.CollectionChildUnbindingRequiresActiveModel);

            _viewModel.Unbind(childViewModel.ActiveModel);
        }

        foreach (TChildViewModel childViewModel in e.NewItems)
        {
            if (childViewModel.ActiveModel == null)
                throw new InvalidOperationException(Strings.CollectionChildBindingRequiresActiveModel);

            lock (_processedModelsLock)
            {
                if (!_processedModels.Contains(childViewModel.ActiveModel))
                    _processedModels.Add(childViewModel.ActiveModel);
            }

            if (_options.BindChildren)
                _viewModel.Bind(childViewModel.ActiveModel);
        }
    }

    private void HandleBindingTimerTick(object? sender, EventArgs e)
    {
        if (!_bindingQueue.TryDequeue(out TChildViewModel? child))
        {
            _bindingTimer.Stop();
            return;
        }

        _changeStrategy.Add(_viewModel, child);
        RequestEnforceCapacity();
    }

    private void HandleCapacityEnforcementTimerTick(object? sender, EventArgs e)
    {
        _capacityEnforcementTimer.Stop();

        EnforceCapacity();
    }
}