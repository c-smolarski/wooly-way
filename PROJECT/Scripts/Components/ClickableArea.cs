using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.Components
{
    public partial class ClickableArea: Area2D
    {

        [Signal] public delegate void ClickedEventHandler();

        private const string COLLIDER_PATH = "collider";

        public bool IsHovered { get; private set; }
        public CollisionShape2D Collider { get; private set; }

        public override void _Ready()
        {
            base._Ready();
            Collider = GetNode<CollisionShape2D>(COLLIDER_PATH);
            MouseEntered += SetHovered;
            MouseExited += SetUnhovered;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            if (IsHovered && Input.IsActionJustPressed(InputManager.UI_CLICK))
                EmitSignal(SignalName.Clicked);
        }

        private void SetHovered()
        {
            IsHovered = true;
        }

        public void SetUnhovered()
        {
            IsHovered = false;
        }
    }
}
