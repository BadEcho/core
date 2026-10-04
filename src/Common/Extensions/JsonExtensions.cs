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

        bool mergeToRoot = string.IsNullOrEmpty(sourcePropertyName);
        
        if (target == null)
        {   // We allow null targets in cases where the node's origin doesn't exist or the node is the result
            // of parsing JSON text that represents a null JSON value.
            return mergeToRoot
                ? source.DeepClone()
                : new JsonObject
                  {
                      { sourcePropertyName, source.DeepClone() }
                  };
        }

        JsonNode? nodeToUpdate = mergeToRoot ? target : target[sourcePropertyName];
        JsonValueKind sourceKind = source.GetValueKind();

        if (nodeToUpdate == null || nodeToUpdate.GetValueKind() != sourceKind || sourceKind is not (JsonValueKind.Object or JsonValueKind.Array))
        {
            JsonNode replacement = source.DeepClone();

            if (mergeToRoot)
                return replacement;

            target[sourcePropertyName] = replacement;

            return target;
        }

        // This preserves the rest of JSON's structure, allowing us to update JSON containing any number of
        // different properties safely.
        ReplaceProperties(nodeToUpdate, source, sourceKind == JsonValueKind.Array);

        return target;
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

        if (targetArray.Count > sourceArray.Count)
        {
            int difference = targetArray.Count - sourceArray.Count;

            targetArray.RemoveRange(sourceArray.Count, difference);
        }

        for (int i = 0; i < sourceArray.Count; i++)
        {
            JsonNode? targetItem = i < targetArray.Count ? targetArray[i] : null;
            JsonNode? mergedItem = MergeArrayItem(targetItem, sourceArray[i]);

            if (i == targetArray.Count) 
                targetArray.Add(mergedItem);
            else if (!ReferenceEquals(mergedItem, targetItem))
                targetArray[i] = mergedItem;
        }
    }

    private static JsonNode? MergeArrayItem(JsonNode? targetItem, JsonNode? sourceItem)
    {
        switch (sourceItem)
        {
            case JsonObject when targetItem is JsonObject:
                ReplaceObjectProperties(targetItem, sourceItem);
                return targetItem;

            case JsonArray when targetItem is JsonArray:
                ReplaceArrayProperties(targetItem, sourceItem);
                return targetItem;

            default:
                // Scalars, nulls, and items of a different kind than the target's replace the target item outright.
                return sourceItem?.DeepClone();
        }
    }

    private static void ReplaceProperty(JsonNode target, JsonNode source, string property)
    {
        JsonNode? targetProperty = target[property];

        if (targetProperty == null) 
            target[property] = targetProperty = new JsonObject();
        
        JsonNode? sourceProperty = source[property];
        object? sourcePropertyValue = sourceProperty?.GetValueKind() is JsonValueKind.Object or JsonValueKind.Array
            ? sourceProperty.DeepClone()
            // The underlying value type of the property is preserved when using GetValue like this.
            : sourceProperty?.GetValue<object>();

        targetProperty.ReplaceWith(sourcePropertyValue);
    }
}
