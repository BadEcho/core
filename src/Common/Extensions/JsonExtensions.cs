// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Nodes;

namespace BadEcho.Extensions;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to JSON.
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Inserts or updates the properties of this node while preserving the rest of its structure.
    /// </summary>
    /// <param name="target">The node to merge <c>source</c> to. If null, a new <see cref="JsonObject"/> is created.</param>
    /// <param name="source">The node to read the properties being merged from.</param>
    /// <returns>The merged <see cref="JsonNode"/> instance.</returns>
    /// <remarks>
    /// This merges the properties found on a source node to either the root or a property of a target node, without affecting
    /// properties only existing on the source node at or above the level the target node is being merged to.
    /// </remarks>
    public static JsonNode MergeNodes(this JsonNode? target, JsonNode source)
        => target.MergeNodes(source, string.Empty);

    /// <summary>
    /// Inserts or updates the properties of this node while preserving the rest of its structure.
    /// </summary>
    /// <param name="target">The node to merge <c>source</c> to. If null, a new <see cref="JsonObject"/> is created.</param>
    /// <param name="source">The node to read the properties being merged from.</param>
    /// <param name="sourcePropertyName">
    /// The name of the property on <c>target</c> to merge properties on <c>source</c> to. If empty, then properties are merged
    /// to the root of <c>target</c>.
    /// </param>
    /// <returns>The merged <see cref="JsonNode"/> instance.</returns>
    /// <remarks>
    /// This merges the properties found on a source node to either the root or a property of a target node, without affecting
    /// properties only existing on the source node at or above the level the target node is being merged to.
    /// </remarks>
    public static JsonNode MergeNodes(this JsonNode? target, JsonNode source, string sourcePropertyName)
    {
        Require.NotNull(source, nameof(source));
        
        bool isArray = source.GetValueKind() == JsonValueKind.Array;
        
        if (target == null)
        {   // We allow null targets in cases where the node's origin doesn't exist or the node is the result
            // of parsing JSON text that represents a null JSON value.
            target = new JsonObject();

            if (isArray)
            {   // Note that JSON configuration providers do not allow arrays as top-level nodes.
                target = string.IsNullOrEmpty(sourcePropertyName)
                    ? source
                    : new JsonObject
                      {
                          { sourcePropertyName, source }
                      };
            }
            else
            {   
                if (!string.IsNullOrEmpty(sourcePropertyName))
                    target[sourcePropertyName] = source;
                else
                    target = source;
            }
        }
        else
        {   // We have a JSON target node to merge to. Only nodes belonging to our source node saving will be applied.
            JsonNode nodeToUpdate = GetJsonNodeToUpdate(target, sourcePropertyName, isArray);

            // This preserves the rest of JSON's structure, allowing us to update JSON containing any number of
            // different properties safely.
            ReplaceProperties(nodeToUpdate, source, isArray);
        }
        
        return target;
    }

    private static JsonNode GetJsonNodeToUpdate(JsonNode root, string propertyName, bool isArray)
    {
        JsonNode? nodeToUpdate;

        if (string.IsNullOrEmpty(propertyName))
            nodeToUpdate = root;
        else
        {
            nodeToUpdate = root[propertyName];

            if (nodeToUpdate == null) 
                root[propertyName] = nodeToUpdate = isArray ? new JsonArray() : new JsonObject();
        }

        return nodeToUpdate;
    }

    private static void ReplaceProperties(JsonNode target, JsonNode source, bool isArray)
    {
        if (isArray)
            ReplaceArrayProperties(target, source);
        else
            ReplaceObjectProperties(target, source);
    }

    private static void ReplaceObjectProperties(JsonNode target, JsonNode source)
    {
        JsonObject sourceObject = source.AsObject();

        IEnumerable<string> properties = sourceObject.Select(kv => kv.Key);

        foreach (string property in properties)
        {
            ReplaceProperty(target, source, property);
        }
    }

    private static void ReplaceArrayProperties(JsonNode target, JsonNode source)
    {
        JsonArray targetArray = target.AsArray();
        JsonArray sourceArray = source.AsArray();

        for (int i = 0; i < sourceArray.Count; i++)
        {
            JsonNode? sourceItem = sourceArray[i];

            if (sourceItem == null)
                sourceArray[i] = sourceItem = new JsonObject();

            JsonNode? targetItem = targetArray[i];

            if (targetItem == null)
                targetArray[i] = targetItem = new JsonObject();

            ReplaceProperties(targetItem, sourceItem, sourceItem.GetValueKind() == JsonValueKind.Array);
        }
    }

    private static void ReplaceProperty(JsonNode target, JsonNode source, string property)
    {
        JsonNode? targetProperty = target[property];

        if (targetProperty == null) 
            target[property] = targetProperty = new JsonObject();
        
        JsonNode? sourceProperty = source[property];
        object? sourcePropertyValue = sourceProperty?.GetValueKind() == JsonValueKind.Object
            ? sourceProperty.DeepClone()
            // The underlying value type of the property is preserved when using GetValue like this.
            : sourceProperty?.GetValue<object>();

        targetProperty.ReplaceWith(sourcePropertyValue);
    }
}
