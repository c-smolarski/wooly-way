using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI
{
    public abstract partial class ClickableArea : Area2D
    {
        private const string COLLIDER_PATH = "collider";

        protected bool IsHovered { get; private set; }
        protected CollisionShape2D Collider { get; private set; }

        public override void _Ready()
        {
            base._Ready();
            Collider = GetNode<CollisionShape2D>(COLLIDER_PATH);
            MouseEntered += OnMouseEnter;
            MouseExited += OnMouseExit;
        }

        private void OnMouseEnter()
        {
            IsHovered = true;
        }

        private void OnMouseExit()
        {
            IsHovered = false;
        }
    }
}
