using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.Scripts.UI.LevelSelectorElements
{
    public partial class AssetDisplayer : Node2D
    {
        [Export] private LevelButton associatedLevel;
        [Export(PropertyHint.Range, "0f, 60f, 0.1f, or_greater")] private float secondsBetweenAssets = 0.1f;
        
        public override void _Ready()
        {
            base._Ready();

            HideAllAssets(); //TODO : if level is not already unlocked

            SignalBus.Instance.UnlockLevel += OnLevelUnlocked;
        }

        private void OnLevelUnlocked(int pLevelNumber)
        {
            if (associatedLevel.LevelNumber <= pLevelNumber)
                Appear();
        }

        private void Appear()
        {
            Godot.Collections.Array<Node> lAssetArray = GetChildren();
            Modulate = Colors.White;
            
            foreach (Asset lAsset in lAssetArray)
            {
                GetTree()
                    .CreateTimer(secondsBetweenAssets * lAssetArray.IndexOf(lAsset))
                    .Timeout += lAsset.PlayAppearAnim;
            }
        }

        private void Disappear()
        {
            foreach (Asset lAsset in GetChildren())
                lAsset.PlayDisappearAnim();
        }

        private void HideAllAssets()
        {
            foreach (Asset lAsset in GetChildren())
                lAsset.Visible = false;
        }

        protected override void Dispose(bool disposing)
        {
            SignalBus.Instance.UnlockLevel -= OnLevelUnlocked;
            base.Dispose(disposing);
        }
    }
}
