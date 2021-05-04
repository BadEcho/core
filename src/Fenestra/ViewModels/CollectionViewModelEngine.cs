//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------  

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using BadEcho.Fenestra.Properties;
using BadEcho.Odin;
using BadEcho.Odin.Collections;
using BadEcho.Odin.Extensions;

namespace BadEcho.Fenestra.ViewModels
{
    internal sealed class CollectionViewModelEngine<TModel, TChildViewModel> : ViewModel<TChildViewModel>
        where TChildViewModel : class, IViewModel<TModel>
    {
        private readonly ConcurrentQueue<TChildViewModel> _bindingQueue = new();
        private readonly List<TModel> _processedModels = new();
        private readonly object _processedModelsLock = new();

        private readonly ICollectionViewModel<TModel, TChildViewModel> _viewModel;
        private readonly CollectionViewModelOptions _options;

        private DispatcherTimer _bindingTimer = new(DispatcherPriority.Background);

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewModelEngine{TModel,TChildViewModel}"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The <see cref="ICollectionViewModel{TModel, TChildViewModel}"/> typed view model being powered by this engine.
        /// </param>
        /// <param name="options">
        /// A <see cref="CollectionViewModelOptions"/> instance that configures the behavior of this engine.
        /// </param>
        public CollectionViewModelEngine(
            ICollectionViewModel<TModel, TChildViewModel> viewModel,
            CollectionViewModelOptions options)
        {
            Require.NotNull(viewModel, nameof(viewModel));
            Require.NotNull(options, nameof(options));

            if (options.ChildrenChangedHandler == null)
                throw new ArgumentException(Strings.CollectionViewModelEngineRequiresHandler, nameof(options));
            
            _viewModel = viewModel;
            _options = options;
            
            _bindingTimer.Interval = options.BindingDelay;
            _bindingTimer.Tick += HandleBindingTimerTick;

            Children = new AtomicObservableCollection<TChildViewModel>();

            var collectionSubscriber = new CollectionPropertyChangeSubscriber<TChildViewModel>(Children);

            collectionSubscriber.Changed += options.ChildrenChangedHandler;
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

            lock (_processedModelsLock)
            {
                if (_processedModels.Contains(model))
                    return;

                _processedModels.Add(model);
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
                Task.Run(() => BindRunner(models))
                    .ContinueWith(ProcessFailedBinding,
                                  CancellationToken.None,
                                  TaskContinuationOptions.OnlyOnFaulted,
                                  TaskScheduler.Default);
            }
            else
                BindRunner(models);
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

            TChildViewModel? childToUnbind = _viewModel.FindChild(model);

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

            List<TChildViewModel> childrenToUnbind = processedModels.Select(m => _viewModel.FindChild(m))
                                                                    .WhereNotNull()
                                                                    .ToList();

            if (processedModels.Count != 0 && childrenToUnbind.Count == 0)
            {
                PurgeZombies(models);
                return;
            }

            if (childrenToUnbind.Count == 0)
                return;

            Children.RemoveRange(childrenToUnbind, false);
            _viewModel.OnChangeCompleted();

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

        /// <inheritdoc/>
        protected override void OnBinding(TChildViewModel viewModel)
        {
            // Child view models being bound in batches are added to the child view model collection silently prior to
            // their binding, unless binding is delayed, to avoid excessive collection change notifications being broadcast.
            if (Children.Contains(viewModel))
                return;

            if (!DelayBindings)
            {
                Children.Add(viewModel);
                _viewModel.OnChangeCompleted();
            }

            _bindingQueue.Enqueue(viewModel);
            _bindingTimer.Start();
        }

        /// <inheritdoc/>
        protected override void OnUnbound(TChildViewModel viewModel)
        {
            if (!Children.Remove(viewModel))
                return;

            _viewModel.OnChangeCompleted();
        }

        private void BindRunner(IEnumerable<TModel> models)
        {
            List<TModel> modelsToBind;

            lock (_processedModelsLock)
            {
                modelsToBind = models.Except(_processedModels).ToList();

                _processedModels.AddRange(modelsToBind);
            }

            if (modelsToBind.Count == 0)
                return;

            TChildViewModel[] createdChildren = new TChildViewModel[modelsToBind.Count];
            if (modelsToBind.Count >= _options.BindingParallelizationThreshold)
            {
                Parallel.For(0,
                             createdChildren.Length,
                             i => createdChildren[i] = _viewModel.CreateChild(modelsToBind[i]));

            }
            else
            {
                for (int i = 0; i < modelsToBind.Count; i++)
                    createdChildren[i] = _viewModel.CreateChild(modelsToBind[i]);
            }

            if (!DelayBindings)
                Children.AddRange(createdChildren, false);

            foreach (TChildViewModel createdChild in createdChildren)
            {
                Bind(createdChild);
            }
        }

        private void PurgeZombies(IEnumerable<TModel> zombieModels)
        {
            List<TChildViewModel> zombieChildren = Children.Where(c => c.ActiveModel == null)
                                                           .ToList();
            if (0 != zombieChildren.Count)
            {
                Children.RemoveRange(zombieChildren, false);
                _viewModel.OnChangeCompleted();
            }

            foreach (TModel zombieModel in zombieModels)
            {
                if (!_viewModel.IsBound(zombieModel))
                    continue;

                _viewModel.Unbind(zombieModel);
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
        
        private void HandleBindingTimerTick(object? sender, EventArgs e)
        {
            if (!_bindingQueue.TryDequeue(out TChildViewModel? child))
            {
                _bindingTimer.Stop();
                _viewModel.OnChangeCompleted();
                return;
            }

            Children.Add(child);
        }
    }
}
