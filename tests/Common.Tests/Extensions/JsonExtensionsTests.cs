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
    [Fact]
    public void MergeNodes_NullTargetSourceWithParent_DoesNotReparentSource()
    {
        var sourceRoot = new JsonObject { ["Section"] = new JsonObject { ["Value"] = 1 } };
        JsonNode source = sourceRoot["Section"]!;
        JsonNode? target = null;

        JsonNode result = target.MergeNodes(source, "Section");

        Assert.Equal("""{"Section":{"Value":1}}""", result.ToJsonString());
        Assert.Same(sourceRoot, source.Parent);
    }

    [Fact]
    public void MergeNodes_NullTargetToRoot_ReturnsCopy()
    {
        JsonNode source = new JsonObject { ["Value"] = 1 };
        JsonNode? target = null;

        JsonNode result = target.MergeNodes(source);

        Assert.NotSame(source, result);
        Assert.Equal(source.ToJsonString(), result.ToJsonString());
    }

    [Fact]
    public void MergeNodes_SectionOnlyProperties_Preserved()
    {
        JsonNode target = JsonNode.Parse("""{"Section":{"A":1,"TargetOnly":2},"Other":true}""")!;
        JsonNode source = JsonNode.Parse("""{"A":3}""")!;

        JsonNode result = target.MergeNodes(source, "Section");

        Assert.Equal("""{"Section":{"A":3,"TargetOnly":2},"Other":true}""", result.ToJsonString());
    }

    [Fact]
    public void MergeNodes_NestedObject_ReplacedWholesale()
    {
        JsonNode target = JsonNode.Parse("""{"Section":{"Nested":{"A":1,"B":2}}}""")!;
        JsonNode source = JsonNode.Parse("""{"Nested":{"A":3}}""")!;

        JsonNode result = target.MergeNodes(source, "Section");

        Assert.Equal("""{"Section":{"Nested":{"A":3}}}""", result.ToJsonString());
    }

    [Fact]
    public void MergeNodes_ScalarSource_ReplacesProperty()
    {
        JsonNode target = JsonNode.Parse("""{"Section":{"A":1},"Other":true}""")!;
        JsonNode source = JsonValue.Create(5);

        JsonNode result = target.MergeNodes(source, "Section");

        Assert.Equal("""{"Section":5,"Other":true}""", result.ToJsonString());
    }

    [Fact]
    public void MergeNodes_KindMismatch_ReplacesProperty()
    {
        JsonNode target = JsonNode.Parse("""{"Section":[1,2],"Other":true}""")!;
        JsonNode source = JsonNode.Parse("""{"A":1}""")!;

        JsonNode result = target.MergeNodes(source, "Section");

        Assert.Equal("""{"Section":{"A":1},"Other":true}""", result.ToJsonString());
    }
}
