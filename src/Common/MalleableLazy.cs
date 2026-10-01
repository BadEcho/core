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

using System.Diagnostics.CodeAnalysis;

namespace BadEcho;

/// <summary>
/// Provides support for lazy initialization with malleable output.
/// </summary>
/// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
public sealed class MalleableLazy<T>
{
    private readonly Lazy<T> _inner;

    // Because T is unconstrainted, T? on a value type is just T. So, to be able to support overriding value types and
    // null reference types, we need to make use of a "box", if you will.
    // We're using the volatile keyword here because we're acting as a Lazy<T>, and Lazy<T> is a thread-safety type.
    // So, special consideration is warranted in regard to thread synchronization on the fields in this class, as they
    // are expected to be written to by multiple threads, and all without a lock involved.
    private volatile Box? _overridingValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy()
        => _inner = new Lazy<T>();

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy(bool isThreadSafe)
        => _inner = new Lazy<T>(isThreadSafe);

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy(LazyThreadSafetyMode mode)
        => _inner = new Lazy<T>(mode);

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy(Func<T> valueFactory)
        => _inner = new Lazy<T>(valueFactory);

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy(T value)
        => _inner = new Lazy<T>(value);

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy(Func<T> valueFactory, bool isThreadSafe)
        => _inner = new Lazy<T>(valueFactory, isThreadSafe);

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy(Func<T> valueFactory, LazyThreadSafetyMode mode)
        => _inner = new Lazy<T>(valueFactory, mode);


    /// <summary>
    /// Gets a value that indicates whether a value has been created for this <see cref="MalleableLazy{T}"/> instance,
    /// either through lazy initialization or overriding.
    /// </summary>
    public bool IsValueCreated
        => IsValueOverridden || _inner.IsValueCreated;

    /// <summary>
    /// Gets a value that indicates whether a value for the <see cref="MalleableLazy{T}"/> instance has been manually
    /// overridden.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_overridingValue))]
    private bool IsValueOverridden
        => _overridingValue != null;

    /// <summary>
    /// Gets either the lazily initialized or overriden value, and sets the overriden value.
    /// </summary>
    public T Value
    {
        // This ensures the value is only read once; another thread could update it between reads if we used a more conventional null check here.
        get => _overridingValue is { } overridingValue ? overridingValue.Value : _inner.Value;
        set => _overridingValue = new Box(value);
    }

    /// <summary>
    /// Provides a value in a box.
    /// </summary>
    /// <param name="value">The value to stuff in the box.</param>
    private sealed class Box(T value)
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value
        { get; } = value;
    }
}