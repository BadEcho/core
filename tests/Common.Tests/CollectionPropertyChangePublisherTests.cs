// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xunit;

namespace BadEcho.Tests;

public class CollectionPropertyChangePublisherTests
{
    private readonly ObservableCollection<FakeItem> _collection = [];
    private readonly List<object?> _changedItems = [];

    public CollectionPropertyChangePublisherTests()
    {
        var publisher = new CollectionPropertyChangePublisher<FakeItem>(_collection);

        publisher.ItemChanged += (sender, _) => _changedItems.Add(sender);
    }

    [Fact]
    public void Add_ItemChanged_Raised()
    {
        var item = new FakeItem();
        _collection.Add(item);

        item.Value = 1;

        Assert.Single(_changedItems, item);
    }

    [Fact]
    public void Remove_ItemChanged_NotRaised()
    {
        var item = new FakeItem();
        _collection.Add(item);
        _collection.Remove(item);

        item.Value = 1;

        Assert.Empty(_changedItems);
    }

    [Fact]
    public void Move_ItemChanged_Raised()
    {
        var first = new FakeItem();
        var second = new FakeItem();
        _collection.Add(first);
        _collection.Add(second);

        _collection.Move(0, 1);
        first.Value = 1;

        Assert.Single(_changedItems, first);
    }

    [Fact]
    public void ReplaceWithSelf_ItemChanged_Raised()
    {
        var item = new FakeItem();
        _collection.Add(item);

        _collection[0] = item;
        item.Value = 1;

        Assert.Single(_changedItems, item);
    }

    [Fact]
    public void ReplaceWithOther_OldItemChanged_NotRaised()
    {
        var oldItem = new FakeItem();
        var newItem = new FakeItem();
        _collection.Add(oldItem);

        _collection[0] = newItem;
        oldItem.Value = 1;
        newItem.Value = 1;

        Assert.Single(_changedItems, newItem);
    }

    private sealed class FakeItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int Value
        {
            get;
            set
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }
    }
}
