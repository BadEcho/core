// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using Xunit;

namespace BadEcho.Tests.Serialization;

public class JsonRangeConverterTests
{
    private const string JSON_TWO_RANGES =
        """{ "ranges": [ { "start": 5, "end": 8 }, { "start": 12, "end": 20 } ] }""";

    [Fact]
    public void Read_TwoRanges_ValidConversion()
    {
        var rangeObject
            = JsonSerializer.Deserialize<RangeFakeJsonObject>(JSON_TWO_RANGES,
                                                              new JsonSerializerOptions
                                                              { PropertyNameCaseInsensitive = true });
        Assert.NotNull(rangeObject);
        Assert.NotNull(rangeObject.Ranges);
        Assert.Collection(rangeObject.Ranges,
                          t => Assert.Equal(5, t),
                          t => Assert.Equal(6, t),
                          t => Assert.Equal(7, t),
                          t => Assert.Equal(8, t),
                          t => Assert.Equal(12, t),
                          t => Assert.Equal(13, t),
                          t => Assert.Equal(14, t),
                          t => Assert.Equal(15, t),
                          t => Assert.Equal(16, t),
                          t => Assert.Equal(17, t),
                          t => Assert.Equal(18, t),
                          t => Assert.Equal(19, t),
                          t => Assert.Equal(20, t));
    }

    [Fact]
    public void Write_TwoRanges_ValidConversion()
    {
        var rangeObject = new RangeFakeJsonObject
                          {
                              Ranges =
                              [
                                  5, 6, 7, 8, 12, 13, 14, 15, 16, 17, 18, 19, 20
                              ]
                          };

        var rangeJson = JsonSerializer.Serialize(rangeObject,
                                                 new JsonSerializerOptions
                                                 {
                                                     PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                                 });

        Assert.Equal(JSON_TWO_RANGES.Replace(" ", ""), rangeJson);
    }

    [Fact]
    public void Write_Duplicates_RoundTrips()
    {
        int[] values = [1, 2, 2, 3];

        Assert.Equal(values, RoundTrip(values));
    }

    [Fact]
    public void Write_Descending_RoundTrips()
    {
        int[] values = [5, 3];

        Assert.Equal(values, RoundTrip(values));
    }

    [Fact]
    public void Write_UnorderedRuns_RoundTrips()
    {
        int[] values = [80, 81, 82, 5, 6, 7];

        Assert.Equal(values, RoundTrip(values));
    }

    [Fact]
    public void Write_Duplicates_SplitsRanges()
    {
        var rangeObject = new RangeFakeJsonObject { Ranges = [1, 2, 2, 3] };

        var rangeJson = JsonSerializer.Serialize(rangeObject,
                                                 new JsonSerializerOptions
                                                 {
                                                     PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                                 });

        Assert.Equal("""{"ranges":[{"start":1,"end":2},{"start":2,"end":3}]}""", rangeJson);
    }

    private static IEnumerable<int>? RoundTrip(IEnumerable<int> values)
    {
        var options = new JsonSerializerOptions
                      {
                          PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                          PropertyNameCaseInsensitive = true
                      };

        string json = JsonSerializer.Serialize(new RangeFakeJsonObject { Ranges = values }, options);

        return JsonSerializer.Deserialize<RangeFakeJsonObject>(json, options)?.Ranges;
    }
}
