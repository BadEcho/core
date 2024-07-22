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
    private readonly float _maxSpeed;

    private float _speed;

    public MoveTo(Vector2 target, float maxSpeed)
    {
        _target = target;
        _maxSpeed = maxSpeed;
    }

    public override void Update(IEntity entity, GameUpdateTime time)
    {
        Require.NotNull(entity, nameof(entity));

        _speed = Move.Approach(_speed, _maxSpeed, time);

        entity.Velocity = Move.Approach(entity.Position, _target, _maxSpeed, time);
    }
}
