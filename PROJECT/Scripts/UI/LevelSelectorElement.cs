using Godot;
using System;

// Author : Camille Smolarski

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


        /// <summary>
        /// Updates a ScaleModifier variable as LevelSelectorElements might have their own Scale.
        /// This modifier must be applied during the operation if you want the animation to have an impact.
        /// </summary>
        /// <param name="pVisible"></param>
        protected virtual void ChangeVisibilty(bool pVisible)
        {
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, nameof(ScaleModifier), pVisible ? baseScale : Vector2.Zero, AppearAnimDuration)
                .From(pVisible ? Vector2.Zero : baseScale);
        }
    }
}
