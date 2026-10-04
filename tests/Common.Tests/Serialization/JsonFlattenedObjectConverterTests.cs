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
using BadEcho.Serialization;
using Xunit;

namespace BadEcho.Tests.Serialization;

public class JsonFlattenedObjectConverterTests
{
    private const string JSON_TWO_OBJECTS =
        """[ { "SomeIdentifier": "hello there" }, { "SomeOtherIdentifier": "general Kenobi" } ]""";

    private const string JSON_ONE_OBJECT =
        """[ { "SomeIdentifier": "hello there" } ]""";

    private const string JSON_NUMBER_SIBLING =
        """[ { "SomeIdentifier": "hello there" }, 5 ]""";

    private const string JSON_DUPLICATE_PROPERTY =
        """[ { "SomeIdentifier": "hello there" }, { "SomeIdentifier": "general Kenobi" } ]""";

    [Fact]
    public void Read_TwoObjects_Flattened()
    {
        var flattened = Assert.Single(Deserialize(JSON_TWO_OBJECTS));

        Assert.Equal("hello there", flattened.SomeIdentifier);
        Assert.Equal("general Kenobi", flattened.SomeOtherIdentifier);
    }

    [Fact]
    public void Read_TwoObjectsTwice_Flattened()
    {
        var options = CreateOptions();

        Assert.Single(JsonSerializer.Deserialize<List<FlattenedFakeJsonObject>>(JSON_TWO_OBJECTS, options)!);
        Assert.Single(JsonSerializer.Deserialize<List<FlattenedFakeJsonObject>>(JSON_TWO_OBJECTS, options)!);
    }

    [Fact]
    public void Write_RegisteredInOptions_ValidJson()
    {
        var flattened = new FlattenedFakeJsonObject
        {
            SomeIdentifier = "hello there",
            SomeOtherIdentifier = "general Kenobi"
        };

        string json = JsonSerializer.Serialize(flattened, CreateOptions());

        Assert.Equal("""{"SomeIdentifier":"hello there","SomeOtherIdentifier":"general Kenobi"}""", json);
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions();

        options.Converters.Add(new JsonFlattenedObjectConverter<FlattenedFakeJsonObject>(2));

        return options;
    }

    private static List<FlattenedFakeJsonObject> Deserialize(string json)
        => JsonSerializer.Deserialize<List<FlattenedFakeJsonObject>>(json, CreateOptions())!;
}