using Com.IsartDigital.WoolyWay.Components;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements
{
    public abstract partial class ClickableLevelSelectorElement : LevelSelectorElement
    {
        protected ClickableArea MouseDetector { get; private set; }

        private const string AREA_PATH = "mouseDetector";

        public override void _Ready()
        {
            base._Ready();
            MouseDetector = GetNode<ClickableArea>(AREA_PATH);
            MouseDetector.Clicked += OnClick;
        }

        protected virtual void OnClick(){ }

        protected override void ChangeVisibilty(bool pVisible)
        {
            if (MouseDetector.IsHovered && !pVisible)
                MouseDetector.SetUnhovered();
            MouseDetector.Collider.SetDeferred(nameof(MouseDetector.Collider.Disabled), !pVisible);
            base.ChangeVisibilty(pVisible);
        }
    }
}
