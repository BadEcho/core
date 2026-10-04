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
using BadEcho.Properties;
using BadEcho.Serialization;
using Xunit;

namespace BadEcho.Tests.Serialization;

public class JsonIntFuncConverterTests
{
    private static readonly JsonIntFuncConverter<TimeSpan> _Converter
        = new(seconds => TimeSpan.FromSeconds(seconds), time => (int)time.TotalSeconds);

    [Fact]
    public void Read_Integer_ValidConversion()
    {
        Assert.Equal(TimeSpan.FromSeconds(90), JsonSerializer.Deserialize<TimeSpan>("90", CreateOptions()));
    }

    [Fact]
    public void Write_Value_ValidJson()
    {
        Assert.Equal("90", JsonSerializer.Serialize(TimeSpan.FromSeconds(90), CreateOptions()));
    }

    [Fact]
    public void Read_NonIntegerDirect_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() =>
        {
            var reader = new Utf8JsonReader("1.5"u8);
            reader.Read();

            return _Converter.Read(ref reader, typeof(TimeSpan), JsonSerializerOptions.Default);
        });
    }

    [Fact]
    public void Read_NonInteger_ThrowsNotInt32()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<TimeSpan>("1.5", CreateOptions()));

        Assert.Equal(Strings.JsonNumberNotInt32, exception.Message);
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions();

        options.Converters.Add(_Converter);

        return options;
    }
}
