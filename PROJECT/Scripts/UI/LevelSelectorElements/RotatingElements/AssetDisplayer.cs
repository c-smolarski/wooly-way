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
        }

        private void OnFocusChanged(bool pFocused)
        {
            if(pFocused)
                foreach(Asset lAsset in assets)
                {
                    GetTree()
                        .CreateTimer(0.25f * (Array.IndexOf(assets, lAsset) + 1))
                        .Timeout += lAsset.PlayAppearAnim;
                }
            else
                foreach(Asset lAsset in assets)
                    lAsset.PlayDisappearAnim();
        }
    }
}
