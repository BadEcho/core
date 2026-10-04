// -----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using BadEcho.Extensions;
using BadEcho.Properties;
using Xunit;

namespace BadEcho.Tests.Serialization;

public class JsonPolymorphicConverterTests
{
    private const string JSON_FAKE_OBJECT =
        """[ { "Type": 0, "Object": { "SomeIdentifier": "hello there" } } ]""";

    private const string JSON_OUT_OF_ORDER_OBJECT =
        """[ { "Object": { "SomeIdentifier": "hello there" }, "Type": 0 } ]""";

    private const string JSON_FAKE_OBJECT_COMPACT =
        """[{"Type":0,"Object":{"SomeIdentifier":"hello there"}}]""";

    private const string JSON_EXTRA_LEADING_PROPERTY =
        """[ { "Type": 0, "Version": 2, "Object": { "SomeIdentifier": "hello there" } } ]""";

    private const string JSON_EXTRA_TRAILING_PROPERTY =
        """[ { "Type": 0, "Object": { "SomeIdentifier": "hello there" }, "Meta": { "Tags": [ 1, { "A": null } ] } } ]""";

    private const string JSON_OUT_OF_ORDER_ARRAY_DATA =
        """[ { "Object": [ { } ], "Type": 0 } ]""";

    private const string JSON_MISSING_TYPE =
        """[ { "Object": { "SomeIdentifier": "hello there" } } ]""";

    [Fact]
    public void Read_ExtraLeadingProperty_ValidConversion()
    {
        var fakeObject = Assert.IsType<FirstFakeJsonObject>(Assert.Single(Deserialize(JSON_EXTRA_LEADING_PROPERTY)));

        Assert.Equal("hello there", fakeObject.SomeIdentifier);
    }

    [Fact]
    public void Read_ExtraTrailingProperty_ValidConversion()
    {
        var fakeObject = Assert.IsType<FirstFakeJsonObject>(Assert.Single(Deserialize(JSON_EXTRA_TRAILING_PROPERTY)));

        Assert.Equal("hello there", fakeObject.SomeIdentifier);
    }

    [Fact]
    public void Read_OutOfOrderArrayData_ThrowsDataNotObject()
    {
        var exception = Assert.Throws<JsonException>(() => Deserialize(JSON_OUT_OF_ORDER_ARRAY_DATA).ToList());

        Assert.Equal(Strings.JsonDataValueNotObject, exception.Message);
    }

    [Fact]
    public void Read_MissingType_ThrowsMissingProperty()
    {
        var exception = Assert.Throws<JsonException>(() => Deserialize(JSON_MISSING_TYPE).ToList());

        Assert.Equal(Strings.JsonMissingProperty.InvariantFormat("Type"), exception.Message);
    }

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

    [Fact]
    public void Write_First_ValidJson()
    {
        FakeJsonObject[] fakeObjects = [new FirstFakeJsonObject { SomeIdentifier = "hello there" }];

        string json = Serialize(fakeObjects);

        Assert.Equal(JSON_FAKE_OBJECT_COMPACT, json);
    }

    [Fact]
    public void Write_Mixed_RoundTrips()
    {
        FakeJsonObject[] fakeObjects =
        [
            new FirstFakeJsonObject { SomeIdentifier = "hello there" },
            new SecondFakeJsonObject { SomeOtherIdentifier = "general kenobi" }
        ];

        var roundTripped = Deserialize(Serialize(fakeObjects)).ToList();

        Assert.Collection(roundTripped,
                          first => Assert.Equal("hello there",
                                                Assert.IsType<FirstFakeJsonObject>(first).SomeIdentifier),
                          second => Assert.Equal("general kenobi",
                                                 Assert.IsType<SecondFakeJsonObject>(second).SomeOtherIdentifier));
    }

    private static string Serialize(IEnumerable<FakeJsonObject> fakeObjects)
    {
        var options = new JsonSerializerOptions();

        options.Converters.Add(new FakeJsonObjectConverter());

        return JsonSerializer.Serialize(fakeObjects, options);
    }

    private static IEnumerable<FakeJsonObject> Deserialize(string json)
    {
        var options = new JsonSerializerOptions();

        options.Converters.Add(new FakeJsonObjectConverter());

        var fakeObject = JsonSerializer.Deserialize<IEnumerable<FakeJsonObject>>(json, options);

        return fakeObject!;
    }
}