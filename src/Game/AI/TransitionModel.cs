using BadEcho.Extensions;

namespace BadEcho.Game.AI;

internal sealed class TransitionModel<T>
{
    public T? Target
    { get; init; }

    public ICollection<Func<bool>> Conditions
    { get; } = [];

    public TimeSpan Duration { get; set; }

    public override bool Equals(object? obj)
        => obj is TransitionModel<T> otherModel && otherModel.Target.Equals<T>(Target);

    public override int GetHashCode()
        => this.GetHashCode(Target);
}