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

using System.Text.Json.Nodes;
using BadEcho.Extensions;
using Xunit;

namespace BadEcho.Tests.Extensions;

public class JsonExtensionsTests
{
    [Fact]
    public void MergeNodes_ScalarArray_ReplacesItems()
    {
        JsonNode target = JsonNode.Parse("""{"ids":[9,8,7,6],"other":true}""")!;
        JsonNode source = JsonNode.Parse("[1,2,3]")!;

        JsonNode merged = target.MergeNodes(source, "ids");

        Assert.Equal("""{"ids":[1,2,3],"other":true}""", merged.ToJsonString());
    }

    [Fact]
    public void MergeNodes_ArrayWithNull_SourceUnchanged()
    {
        JsonNode target = new JsonObject();
        JsonNode source = JsonNode.Parse("""[{"a":1},null]""")!;

        JsonNode merged = target.MergeNodes(source, "items");

        Assert.Equal("""[{"a":1},null]""", source.ToJsonString());
        Assert.Equal("""{"items":[{"a":1},null]}""", merged.ToJsonString());
    }

    [Fact]
    public void MergeNodes_ObjectArrayItems_PreservesTargetOnlyProperties()
    {
        JsonNode target = JsonNode.Parse("""{"items":[{"a":1,"b":2}]}""")!;
        JsonNode source = JsonNode.Parse("""[{"a":5}]""")!;

        JsonNode merged = target.MergeNodes(source, "items");

        Assert.Equal("""{"items":[{"a":5,"b":2}]}""", merged.ToJsonString());
    }
}
