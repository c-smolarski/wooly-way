using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim
{
    public partial class AnimatedButton : ButtonWithHoverAnim
    {
        [Export] private float animationStartDelay;

        private const float BTN_ROT_DEG = 5f;
        private const float APPEAR_TIMER = 0.25f;
        private static int btnNumber;

        public override void _Ready()
        {
            base._Ready();
            SignalBus.Instance.AnyMenuButtonPressed += Disable;
            btnNumber++;
            StartAnim();
        }

        private void StartAnim()
        {
            float lDelay = (btnNumber - 1) * APPEAR_TIMER + animationStartDelay;
            Scale = Vector2.Zero;

            Tween lTween = CreateTween();
            lTween.SetParallel(true);
            lTween.TweenProperty(this, TweenProp.ROTATION, Rotation + (GD.Randf() < 0.5f ? 1 : -1) * GD.Randf() * Mathf.DegToRad(BTN_ROT_DEG), APPEAR_TIMER)
                .SetDelay(lDelay);
            lTween.TweenProperty(this, TweenProp.SCALE, BaseScale * 1.2f, APPEAR_TIMER)
                .SetDelay(lDelay);
            lTween.Chain().TweenProperty(this, TweenProp.SCALE, BaseScale, APPEAR_TIMER);

            GetTree().CreateTimer(lDelay + APPEAR_TIMER, false).Timeout += () => PlaySound(appearSFX);
        }

        protected override void Dispose(bool pDisposing)
        {
            btnNumber = 0;
            SignalBus.Instance.AnyMenuButtonPressed -= Disable;
            base.Dispose(pDisposing);
        }
    }
}
