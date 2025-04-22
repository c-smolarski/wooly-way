using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI.Buttons
{
    public partial class ButtonWithHoverAnim : AbstractButton
    {
        [Export] private float hoverScaleMult = 1.1f;
        [Export] private float hoverScaleTime = 0.2f;

        protected Vector2 BaseScale { get; private set; }

        public override void _Ready()
        {
            BaseScale = Scale;
            base._Ready();
            MouseEntered += OnHovered;
            MouseExited += OnUnhovered;
        }

        private void OnHovered()
        {
            if (!Visible || Disabled)
                return;

            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.SCALE, BaseScale * hoverScaleMult, hoverScaleTime)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.Out);
        }

        protected virtual void OnUnhovered()
        {
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.SCALE, BaseScale, hoverScaleTime);
        }

        public override void Disable()
        {
            base.Disable();
            OnUnhovered();
        }
    }
}
