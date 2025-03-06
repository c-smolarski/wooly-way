using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI
{
    public abstract partial class LevelSelectorElement : Node2D
    {
        [Signal] public delegate void ScaleModifierChangedEventHandler();

        [Export] protected float AppearAnimDuration { get; private set; } = 0.5f;

        protected Vector2 ScaleModifier
        {
            get => backingScaleModifier;
            private set
            {
                backingScaleModifier = value;
                EmitSignal(SignalName.ScaleModifierChanged);
            }
        }

        private Vector2 backingScaleModifier;
        private Vector2 baseScale;

        public override void _Ready()
        {
            base._Ready();
            baseScale = Scale;
        }

        protected virtual void ChangeVisibilty(bool pVisible)
        {
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, nameof(ScaleModifier), pVisible ? baseScale : Vector2.Zero, AppearAnimDuration)
                .From(pVisible ? Vector2.Zero : baseScale);
        }
    }
}
