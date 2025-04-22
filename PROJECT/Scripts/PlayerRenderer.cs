using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.Scripts
{
    public partial class PlayerRenderer : AnimatedSprite2D
    {
        [Export] private GpuParticles2D grassParticles;
        [Export] private GpuParticles2D snowParticles;

        private GpuParticles2D particles;

        public override void _Ready()
        {
            base._Ready();
            particles = grassParticles;
            AnimationChanged += OnAnimChanged;
        }

        private void OnAnimChanged()
        {
            grassParticles.Emitting = snowParticles.Emitting = false;
            particles = EnvironmentManager.CurrentWeather == EnvironmentManager.Weather.SNOW ? snowParticles : grassParticles;

            particles.Emitting = Animation.ToString().Contains(StringDirection.WALK);
            particles.ProcessMaterial.Set("direction", new Vector2(FlipH ? -0.5f : 0.5f, -1));
        }
    }
}
