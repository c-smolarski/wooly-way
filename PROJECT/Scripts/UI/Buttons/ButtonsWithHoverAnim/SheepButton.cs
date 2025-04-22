using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim
{
    public partial class SheepButton : ButtonWithHoverAnim
    {
        private const float BLEAT_ANIM_DURATION = 1f;
        private const float SCALE_MULT = 1.1f;
        private const int NUM_SHAKE = 15;

        private bool bleating;

        protected override void OnPressed()
        {
            base.OnPressed();
            BleatAnim();
        }

        private void BleatAnim()
        {
            bleating = true;
            Tween lTween = CreateTween();
            for (int i = 0; i < NUM_SHAKE; i++)
                lTween.TweenProperty(this, TweenProp.SCALE, BaseScale * SCALE_MULT, BLEAT_ANIM_DURATION / NUM_SHAKE)
                    .From(BaseScale);
            lTween.TweenProperty(this, TweenProp.SCALE, BaseScale, 0f);
            lTween.Finished += () => bleating = false;
        }

        protected override void OnUnhovered()
        {
            if(!bleating)
                base.OnUnhovered();
        }
    }
}
