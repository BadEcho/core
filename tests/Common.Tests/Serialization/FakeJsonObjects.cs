//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;
using BadEcho.Serialization;

namespace BadEcho.Tests.Serialization;

/// <suppresions>
/// ReSharper disable LocalizableElement
/// </suppresions>
public abstract class FakeJsonObject
{ }

public sealed class FirstFakeJsonObject : FakeJsonObject
{
    public string? SomeIdentifier
    { get; set; }
}

public sealed class SecondFakeJsonObject : FakeJsonObject
{
    public string? SomeOtherIdentifier
    { get; set; }
}

public enum FakeJsonObjectType
{
    First,
    Second
}

public sealed class FakeJsonObjectConverter : JsonPolymorphicConverter<FakeJsonObjectType, FakeJsonObject>
{
    protected override string DataPropertyName
        => "Object";

    protected override FakeJsonObject? ReadFromDescriptor(ref Utf8JsonReader reader, FakeJsonObjectType typeDescriptor)
    {
        return typeDescriptor switch
        {
            FakeJsonObjectType.First => JsonSerializer.Deserialize<FirstFakeJsonObject>(ref reader),
            FakeJsonObjectType.Second => JsonSerializer.Deserialize<SecondFakeJsonObject>(ref reader),
            _ => throw new InvalidEnumArgumentException(nameof(typeDescriptor),
                                                        (int) typeDescriptor,
                                                        typeof(FakeJsonObjectType))
        };
    }

    protected override FakeJsonObjectType DescriptorFromValue(FakeJsonObject value)
    {
        return value switch
        {
            FirstFakeJsonObject => FakeJsonObjectType.First,
            SecondFakeJsonObject => FakeJsonObjectType.Second,
            _ => throw new ArgumentException("Type described in JSON not supported.",
                                             nameof(value))
        };
    }
}