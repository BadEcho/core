//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensions;

namespace BadEcho.Fenestra.Messaging;

/// <summary>
/// Provides a message that can be sent through a mediator to anything listening for it.
/// </summary>
public sealed class MediatorMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatorMessage"/> class.
    /// </summary>
    /// <param name="name">The identifying name of the message.</param>
    /// <param name="callbackType">The type of callback supported by the message.</param>
    public MediatorMessage(string name, Type? callbackType)
    {
        Require.NotNullOrEmpty(name, nameof(name));

        Name = name;
        CallbackType = callbackType;
    }

    /// <summary>
    /// Gets the type of callback supported by the message.
    /// </summary>
    public Type? CallbackType
    { get; }

    /// <summary>
    /// Gets the identifying name of the message.
    /// </summary>
    public string Name
    { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not MediatorMessage otherMessage)
            return false;

        return CallbackType == otherMessage.CallbackType
            && Name == otherMessage.Name;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => this.GetHashCode(CallbackType, Name);
}
