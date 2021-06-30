//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace BadEcho.Odin.Tests.Json
{
    public class JsonPolymorphicConverterTests
    {
        private const string JSON_FAKE_OBJECT =
            @"[ { ""Type"": 0, ""Object"": { ""SomeIdentifier"": ""hello there"" } } ]";

        private const string JSON_OUT_OF_ORDER_OBJECT =
            @"[ { ""Object"": { ""SomeIdentifier"": ""hello there"" }, ""Type"": 0 } ]";

        [Fact]
        public void Read_First_ValidConversion()
        {
            var fakeObjects = Deserialize(JSON_FAKE_OBJECT);

            Assert.NotNull(fakeObjects);

            var fakeFirstObjects = fakeObjects.OfType<FirstFakeJsonObject>().ToList();

            Assert.NotEmpty(fakeFirstObjects);

            var fakeObject = fakeFirstObjects.First();

            Assert.Equal("hello there", fakeObject.SomeIdentifier);
        }

        [Fact]
        public void Read_OutOfOrderFirst_ValidConversion()
        {
            var fakeObjects = Deserialize(JSON_OUT_OF_ORDER_OBJECT);

            Assert.NotNull(fakeObjects);

            var fakeFirstObjects = fakeObjects.OfType<FirstFakeJsonObject>().ToList();

            Assert.NotEmpty(fakeFirstObjects);

            var fakeObject = fakeFirstObjects.First();

            Assert.Equal("hello there", fakeObject.SomeIdentifier);
        }

        private static IEnumerable<FakeJsonObject> Deserialize(string json)
        {
            var options = new JsonSerializerOptions();

            options.Converters.Add(new FakeJsonObjectConverter());

            var fakeObject = JsonSerializer.Deserialize<IEnumerable<FakeJsonObject>>(json, options);

            Assert.NotNull(fakeObject);

            return fakeObject!;
        }
    }
}
