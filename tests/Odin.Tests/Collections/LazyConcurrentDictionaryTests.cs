using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BadEcho.Odin.Collections;
using Xunit;

namespace BadEcho.Odin.Tests.Collections
{
    public class LazyConcurrentDictionaryTests
    {
        private readonly LazyConcurrentDictionary<int,string> _lazyDictionary;

        public LazyConcurrentDictionaryTests()
        {
            _lazyDictionary = new LazyConcurrentDictionary<int, string>(
                new List<KeyValuePair<int, Lazy<string>>>
                {
                    new (0, new Lazy<string>(() => "First")),
                    new (1, new Lazy<string>(() => "Second"))
                },
            LazyThreadSafetyMode.ExecutionAndPublication);
        }

        [Fact]
        public void GetOrAdd_Existing()
        {
            var value = _lazyDictionary.GetOrAdd(0, () => "Not First");

            Assert.Equal("First", value.Value);
        }

        [Fact]
        public void GetOrAdd_New()
        {
            var value = _lazyDictionary.GetOrAdd(2, () => "Third");

            Assert.Equal("Third", value.Value);
        }

        [Fact]
        public void GetOrAdd_ExistingLazy()
        {
            var value = _lazyDictionary.GetOrAdd(0, _ => new Lazy<string>("Not First"));

            Assert.NotNull(value);

            Assert.Equal("First", value.Value);
        }

        [Fact]
        public void GetOrAdd_LazyUntilNewAccess()
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
        public void IReadOnlyDictionary_Indexer_Existing()
        {
            IReadOnlyDictionary<int, string?> readOnly = _lazyDictionary;

            Assert.Equal("First", readOnly[0]);
            Assert.Equal("Second", readOnly[1]);
        }

        [Fact]
        public void IReadOnlyDictionary_Indexer_LazyUntilNewAccess()
        {
            bool factoryRan = false;

            _lazyDictionary.GetOrAdd(2,
                                     () =>
                                     {
                                         factoryRan = true;
                                         return "Third";
                                     });

            IReadOnlyDictionary<int, string?> readOnly = _lazyDictionary;

            Assert.Equal("First",readOnly[0]);

            Assert.False(factoryRan);

            Assert.Equal("Third", readOnly[2]);
        }

        [Fact]
        public void IReadOnlyDictionary_Enumerator_Existing()
        {
            IReadOnlyDictionary<int, string?> readOnly = _lazyDictionary;

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
        public void IReadOnlyDictionary_Enumerator_LazyUntilNewAccess()
        {
            bool factoryRan = false;

            _lazyDictionary.GetOrAdd(2,
                                     () =>
                                     {
                                         factoryRan = true;
                                         return "Third";
                                     });

            IReadOnlyDictionary<int, string?> readOnly = _lazyDictionary;

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
        public void TryGetValue_Existing()
        {
            Assert.True(_lazyDictionary.TryGetValue(0, out string? firstValue));

            Assert.Equal("First", firstValue);
        }

        [Fact]
        public void TryGetValue_NonExisting()
        {
            Assert.False(_lazyDictionary.TryGetValue(2, out string? noValue));
            Assert.Null(noValue);
        }
    }
}
