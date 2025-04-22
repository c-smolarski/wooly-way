using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.Assets.SingleAssets
{
    public partial class Lantern : LightableAsset
    {
        private const float CLICK_ROT_NUM = 5f;
        private const float CLICK_ROT_DEG = 10f;
        private const float CLICK_ROT_TIME = 0.25f;

        protected override void OnClick()
        {
            Tween lTween = CreateTween();
            for (int i = 0; i < CLICK_ROT_NUM; i++)
                lTween.TweenProperty(this, TweenProp.ROTATION, Mathf.DegToRad((i % 2 == 0 ? 1 : -1) * CLICK_ROT_DEG), CLICK_ROT_TIME / CLICK_ROT_NUM);
            lTween.TweenProperty(this, TweenProp.ROTATION, 0f, CLICK_ROT_TIME / CLICK_ROT_NUM);
            base.OnClick();
        }
    }
}
