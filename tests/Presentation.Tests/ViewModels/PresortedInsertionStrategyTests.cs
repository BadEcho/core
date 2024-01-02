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

public class PresortedInsertionStrategyTests
{
    private readonly PresortedInsertionStrategy<ViewModelStub, int> _strategy
        = new(x => x.Value);

    private readonly FakeAncestorViewModel _collectionViewModel = new();

    public PresortedInsertionStrategyTests()
    {
        var a = new ModelStub("A", 1);
        var b = new ModelStub("B", 8);
        var c = new ModelStub("C", 13);
        var d = new ModelStub("D", 2);

        var aVm = new ViewModelStub();
        var bVm = new ViewModelStub();
        var cVm = new ViewModelStub();
        var dVm = new ViewModelStub();

        aVm.Bind(a);
        bVm.Bind(b);
        cVm.Bind(c);
        dVm.Bind(d);

        _collectionViewModel.Items.Add(aVm);
        _collectionViewModel.Items.Add(dVm);
        _collectionViewModel.Items.Add(bVm);
        _collectionViewModel.Items.Add(cVm);
    }

    [Fact]
    public void Add_NewItems_AddedAndSorted()
    {
        var newItem = new ModelStub("New", 3);
        var newChild = new ViewModelStub();

        newChild.Bind(newItem);

        _strategy.Add(_collectionViewModel, newChild);

        Assert.Equal(newChild, _collectionViewModel.Items[2]);
    }

    [Fact]
    public void AddRange_NewItems_AddedAndSorted()
    {
        var newItem1 = new ModelStub("New1", 3);
        var newItem2 = new ModelStub("New2", 0);
        var newItem3 = new ModelStub("New3", -1);
        var newItem4 = new ModelStub("New4", 15);
        var newItem5 = new ModelStub("New5", 7);

        var newChild1 = new ViewModelStub();
        var newChild2 = new ViewModelStub();
        var newChild3 = new ViewModelStub();
        var newChild4 = new ViewModelStub();
        var newChild5 = new ViewModelStub();

        newChild1.Bind(newItem1);
        newChild2.Bind(newItem2);
        newChild3.Bind(newItem3);
        newChild4.Bind(newItem4);
        newChild5.Bind(newItem5);

        var newItems = new List<ViewModelStub> { newChild1, newChild2, newChild3, newChild4, newChild5 };

        _strategy.AddRange(_collectionViewModel, newItems);

        Assert.Equal(newChild1, _collectionViewModel.Items[4]);
        Assert.Equal(newChild2, _collectionViewModel.Items[1]);
        Assert.Equal(newChild3, _collectionViewModel.Items[0]);
        Assert.Equal(newChild4, _collectionViewModel.Items[8]);
        Assert.Equal(newChild5, _collectionViewModel.Items[5]);
    }
}