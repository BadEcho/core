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

using System.Diagnostics.CodeAnalysis;
using BadEcho.Presentation.Properties;
using BadEcho.Extensions;

namespace BadEcho.Presentation.ViewModels;

/// <summary>
/// Provides a base view model that automates communication between a view and a collection of bound polymorphic data.
/// </summary>
/// <typeparam name="TModel">
/// The type of data bound to the view model as part of a collection for display on a view.
/// </typeparam>
/// <typeparam name="TChildViewModel">
/// The type of view model that this collection view model generates for its children.
/// </typeparam>
public abstract class PolymorphicCollectionViewModel<TModel, TChildViewModel> : CollectionViewModel<TModel, TChildViewModel>
    where TModel : notnull
    where TChildViewModel : class, IViewModel, IModelProvider<TModel>
{
    private readonly Dictionary<Type, Func<TModel, TChildViewModel>> _typeInitializerMap = new();
    private readonly Dictionary<Type, Action<TModel>> _typeUpdaterMap = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PolymorphicCollectionViewModel{TModel, TChildViewModel}"/> class.
    /// </summary>
    /// <param name="options">
    /// A <see cref="CollectionViewModelOptions"/> instance that configures the behavior of this engine.
    /// </param>
    protected PolymorphicCollectionViewModel(CollectionViewModelOptions options)
        : base(options)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolymorphicCollectionViewModel{TModel, TChildViewModel}"/> class.
    /// </summary>
    /// <param name="options">
    /// A <see cref="CollectionViewModelOptions"/> instance that configures the behavior of this view model's internal engine.
    /// </param>
    /// <param name="changeStrategy">
    /// The strategy behind how view models will be added to and removed from this view model's collection.
    /// </param>
    protected PolymorphicCollectionViewModel(CollectionViewModelOptions options,
                                             ICollectionChangeStrategy<TChildViewModel> changeStrategy)
        : base(options, changeStrategy)
    { }

    /// <inheritdoc/>
    public override TChildViewModel CreateItem(TModel model)
    {
        Require.NotNull(model, nameof(model));

        Type modelType = model.GetType();

        return TryInitialize(model, out TChildViewModel? viewModel)
            ? viewModel
            : throw new ArgumentException(Strings.ModelImplentationNotRegistered.InvariantFormat(modelType.Name),
                                          nameof(model));
    }

    /// <inheritdoc/>
    public override void UpdateItem(TModel model)
    {
        Require.NotNull(model, nameof(model));

        Type modelType = model.GetType();

        if (!_typeUpdaterMap.TryGetValue(modelType, out Action<TModel>? typeUpdater))
        {
            throw new ArgumentException(Strings.ModelImplentationNotRegistered.InvariantFormat(modelType.Name),
                                        nameof(model));
        }

        typeUpdater(model);
    }

    /// <summary>
    /// Registers a derived model and child view model pair for automatic model to view model operative resolution.
    /// </summary>
    /// <typeparam name="TModelImpl">
    /// A derivation of the type of data bound to view models contained in the collection-typed view model.
    /// </typeparam>
    /// <typeparam name="TChildViewModelImpl">
    /// A derivation of the type of view model contained in the collection-typed view model.
    /// </typeparam>
    protected void RegisterDerivation<TModelImpl, TChildViewModelImpl>()
        where TModelImpl : class, TModel
        where TChildViewModelImpl : ViewModel<TModelImpl>, TChildViewModel, new()
    {
        Type modelType = typeof(TModelImpl);

        if (_typeInitializerMap.ContainsKey(modelType))
            throw new ArgumentException(Strings.ModelImplementationAlreadyRegistered.InvariantFormat(modelType.Name));

        _typeInitializerMap.Add(modelType,
                                baseModel =>
                                {
                                    var model = (TModelImpl)baseModel;
                                    var childViewModel = new TChildViewModelImpl();

                                    childViewModel.Bind(model);

                                    return childViewModel;
                                });

        _typeUpdaterMap.Add(modelType,
                            baseModel =>
                            {
                                var model = (TModelImpl)baseModel;
                                
                                var childViewModel = FindItem<TChildViewModelImpl>(model)
                                    ?? throw new ArgumentException(Strings.CannotUpdateUnboundData, nameof(baseModel));

                                childViewModel.Bind(model);
                            });
    }

    private bool TryInitialize(TModel model, [NotNullWhen(true)] out TChildViewModel? viewModel)
    {
        Type? modelType = model.GetType();
        viewModel = null;

        while (modelType != null)
        {
            if (_typeInitializerMap.TryGetValue(modelType, out Func<TModel, TChildViewModel>? typeInitializer))
            {
                viewModel = typeInitializer(model);
                break;
            }

            modelType = modelType.BaseType;
        }

        return viewModel != null;
    }
}