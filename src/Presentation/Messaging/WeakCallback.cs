//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Reflection;
using BadEcho.Extensions;

namespace BadEcho.Presentation.Messaging;

/// <summary>
/// Provides support for weakly referenced callbacks for use in messaging.
/// </summary>
internal sealed class WeakCallback
{
    private readonly Type _callbackType;
    private readonly MethodInfo _method;
    private readonly WeakReference _weakMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakCallback"/> class.
    /// </summary>
    /// <param name="callback">The delegate to weakly reference.</param>
    public WeakCallback(Delegate callback)
    {
        _callbackType = callback.GetType();
        _method = callback.Method;
        _weakMessage = new WeakReference(callback.Target);
    }

    /// <summary>
    /// Gets an indication whether the target of the callback has been garbage collected.
    /// </summary>
    public bool IsAlive
        => _weakMessage.IsAlive;

    /// <summary>
    /// Gets the callback being weakly referenced.
    /// </summary>
    /// <remarks>
    /// <para>
    /// We can't simply store a weak reference to a provided delegate, as that would tie us to the lifetime of the
    /// delegate itself, which more than likely will have been declared inline in the form of a lambda expression.
    /// As soon as it goes out of scope, it will be garbage collected a short time after since nothing else is pinning
    /// it.
    /// </para>
    /// <para>
    /// Instead, we tie this object to the lifetime of the delegate's target itself. We do this by taking the delegate's
    /// constituent parts and then using them, along with a stored weakly reference to the target, to create a functionally
    /// identical delegate upon demand.
    /// </para>
    /// </remarks>
    public MulticastDelegate Target
        => (MulticastDelegate) Delegate.CreateDelegate(_callbackType, _weakMessage.Target, _method);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not WeakCallback other)
            return false;

        return _callbackType == other._callbackType
            && _method == other._method
            && _weakMessage.Target == other._weakMessage.Target;
    }

    /// <inheritdoc/>
    public override int GetHashCode() 
        => this.GetHashCode(_callbackType, _method, _weakMessage.Target);
}
