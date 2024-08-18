namespace BadEcho.Game.AI;

internal sealed class StateModel<T>
{
    public T? Identifier
    { get; init; }

    public ICollection<TransitionModel<T>> Transitions
    { get; } = [];

    public ICollection<Action<T>> EnterActions
    { get; } = [];

    public ICollection<Action<T>> ExitActions
    { get; } = [];

    public ICollection<Action<GameUpdateTime>> UpdateActions
    { get; } = [];
}