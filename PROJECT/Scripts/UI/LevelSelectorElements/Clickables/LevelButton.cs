using Com.IsartDigital.WoolyWay.Assets;
using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.Menus;
using Com.IsartDigital.WoolyWay.Utils;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables
{
    public partial class LevelButton : ClickableLevelSelectorElement
    {
        [Export] public LevelSelector LevelSelector { get; private set; }
        [Export] public EnvironmentManager.Weather Weather { get; private set; }
        [Export] public PackedScene sceneBackground { get; private set; }
        [Export(PropertyHint.Range, "1, 100, 1")] public int LevelNumber { get; private set; } = 1;
        [Export] private Vector2 gridScale = Vector2.One * 0.5f;

        public const int START_LEVEL_NUMBER = 0;
        private const int SHAPE_POINT_NUMBER = 4;
        private const string AREA_MOVE_HERE_PATH = "levelMoveTo";
        private const string GRID_CONTAINER_PATH = "gridContainer";

        public Grid LevelGrid { get; private set; }

        private MapInfo levelInfo;
        private Node2D gridContainer;
        private ClickableArea areaMoveHere;
        private LevelBackground levelBackground;

        public override void _Ready()
        {
            base._Ready();
            levelInfo = LevelManager.GetLevel(LevelSelector?.WorldNumber ?? LevelSelector.DEFAULT_WORLD_NUMBER, LevelNumber);
            SignalBus.Instance.ArrivedAtLevelButton += OnButtonSelected;
            SignalBus.Instance.LevelClicked += OnLevelSelectorInput;
            SignalBus.Instance.MoveToLevelButton += OnMoved;
            AreasInit();
            GridInit();
        }

        private void AreasInit()
        {
            areaMoveHere = GetNode<ClickableArea>(AREA_MOVE_HERE_PATH);
            areaMoveHere.Clicked += OnClickedMoveArea;
            areaMoveHere.SetActive(false);
            MouseDetector.SetActive(false);
        }

        private void GridInit()
        {
            gridContainer = GetNode<Node2D>(GRID_CONTAINER_PATH);
            LevelGrid = Grid.GenerateFromFile(levelInfo, gridContainer);
            LevelGrid.Scale = gridScale;
            LevelGrid.SetPathfinding(false);

            Vector2 lSize = LevelGrid.PixelSize * LevelGrid.GlobalScale * 0.5f * GlobalScale;
            Vector2[] lPoints = new Vector2[4];

            for (int i = 0; i < SHAPE_POINT_NUMBER; i++)
            {
                lPoints[i] = MathS.IsoMatrix * lSize;
                lSize = lSize.Rotated(MathF.PI * 0.5f);
            }

            ((ConvexPolygonShape2D)MouseDetector.Collider.Shape).Points = lPoints;
        }

        private void OnMoved(int pCurrentLevelNumber, bool pToNextLevel)
        {
            OnButtonSelected(START_LEVEL_NUMBER);
        }

        private void OnButtonSelected(int pLevelNumber)
        {
            bool lIsThis = pLevelNumber == LevelNumber;
            bool lUnlocked = LevelSelector.UnlockedLevels >= LevelNumber;

            MouseDetector.SetActive(lIsThis && lUnlocked);
            areaMoveHere.SetActive(lUnlocked && (pLevelNumber == LevelNumber + 1 || pLevelNumber == LevelNumber - 1));
            if (lIsThis)
                LevelSelector.selectedLevel = this;
        }

        private void OnLevelSelectorInput(int pLevelNumber)
        {
            if (pLevelNumber == LevelNumber)
                OnClick();
        }

        protected override void OnClick()
        {
            if (LevelManager.CurrentLevel != null)
                return;

            base.OnClick();
            LevelLoadingAnim();
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.StartLevel, LevelGrid);
        }

        private void OnClickedMoveArea()
        {
            SignalBus.Instance.EmitSignal(
                SignalBus.SignalName.MoveToLevelButton,
                LevelNumber,
                LevelSelector.SelectedLevelNumber < LevelNumber);
        }

        private void LevelLoadingAnim()
        {
            Vector2 lCameraGlobalPos = GetViewport().GetCamera2D().GlobalPosition;
            LevelSelector.GlobalPosition -= lCameraGlobalPos;
            lCameraGlobalPos = GetViewport().GetCamera2D().GlobalPosition = Vector2.Zero;
            levelBackground = NodeCreator.CreateNode<LevelBackground>(sceneBackground, GameManager.Instance.AssetsContainer);
            levelBackground.GlobalPosition = lCameraGlobalPos;

            Tween lTween = CreateTween();
            lTween.SetParallel();
            lTween.TweenProperty(levelBackground, TweenProp.MODULATE_ALPHA, 0f, 0f);
            lTween.TweenProperty(levelBackground, TweenProp.MODULATE_ALPHA, 1f, LevelSelector.TRANSITION_TIME);
            lTween.TweenProperty(LevelGrid, TweenProp.GLOBAL_POSITION, lCameraGlobalPos, LevelSelector.TRANSITION_TIME);
            lTween.TweenProperty(LevelGrid, TweenProp.GLOBAL_SCALE, Vector2.One, LevelSelector.TRANSITION_TIME);
            lTween.Finished += ResetGridPos;
            lTween.Finished += levelBackground.PlayAppearAnim;
        }

        private void ResetGridPos()
        {
            LevelGrid.GlobalPosition = levelBackground.GlobalPosition = GetViewport().GetCamera2D().GlobalPosition = default;
        }

        protected override void Dispose(bool disposing)
        {
            if (IsInstanceValid(areaMoveHere))
                areaMoveHere.Clicked -= OnClickedMoveArea;
            SignalBus.Instance.ArrivedAtLevelButton -= OnButtonSelected;
            SignalBus.Instance.MoveToLevelButton -= OnMoved;
            SignalBus.Instance.LevelClicked -= OnLevelSelectorInput;
            base.Dispose(disposing);
        }
    }
}
