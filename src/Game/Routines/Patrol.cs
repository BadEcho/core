using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadEcho.Game.AI;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.Routines;

public sealed class Patrol : Component
{
    public Patrol(IEnumerable<Vector2> targets)
    {
        Require.NotNull(targets, nameof(targets));

        int targetIndex = 0;
        var builder = new StateMachineBuilder<int>();

        foreach (Vector2 target in targets)
        {
            
        }
    }

    public override bool Update(IEntity entity, GameUpdateTime time)
    {
        throw new NotImplementedException();
    }
}
