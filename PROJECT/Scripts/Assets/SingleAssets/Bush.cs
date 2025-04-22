using Com.IsartDigital.WoolyWay.Scripts.Assets;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.Assets.SingleAssets
{
    public partial class Bush : SingleAsset
    {
        protected override float AnimStepDuration => 0.5f;
        protected bool isSpawned = default;

        public override void PlayAppearAnim()
        {
            base.PlayAppearAnim();
            Tween tweenAppear = CreateTween(); ;
            tweenAppear.TweenProperty(this, TweenProp.SCALE, Scale, AnimStepDuration)
                .From(Vector2.Zero)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Bounce);
            tweenAppear.Finished += () => isSpawned = true;
        }

        public override void PlayDisappearAnim()
        {
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.SCALE, Vector2.Zero, AnimStepDuration)
                    .SetEase(Tween.EaseType.In)
                    .SetTrans(Tween.TransitionType.Bounce);
            lTween.Finished += ResetValues;
        }
    }
}
