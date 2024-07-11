using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadEcho.Logging;
using Microsoft.Xna.Framework;

namespace BadEcho.Game;

public static class Move
{
    public static float Approach(float speed, float maxSpeed, GameUpdateTime time)
    {
        float change = (float) (160f * time.ElapsedGameTime.TotalSeconds);

        return speed <= maxSpeed ? Math.Min(speed + change, maxSpeed) : Math.Max(speed - change, maxSpeed);
    }

    public static Vector2 Approach(Vector2 position, Vector2 target, float maxChange, GameUpdateTime time)
    {
        if (maxChange == 0f || position == target)
            return Vector2.Zero;

        Vector2 difference = target - position;
        Vector2 velocity;
        
        float differenceLength = difference.Length();
        //if (differenceLength < maxChange)
        //    velocity = difference;
        //else
        //{
            difference.Normalize();
            velocity = difference * maxChange;
        //}

        float timeScale 
            = (float) (time.ElapsedGameTime.TotalMilliseconds / time.TargetElapsedTime.TotalMilliseconds);

        float scaledVelocityLength = (velocity * timeScale).Length();

        if (scaledVelocityLength > differenceLength)
            velocity *= (differenceLength / scaledVelocityLength);

        Logger.Debug($"Position: {position} Velocity: {velocity}");

        return velocity;
    }
}
 