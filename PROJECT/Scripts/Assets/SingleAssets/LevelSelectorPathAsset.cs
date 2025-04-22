using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Scripts.Assets;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.Assets.SingleAssets
{
    public partial class LevelSelectorPathAsset : SingleAsset
    {
        [Signal] public delegate void AppearAnimFinishedEventHandler();

        private const float SOUND_DURATION = 0.1f;
        protected override float AnimStepDuration => 0.5f;

        public override void PlayAppearAnim()
        {
            base.PlayAppearAnim();
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.SCALE, Scale, AnimStepDuration)
                .From(Vector2.Zero)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.Out);
            lTween.Finished += () => EmitSignal(SignalName.AppearAnimFinished);

            SoundManager.GetInstance().PlaySFX(SoundManager.GetInstance().PopSFX, SOUND_DURATION);
        }

        public override void PlayDisappearAnim()
        {
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.SCALE, Vector2.Zero, AnimStepDuration)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.In);
            lTween.Finished += ResetValues;

        }
    }
}
