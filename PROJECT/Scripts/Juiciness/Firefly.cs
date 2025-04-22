using Godot;
using System;

// Author : Daniel Moussouni--Lepilliez

namespace Com.IsartDigital.WoolyWay.Juiciness
{

    public partial class Firefly : Node2D
    {
        private Vector2 velocity;
        private float speed = 20f;
        private float wanderStrength = 50f;
        private float flickerTime = 0;
        private float flickerDuration = 0.5f;
        private Light2D glow;

        public override void _Ready()
        {
            velocity = new Vector2(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1).Normalized() * speed;
        }

        public override void _Process(double delta)
        {
            float lDelta = (float)delta;
            Wander(lDelta);
        }

        private void Wander(float delta)
        {
            // Random direction change over time
            Vector2 randomDirection = new Vector2(
                (GD.Randf() * 2 - 1) * wanderStrength,
                (GD.Randf() * 2 - 1) * wanderStrength
            );
            velocity = (velocity + randomDirection * delta).Normalized() * speed;

            Position += velocity * delta;
        }

        private void FlickerEffect(float delta)
        {
            flickerTime += delta;
            if (flickerTime > flickerDuration)
            {
                flickerTime = 0;
                flickerDuration = GD.Randf() * 1.5f; // Random flicker timing
                glow.Energy = GD.Randf() * 0.5f + 0.5f; // Random light intensity
            }
        }

    }
}
