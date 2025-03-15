using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.Scripts.Assets
{
    public partial class LevelSelectorPath : Asset
    {
        protected override float AnimStepDuration => 0.5f;

        private Vector2 baseScale;

        public override void _Ready()
        {
            base._Ready();
            baseScale = Scale;
            Visible = false;
        }

        public override void PlayAppearAnim()
        {
            Visible = true;
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.SCALE, Scale, AnimStepDuration)
                .From(Vector2.Zero)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.Out);
        }

        public override void PlayDisappearAnim()
        {
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.SCALE, Vector2.Zero, AnimStepDuration)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.In);
            lTween.Finished += ResetValues;

        }

        private void ResetValues()
        {
            Scale = baseScale;
            Visible = false;
        }
    }
}
