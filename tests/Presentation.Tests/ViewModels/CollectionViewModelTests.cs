//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Presentation.ViewModels;
using Xunit;

namespace BadEcho.Presentation.Tests.ViewModels;

public class CollectionViewModelTests
{
    private readonly FakeCollectionViewModel _collectionViewModel = new();

    [Fact]
    public void Bind_Model_AddedToItemsOnce()
    {
        var model = new ModelStub("Test", 2);

        _collectionViewModel.Bind(model);

        var child = _collectionViewModel.Items.SingleOrDefault(vm => Equals(vm.ActiveModel, model));

        Assert.NotNull(child);
        Assert.Equal(2, _collectionViewModel.Items[0].Value);
    }

    [Fact]
    public void Bind_ViewModel_AddedToItemsOnce()
    {
        var model = new ModelStub("Test", 2);
        var viewModel = new ViewModelStub();

        viewModel.Bind(model);

        _collectionViewModel.Bind(viewModel);

        var child = _collectionViewModel.Items.Single(vm => Equals(vm.ActiveModel, model));

        Assert.NotNull(child);
    }

    [Fact]
    public void Bind_ViewModelTwice_AddedToItemsOnce()
    {
        var model = new ModelStub("Test", 2);
        var viewModel = new ViewModelStub();

        viewModel.Bind(model);

        _collectionViewModel.Bind(viewModel);
        _collectionViewModel.Bind(viewModel);

        var child = _collectionViewModel.Items.Single(vm => Equals(vm.ActiveModel, model));

        Assert.NotNull(child);
    }

    [Fact]
    public void Bind_Model_ModelIsBound()
    {
        var model = new ModelStub("Test", 2);

        _collectionViewModel.Bind(model);

        Assert.True(_collectionViewModel.IsBound(model));
    }

    [Fact]
    public void Bind_ViewModel_ModelIsBound()
    {
        var model = new ModelStub("Test", 2);
        var viewModel = new ViewModelStub();

        viewModel.Bind(model);

        _collectionViewModel.Bind(viewModel);

        Assert.True(_collectionViewModel.IsBound(model));
    }

    [Fact]
    public void Bind_ExistingModel_UpdatesOnly()
    {
        var model = new ModelStub("Test", 2);

        _collectionViewModel.Bind(model);

        Assert.Single(_collectionViewModel.Items);
        Assert.Equal(2, _collectionViewModel.Items[0].Value);

        model.Value = 3;

        _collectionViewModel.Bind(model);

        Assert.Single(_collectionViewModel.Items);
        Assert.Equal(3, _collectionViewModel.Items[0].Value);
    }

    [Fact]
    public void BindMany_ExistingModel_UpdatesOnly()
    {
        var models = new List<ModelStub> {new("Test", 2), new("Another", 4)};

        _collectionViewModel.Bind(models);

        Assert.Equal(2, _collectionViewModel.Items.Count);
        Assert.Equal(2, _collectionViewModel.Items[0].Value);

        models[0].Value = 3;

        _collectionViewModel.Bind(models);

        Assert.Equal(2, _collectionViewModel.Items.Count);
        Assert.Equal(3, _collectionViewModel.Items[0].Value);
    }

    [Fact]
    public void AddItems_ViewModel_ModelIsBound()
    {
        var model = new ModelStub("Test", 2);
        var viewModel = new ViewModelStub();

        viewModel.Bind(model);

        _collectionViewModel.Items.Add(viewModel);
            
        Assert.True(_collectionViewModel.IsBound(model));
    }

    [Fact]
    public void RemoveItems_ViewModel_ModelUnbound()
    {
        var model = new ModelStub("Test", 2);

        _collectionViewModel.Bind(model);

        _collectionViewModel.Items.Remove(_collectionViewModel.Items.First());

        Assert.False(_collectionViewModel.IsBound(model));
    }

    [Fact]
    public void Unbind_ViewModel_ModelUnbound()
    {
        var model = new ModelStub("Test", 2);

        _collectionViewModel.Bind(model);

        var child = _collectionViewModel.Items.First();

        _collectionViewModel.Unbind(child);

        Assert.False(_collectionViewModel.IsBound(model));
    }

    [Fact]
    public void Unbind_ViewModel_RemovedFromItems()
    {
        var model = new ModelStub("Test", 2);

        _collectionViewModel.Bind(model);

        var child = _collectionViewModel.Items.First();

        _collectionViewModel.Unbind(child);

        child = _collectionViewModel.Items.FirstOrDefault(vm => Equals(vm.ActiveModel, model));

        Assert.Null(child);
    }

    [Fact]
    public void Unbind_Model_RemovedFromItems()
    {
        var model = new ModelStub("Test", 2);

        _collectionViewModel.Bind(model);

        _collectionViewModel.Unbind(model);

        var child = _collectionViewModel.Items.FirstOrDefault(vm => Equals(vm.ActiveModel, model));

        Assert.Null(child);
    }
        
    private sealed class FakeCollectionViewModel : CollectionViewModel<ModelStub, ViewModelStub>
    {
        public FakeCollectionViewModel() : base(new CollectionViewModelOptions { AsyncBatchBindings = false })
        { }

        public override ViewModelStub CreateItem(ModelStub model)
        {
            var stub = new ViewModelStub();

            stub.Bind(model);

            return stub;
        }

        public override void UpdateItem(ModelStub model)
        {
            var existingChild = FindItem<ViewModelStub>(model);

            existingChild?.Bind(model);
        }
    }

}