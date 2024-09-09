using BadEcho.Extensions;

namespace BadEcho.Game.AI;

/// <summary>
/// Provides a model for a possible state of a finite-state machine.
/// </summary>
/// <typeparam name="T">Type used as the descriptor of the state.</typeparam>
/// <remarks>
/// A model type for states exists so that the same finite-state machine configuration can be used to generate
/// unique <see cref="StateMachine{T}"/> instances containing separate stateful data.
/// </remarks>
internal sealed class StateModel<T>
{
    /// <summary>
    /// Gets a descriptor that identifies the state.
    /// </summary>
    public T? Identifier
    { get; init; }

    /// <summary>
    /// Gets the set of states the state can transition to.
    /// </summary>
    public HashSet<TransitionModel<T>> Transitions
    { get; } = [];

    /// <summary>
    /// Gets a collection of methods executed by the state when activated.
    /// </summary>
    public ICollection<Action<T>> EnterActions
    { get; } = [];

    /// <summary>
    /// Gets a collection of methods executed by the state when deactivated.
    /// </summary>
    public ICollection<Action<T>> ExitActions
    { get; } = [];

    /// <summary>
    /// Gets a collection of methods executed if the state is active during the finite-state machine's update pass.
    /// </summary>
    public ICollection<Action<GameUpdateTime>> UpdateActions
    { get; } = [];

    /// <summary>
    /// Gets a collection of factories used to create the components executed by the state when active.
    /// </summary>
    public ICollection<Func<Component>> ComponentFactories
    { get; } = [];

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is StateModel<T> otherModel && otherModel.Identifier.Equals<T>(Identifier);
    
    /// <inheritdoc/>
    public override int GetHashCode() 
        => this.GetHashCode(Identifier);
}