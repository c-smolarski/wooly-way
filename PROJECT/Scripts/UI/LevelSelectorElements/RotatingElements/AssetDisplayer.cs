using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.RotatingElements
{
    public partial class AssetDisplayer : RotatingElement
    {
        [Export] private LevelButtonDisplayer buttonDisplayer;
        [Export] private Asset[] assets;

        public override void _Ready()
        {
            DisplayFrame = buttonDisplayer.DisplayFrame;
            base._Ready();
            buttonDisplayer.Focused += OnFocusChanged;
            buttonDisplayer.Mountain.Ready += () => ChangeVisibilty(true);
            HideAllAssets();
        }

        private void OnFocusChanged(bool pFocused)
        {
            if (pFocused)
                Appear();
            else
                Disappear();
        }

        private void Appear()
        {
            Modulate = Colors.White;
            foreach (Asset lAsset in assets)
            {
                GetTree()
                    .CreateTimer(0.1f * (Array.IndexOf(assets, lAsset) + 1))
                    .Timeout += lAsset.PlayAppearAnim;
            }
        }

        private void Disappear()
        {
            foreach (Asset lAsset in assets)
                lAsset.PlayDisappearAnim();
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.MODULATE_ALPHA, 0f, AppearAnimDuration / 2);
        }

        private void HideAllAssets()
        {
            foreach (Asset lAsset in assets)
                lAsset.Visible = false;
        }
    }
}
