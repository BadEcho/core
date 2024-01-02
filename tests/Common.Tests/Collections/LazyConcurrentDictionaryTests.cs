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

using BadEcho.Collections;
using Xunit;

namespace BadEcho.Tests.Collections;

public class LazyConcurrentDictionaryTests
{
    private readonly LazyConcurrentDictionary<int,string> _lazyDictionary = new(
        new List<KeyValuePair<int, Lazy<string>>>
        {
            new (0, new Lazy<string>(() => "First")),
            new (1, new Lazy<string>(() => "Second"))
        },
        LazyThreadSafetyMode.ExecutionAndPublication);

    [Fact]
    public void GetOrAdd_Existing_ReturnsFirst()
    {
        var value = _lazyDictionary.GetOrAdd(0, () => "Not First");

        Assert.Equal("First", value.Value);
    }

    [Fact]
    public void GetOrAdd_New_NoFactoryUntilAccess()
    {
        var factoryRun = false;
        var value = _lazyDictionary.GetOrAdd(2, () =>
                                                {
                                                    factoryRun = true;
                                                    return "Third";
                                                });

        Assert.False(factoryRun);
        Assert.Equal("Third", value.Value);
    }

    [Fact]
    public void GetOrAdd_ExistingLazy_ReturnsFirst()
    {
        var value = _lazyDictionary.GetOrAdd(0, _ => new Lazy<string>("Not First"));

        Assert.NotNull(value);

        Assert.Equal("First", value.Value);
    }

    [Fact]
    public void GetOrAdd_ExistingLazy_NotFactoryUntilAccess()
    {
        var factoryRan = false;
        var value = _lazyDictionary
            .GetOrAdd(2,
                      _ => new Lazy<string>(() =>
                                            {
                                                factoryRan = true;
                                                return "Third";
                                            }));

        Assert.False(factoryRan);
        Assert.Equal("Third", value.Value);
    }

    [Fact]
    public void IReadOnlyDictionaryIndexer_Existing_ReturnsItems()
    {
        IReadOnlyDictionary<int, string> readOnly = _lazyDictionary;

        Assert.Equal("First", readOnly[0]);
        Assert.Equal("Second", readOnly[1]);
    }

    [Fact]
    public void IReadOnlyDictionaryIndexer_New_NoFactoryUntilAccess()
    {
        bool factoryRan = false;

        _lazyDictionary.GetOrAdd(2,
                                 () =>
                                 {
                                     factoryRan = true;
                                     return "Third";
                                 });

        IReadOnlyDictionary<int, string> readOnly = _lazyDictionary;

        Assert.Equal("First",readOnly[0]);

        Assert.False(factoryRan);

        Assert.Equal("Third", readOnly[2]);
    }

    [Fact]
    public void IReadOnlyDictionaryEnumerator_Existing_ReturnsItems()
    {
        IReadOnlyDictionary<int, string> readOnly = _lazyDictionary;

        using (var enumerator = readOnly.GetEnumerator())
        {
            enumerator.MoveNext();

            Assert.Equal(0, enumerator.Current.Key);
            Assert.Equal("First", enumerator.Current.Value);

            enumerator.MoveNext();

            Assert.Equal(1, enumerator.Current.Key);
            Assert.Equal("Second", enumerator.Current.Value);
        }
    }

    [Fact]
    public void IReadOnlyDictionaryEnumerator_New_NoFactoryUntilAccess()
    {
        bool factoryRan = false;

        _lazyDictionary.GetOrAdd(2,
                                 () =>
                                 {
                                     factoryRan = true;
                                     return "Third";
                                 });

        IReadOnlyDictionary<int, string> readOnly = _lazyDictionary;

        using (var enumerator = readOnly.GetEnumerator())
        {
            enumerator.MoveNext();

            Assert.False(factoryRan);

            Assert.Equal(0, enumerator.Current.Key);
            Assert.Equal("First", enumerator.Current.Value);

            enumerator.MoveNext();

            Assert.False(factoryRan);

            Assert.Equal(1, enumerator.Current.Key);
            Assert.Equal("Second", enumerator.Current.Value);

            enumerator.MoveNext();

            Assert.False(factoryRan);

            Assert.Equal(2, enumerator.Current.Key);
            Assert.Equal("Third", enumerator.Current.Value);

            Assert.True(factoryRan);
        }
    }

    [Fact]
    public void TryGetValue_Existing_ReturnsFirst()
    {
        Assert.True(_lazyDictionary.TryGetValue(0, out string? firstValue));

        Assert.Equal("First", firstValue);
    }

    [Fact]
    public void TryGetValue_NonExisting_ReturnsNull()
    {
        Assert.False(_lazyDictionary.TryGetValue(2, out string? noValue));
        Assert.Null(noValue);
    }
}