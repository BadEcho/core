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

using System.Text.Json;
using System.Windows;
using Xunit;

namespace BadEcho.Presentation.Tests.Serialization;

public class JsonThicknessConverterTests
{
    private const string FOUR_LENGTHS_OBJECT =
        """{"someThickness":"2,1,0,3"}""";

    private const string TWO_LENGTHS_OBJECT =
        """{"someThickness":"1,5"}""";

    private const string ONE_LENGTH_OBJECT =
        """{"someThickness":"8"}""";


    [Fact]
    public void Read_FourLengths_ValidConversion()
    {
        var fakeObject = Deserialize(FOUR_LENGTHS_OBJECT);

        Assert.NotNull(fakeObject);

        Assert.Equal(2, fakeObject.SomeThickness.Left);
        Assert.Equal(1, fakeObject.SomeThickness.Top);
        Assert.Equal(0, fakeObject.SomeThickness.Right);
        Assert.Equal(3, fakeObject.SomeThickness.Bottom);
    }

    [Fact]
    public void Write_FourLengths_ValidConversion()
    {
        var fakeObject = new FakeThicknessObject {SomeThickness = new Thickness(2, 1, 0, 3)};

        var jsonValue = Serialize(fakeObject);

        Assert.Equal(FOUR_LENGTHS_OBJECT, jsonValue);
    }

    [Fact]
    public void Read_TwoLengths_ValidConversion()
    {
        var fakeObject = Deserialize(TWO_LENGTHS_OBJECT);

        Assert.NotNull(fakeObject);

        Assert.Equal(1, fakeObject.SomeThickness.Left);
        Assert.Equal(5, fakeObject.SomeThickness.Top);
        Assert.Equal(1, fakeObject.SomeThickness.Right);
        Assert.Equal(5, fakeObject.SomeThickness.Bottom);
    }

    [Fact]
    public void Read_OneLength_ValidConversion()
    {
        var fakeObject = Deserialize(ONE_LENGTH_OBJECT);

        Assert.NotNull(fakeObject);

        Assert.Equal(8, fakeObject.SomeThickness.Left);
        Assert.Equal(8, fakeObject.SomeThickness.Top);
        Assert.Equal(8, fakeObject.SomeThickness.Right);
        Assert.Equal(8, fakeObject.SomeThickness.Bottom);
    }

    private static FakeThicknessObject Deserialize(string json)
    {
        var options = new JsonSerializerOptions
                      {
                          PropertyNameCaseInsensitive = true, 
                          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                      };

        var fakeObject = JsonSerializer.Deserialize<FakeThicknessObject>(json, options);

        return fakeObject!;
    }

    private static string Serialize(FakeThicknessObject fakeObject)
    {
        var options = new JsonSerializerOptions
                      {
                          PropertyNameCaseInsensitive = true,
                          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                      };

        return JsonSerializer.Serialize(fakeObject, options);
    }
}