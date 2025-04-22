using Com.IsartDigital.WoolyWay.Juiciness;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.Menus;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.Assets
{
    public partial class LevelBackground : Asset
    {
        [Export] private Node2D[] assetsContainers;
        [Export] private Grass[] grassList;

        private const int ZINDEX_LAYER = 50;

        protected override float AnimStepDuration => 0.1f;

        public override void _Ready()
        {
            base._Ready();
            if (grassList?.Length == 0)
                grassList = null;

            GetViewport().SizeChanged += OnWindowSizeChanged;
            SignalBus.Instance.FullscreenButtonPressed += OnWindowSizeChanged;

            if (assetsContainers != null)
                foreach (Node2D lContainer in assetsContainers)
                {
                    lContainer.ZIndex += -GetParent<Node2D>().ZIndex + GameManager.Instance.GameContainer.ZIndex + ZINDEX_LAYER;
                    foreach (Asset lAsset in lContainer.GetChildren())
                        lAsset.Visible = false;
                }

            if (grassList != null)
                foreach (Grass lGrassSystem in grassList)
                {
                    lGrassSystem.ShaderAlpha = 0f;
                    lGrassSystem.Visible = false;
                }
        }

        private void OnWindowSizeChanged()
        {
            GetViewport().GetCamera2D().GlobalPosition = GlobalPosition;
        }

        public override void PlayAppearAnim()
        {
            if (assetsContainers != null)
                foreach (Node2D lContainer in assetsContainers)
                    for (int i = 0; i < lContainer.GetChildCount(); i++)
                        GetTree().CreateTimer(AnimStepDuration * i, false).Timeout +=
                            ((Asset)lContainer.GetChild(i)).PlayAppearAnim;

            if (grassList == null)
                return;

            Tween lTween = CreateTween();
            lTween.SetParallel();
            foreach (Grass lGrassSystem in grassList)
            {
                lGrassSystem.Visible = true;
                lTween.TweenProperty(lGrassSystem, nameof(lGrassSystem.ShaderAlpha), 1f, LevelSelector.TRANSITION_TIME)
                    .From(0f);
            }
        }

        public override void PlayDisappearAnim()
        {
            if (assetsContainers == null)
                return;

            foreach (Node2D lContainer in assetsContainers)
                foreach (Asset lAsset in lContainer.GetChildren())
                    lAsset.PlayDisappearAnim();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsInstanceValid(GetViewport()))
                GetViewport().SizeChanged -= OnWindowSizeChanged;
            SignalBus.Instance.FullscreenButtonPressed -= OnWindowSizeChanged;
            base.Dispose(disposing);
        }
    }
}
