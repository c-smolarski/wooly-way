using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI
{
    public abstract partial class ClickableArea : Area2D
    {
        [Signal] public delegate void ClickedEventHandler();
        [Signal] public delegate void ScaleModifierChangedEventHandler();

        [Export] protected float AppearAnimDuration { get; private set; } = 0.5f;

        private const string COLLIDER_PATH = "collider";

        protected bool IsHovered { get; private set; }
        protected CollisionShape2D Collider { get; private set; }
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
            Collider = GetNode<CollisionShape2D>(COLLIDER_PATH);
            MouseEntered += OnMouseEnter;
            MouseExited += OnMouseExit;
            baseScale = Scale;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            if (IsHovered && Input.IsActionJustPressed(InputManager.UI_CLICK))
                EmitSignal(SignalName.Clicked);
        }

        private void OnMouseEnter()
        {
            IsHovered = true;
        }

        private void OnMouseExit()
        {
            IsHovered = false;
        }

        protected virtual void ChangeVisibilty(bool pVisible)
        {
            if (IsHovered && !pVisible)
                OnMouseExit();
            Collider.SetDeferred(nameof(Collider.Disabled), !pVisible);

            Tween lTween = CreateTween();
            lTween.TweenProperty(this, nameof(ScaleModifier), pVisible ? baseScale : Vector2.Zero, AppearAnimDuration)
                .From(pVisible ? Vector2.Zero : baseScale);
        }
    }
}
