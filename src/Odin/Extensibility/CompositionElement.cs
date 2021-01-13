//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides an element that participates in composition.
    /// </summary>
    /// <remarks>
    /// This class seeks to partially mimic the internal class of the same name found in the Managed Extensibility
    /// Framework, thus special consideration is required as .NET itself is updated.
    /// </remarks>
    [Serializable]
    [DebuggerTypeProxy(typeof(CompositionElementDebuggerProxy))]
    internal sealed class CompositionElement : ICompositionElement
    {
        private static readonly ICompositionElement _UnknownOrigin
            = new CompositionElement(Strings.CompositionUnknownOrigin, null);

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionElement"/> class.
        /// </summary>
        /// <param name="underlyingDefinition">The underlying definition of this composition element.</param>
        public CompositionElement(ExportDefinition underlyingDefinition)
            : this(underlyingDefinition.ContractName, _UnknownOrigin)
        {
            UnderlyingDefinition = underlyingDefinition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionElement"/> class.
        /// </summary>
        /// <param name="displayName">The human-readable display name of the composition element.</param>
        /// <param name="origin">
        /// The composition element from which the new composition element originated, or null if the new composition
        /// element is the root.
        /// </param>
        private CompositionElement(string? displayName, ICompositionElement? origin)
        {
            DisplayName = displayName ?? string.Empty;
            Origin = origin;
        }

        /// <inheritdoc/>
        public string DisplayName 
        { get; }

        /// <inheritdoc/>
        public ICompositionElement? Origin 
        { get; }

        /// <summary>
        /// Gets the underlying definition of this composition element.
        /// </summary>
        public ExportDefinition? UnderlyingDefinition
        { get; }

        /// <inheritdoc/>
        public override string ToString() 
            => DisplayName;

        /// <summary>
        /// Provides a display proxy for the <see cref="CompositionElement"/> type.
        /// </summary>
        private sealed class CompositionElementDebuggerProxy
        {
            private readonly CompositionElement _element;

            /// <summary>
            /// Initializes a new instance of the <see cref="CompositionElementDebuggerProxy"/> class.
            /// </summary>
            /// <param name="element">The element to display.</param>
            public CompositionElementDebuggerProxy(CompositionElement element)
            {
                _element = element;
            }

            /// <inheritdoc cref="CompositionElement.DisplayName"/>
            public string DisplayName
                => _element.DisplayName;

            /// <inheritdoc cref="CompositionElement.Origin"/>
            public ICompositionElement? Origin
                => _element.Origin;

            /// <inheritdoc cref="CompositionElement.UnderlyingDefinition"/>
            public ExportDefinition? UnderlyingDefinition
                => _element.UnderlyingDefinition;
        }
    }
}
