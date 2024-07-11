using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.Routines;

public sealed class MoveTo : Component
{
    private readonly Vector2 _target;

    private float _speed;

    public MoveTo(Vector2 target)
    {
        _target = target;
    }

    public override void Update(IPositionalEntity entity, GameUpdateTime time)
    {
        Require.NotNull(entity, nameof(entity));

        _speed = Move.Approach(_speed, entity.MaxSpeed, time);

        entity.Velocity = Move.Approach(entity.Position, _target, entity.MaxSpeed, time);
    }
}
