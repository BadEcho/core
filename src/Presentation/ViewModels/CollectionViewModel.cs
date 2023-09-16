//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Threading;
using BadEcho.Collections;

namespace BadEcho.Presentation.ViewModels;

/// <summary>
/// Provides a base view model that automates communication between a view and a collection of bound data.
/// </summary>
/// <typeparam name="TModel">
/// The type of data bound to the view model as part of a collection for display on a view.
/// </typeparam>
/// <typeparam name="TChildViewModel">
/// The type of view model that this collection view model generates for its children.
/// </typeparam>
public abstract class CollectionViewModel<TModel, TChildViewModel> : ViewModel<TModel>, ICollectionViewModel<TModel,TChildViewModel>
    where TChildViewModel : class, IViewModel, IModelProvider<TModel>
{
    private readonly CollectionViewModelEngine<TModel, TChildViewModel> _engine;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionViewModel{TModel,TChildViewModel}"/> class.
    /// </summary>
    /// <param name="options">
    /// A <see cref="CollectionViewModelOptions"/> instance that configures the behavior of this engine.
    /// </param>
    protected CollectionViewModel(CollectionViewModelOptions options)
        : this(options, new UnsortedAddStrategy<TChildViewModel>())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionViewModel{TModel,TChildViewModel}"/> class.
    /// </summary>
    /// <param name="options">
    /// A <see cref="CollectionViewModelOptions"/> instance that configures the behavior of this view model's internal engine.
    /// </param>
    /// <param name="changeStrategy">
    /// The strategy behind how view models will be added to and removed from the view model's collection.
    /// </param>
    protected CollectionViewModel(CollectionViewModelOptions options, ICollectionChangeStrategy<TChildViewModel> changeStrategy)
    {
        Require.NotNull(options, nameof(options));

        options.ChildrenChangedHandler = HandleChildrenChanged;

        _engine = new CollectionViewModelEngine<TModel, TChildViewModel>(this, changeStrategy, options);
    }

    /// <inheritdoc/>
    public AtomicObservableCollection<TChildViewModel> Children
        => _engine.Children;

    /// <inheritdoc/>
    public void Bind(TChildViewModel viewModel) 
        => _engine.Bind(viewModel);

    /// <inheritdoc/>
    public bool Unbind(TChildViewModel viewModel) 
        => _engine.Unbind(viewModel);

    /// <inheritdoc/>
    public void ChangeDispatcher(Dispatcher dispatcher)
        => _engine.ChangeDispatcher(dispatcher);

    /// <inheritdoc/>
    public TChildViewModelImpl? FindChild<TChildViewModelImpl>(TModel model) 
        where TChildViewModelImpl : TChildViewModel
    {
        return _engine.FindChild<TChildViewModelImpl>(model);
    }

    /// <inheritdoc/>
    public abstract TChildViewModel CreateChild(TModel model);

    /// <inheritdoc/>
    public abstract void UpdateChild(TModel model);

    /// <inheritdoc/>
    protected override void OnBinding(TModel model) 
        => _engine.Bind(model);

    /// <inheritdoc/>
    protected override bool OnBatchBinding(IEnumerable<TModel> models)
    {   
        _engine.Bind(models);

        return true;
    }

    /// <inheritdoc/>
    protected override void OnUnbound(TModel model)
        => _engine.Unbind(model);

    /// <inheritdoc/>
    protected override bool OnBatchUnbound(IEnumerable<TModel> models)
    {
        _engine.Unbind(models);

        return true;
    }
        
    /// <summary>
    /// Called when there is a change to either the collection's composition or a property value of one of this view model's children.
    /// </summary>
    /// <param name="e">The <see cref="CollectionPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnChildrenChanged(CollectionPropertyChangedEventArgs e)
    { }

    private void HandleChildrenChanged(object? sender, CollectionPropertyChangedEventArgs e) 
        => OnChildrenChanged(e);
}