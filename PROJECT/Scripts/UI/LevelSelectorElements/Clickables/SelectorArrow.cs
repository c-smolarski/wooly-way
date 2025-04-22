using Com.IsartDigital.WoolyWay.Assets.SingleAssets;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.Menus;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables
{
    public partial class SelectorArrow : ClickableLevelSelectorElement
    {
        [Export] private LevelButton level;
        [Export] private bool next;
        [Export] private Color hoverColor = new Color(1.0f, 1.0f, 0.812f, 1.0f);
        [Export] private float animDuration = 0.25f;
        [Export] private LevelSelectorPathAsset block;

        private const string POLYGON_CONTAINER_PATH = "arrowPolygons";
        private const string POLYGON_PATH = POLYGON_CONTAINER_PATH + "/polygon";
        private const string SHADOW_PATH = POLYGON_CONTAINER_PATH + "/shadow";

        private Polygon2D polygon, shadow;
        private Node2D polygonContainer;
        private Vector2 blockBaseScale, polygonsBaseScale;
        private Color defaultColor;
        private bool arrowVisible = false;

        public override void _Ready()
        {
            base._Ready();
            MouseDetector.MouseEntered += OnHovered;
            MouseDetector.MouseExited += OnUnhovered;
            block.AppearAnimFinished += OnFirstAppear;

            NodeVariablesInit();
            BaseValuesInit();
        }

        protected override void OnClick()
        {
            if (LevelManager.CurrentLevel != null)
                return;

            base.OnClick();
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.MoveToLevelButton, level.LevelNumber, next);
        }

        protected override void OnLevelStart(Grid pGrid)
        {
            ChangeVisibility(false);
        }

        private void NodeVariablesInit()
        {
            polygon = GetNode<Polygon2D>(POLYGON_PATH);
            shadow = GetNode<Polygon2D>(SHADOW_PATH);
            polygonContainer = GetNode<Node2D>(POLYGON_CONTAINER_PATH);
        }

        private void BaseValuesInit()
        {
            defaultColor = polygon.Color;
            blockBaseScale = block.Scale;
            polygonsBaseScale = polygonContainer.Scale;
            polygonContainer.Scale = Vector2.Zero;
            MouseDetector.SetActive(false);
        }

        private void OnHovered()
        {
            polygon.Color = hoverColor;
        }

        private void OnUnhovered()
        {
            polygon.Color = defaultColor;
        }

        private void OnFirstAppear()
        {
            if (level.LevelNumber == LevelButton.START_LEVEL_NUMBER + 1 && next || LevelSelector.SelectedLevelNumber == level.LevelNumber - (next ? 1 : 0))
                ChangeVisibility(true);
            SignalBus.Instance.MoveToLevelButton += OnDeparted;
            SignalBus.Instance.ArrivedAtLevelButton += OnArrived;
            block.AppearAnimFinished -= OnFirstAppear;
        }

        private void OnDeparted(int pCurrentLevelNumber, bool pToNext)
        {
            ChangeVisibility(false);
        }

        private void OnArrived(int pLevelNumber)
        {
            ChangeVisibility(level.LevelNumber == pLevelNumber + (next ? 1 : 0));
        }

        protected void ChangeVisibility(bool pVisible)
        {
            if (arrowVisible != pVisible)
            {
                arrowVisible = pVisible;
                MouseDetector.SetActive(pVisible);
                if (pVisible)
                    PlayAppearAnim();
                else
                    PlayDisappearAnim();
            }
        }

        private void PlayAppearAnim()
        {
            Tween lTween = CreateTween();
            lTween.SetParallel();
            lTween.TweenProperty(block, TweenProp.SCALE, Vector2.Zero, animDuration)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.Out);
            lTween.TweenProperty(polygonContainer, TweenProp.SCALE, polygonsBaseScale, animDuration)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.In);
        }

        private void PlayDisappearAnim()
        {
            Tween lTween = CreateTween();
            lTween.SetParallel();
            lTween.TweenProperty(block, TweenProp.SCALE, blockBaseScale, animDuration)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.In);
            lTween.TweenProperty(polygonContainer, TweenProp.SCALE, Vector2.Zero, animDuration)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.Out);
        }

        protected override void Dispose(bool disposing)
        {
            if (IsInstanceValid(MouseDetector))
            {
                MouseDetector.MouseEntered -= OnHovered;
                MouseDetector.MouseExited -= OnUnhovered;
            }
            if (IsInstanceValid(block))
                block.AppearAnimFinished -= OnFirstAppear;
            SignalBus.Instance.MoveToLevelButton -= OnDeparted;
            SignalBus.Instance.ArrivedAtLevelButton -= OnArrived;
            base.Dispose(disposing);
        }
    }
}
