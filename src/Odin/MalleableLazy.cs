//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace BadEcho.Odin;

/// <summary>
/// Provides support for lazy initialization with malleable output.
/// </summary>
/// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
public sealed class MalleableLazy<T> : Lazy<T>
{
    private T? _overridingValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy(bool isThreadSafe)
        : base(isThreadSafe)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy(LazyThreadSafetyMode mode)
        : base(mode)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy(Func<T> valueFactory)
        : base(valueFactory)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy(T value)
        : base(value)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy(Func<T> valueFactory, bool isThreadSafe)
        : base(valueFactory, isThreadSafe)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MalleableLazy{T}"/> class.
    /// </summary>
    public MalleableLazy(Func<T> valueFactory, LazyThreadSafetyMode mode)
        : base(valueFactory, mode)
    { }

    /// <summary>
    /// Gets a value that indicates whether a value has been created for this <see cref="MalleableLazy{T}"/> instance,
    /// either through lazy initialization or overriding.
    /// </summary>
    public new bool IsValueCreated
        => IsValueOverridden || base.IsValueCreated;

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
    public new T Value
    {
        get => IsValueOverridden ? _overridingValue : base.Value;
        set => _overridingValue = value;
    }
}