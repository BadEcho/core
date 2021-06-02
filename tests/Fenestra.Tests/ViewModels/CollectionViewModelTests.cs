//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using BadEcho.Fenestra.ViewModels;
using Xunit;

namespace BadEcho.Fenestra.Tests.ViewModels
{
    public class CollectionViewModelTests
    {
        private readonly FakeCollectionViewModel _viewModel;

        public CollectionViewModelTests()
        {
            _viewModel = new FakeCollectionViewModel();
        }

        [Fact]
        public void Bind_Model_AddedToChildrenOnce()
        {
            _viewModel.Bind(2);

            var child = _viewModel.Children.SingleOrDefault(vm => vm.ActiveModel == 2);

            Assert.NotNull(child);
        }

        [Fact]
        public void Bind_ViewModel_AddedToChildrenOnce()
        {
            var stub = new ViewModelStub();

            stub.Bind(2);

            _viewModel.Bind(stub);

            var child = _viewModel.Children.Single(vm => vm.ActiveModel == 2);

            Assert.NotNull(child);
        }

        [Fact]
        public void Bind_ViewModelTwice_AddedToChildrenOnce()
        {
            var stub = new ViewModelStub();

            stub.Bind(2);

            _viewModel.Bind(stub);
            _viewModel.Bind(stub);

            var child = _viewModel.Children.Single(vm => vm.ActiveModel == 2);

            Assert.NotNull(child);
        }

        [Fact]
        public void Bind_Model_ModelIsBound()
        {
            _viewModel.Bind(2);

            Assert.True(_viewModel.IsBound(2));
        }

        [Fact]
        public void Bind_ViewModel_ModelIsBound()
        {
            var stub = new ViewModelStub();

            stub.Bind(2);

            _viewModel.Bind(stub);

            Assert.True(_viewModel.IsBound(2));
        }

        [Fact]
        public void AddChildren_ViewModel_ModelIsBound()
        {
            var stub = new ViewModelStub();

            stub.Bind(2);

            _viewModel.Children.Add(stub);
            
            Assert.True(_viewModel.IsBound(2));
        }

        [Fact]
        public void RemoveChildren_ViewModel_ModelUnbound()
        {
            _viewModel.Bind(2);

            _viewModel.Children.Remove(_viewModel.Children.First());

            Assert.False(_viewModel.IsBound(2));
        }

        [Fact]
        public void Unbind_ViewModel_ModelUnbound()
        {
            _viewModel.Bind(2);

            var child = _viewModel.Children.First();

            _viewModel.Unbind(child);

            Assert.False(_viewModel.IsBound(2));
        }

        [Fact]
        public void Unbind_ViewModel_RemovedFromChildren()
        {
            _viewModel.Bind(2);

            var child = _viewModel.Children.First();

            _viewModel.Unbind(child);

            child = _viewModel.Children.FirstOrDefault(vm => vm.ActiveModel == 2);

            Assert.Null(child);
        }

        [Fact]
        public void Unbind_Model_RemovedFromChildren()
        {
            _viewModel.Bind(2);

            _viewModel.Unbind(2);

            var child = _viewModel.Children.FirstOrDefault(vm => vm.ActiveModel == 2);

            Assert.Null(child);
        }

        
        private sealed class FakeCollectionViewModel : CollectionViewModel<int, ViewModelStub>
        {
            public FakeCollectionViewModel() : base(new CollectionViewModelOptions())
            { }

            public override ViewModelStub CreateChild(int model)
            {
                var stub = new ViewModelStub();

                stub.Bind(model);

                return stub;
            }

            public override void OnChangeCompleted()
            { }
        }

        private sealed class ViewModelStub : ViewModel<int>
        {
            protected override void OnBinding(int model)
            { }

            protected override void OnUnbound(int model)
            { }
        }
    }
}
