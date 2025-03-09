using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils;
using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.RotatingElements;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables
{
    public partial class LevelButton : ClickableLevelSelectorElement
    {
        [Export] public LevelButtonDisplayer Displayer { get; private set; }
        [ExportGroup("On focus")]
        [Export] private float sinusoidFreq = 2.5f;
        [Export] private float sinusoidIntensity = 0.45f;

        public const int DISPLAY_NO_LEVEL = -1;

        public Grid LevelGrid { get; private set; }

        private float elapsedTime;
        private bool focused;
        private MapInfo levelInfo;
        private ConvexPolygonShape2D shape;

        public override void _Ready()
        {
            base._Ready();
            levelInfo = LevelManager.GetLevel(Displayer.Mountain.WorldNumber, Displayer.LevelNumber);

            GridInit();
        }

        public override void _Process(double pDelta)
        {
            base._Process(pDelta);
            float lDelta = (float)pDelta;
            if (focused)
                FocusedAnim(lDelta);
        }

        private void GridInit()
        {
            LevelGrid = Grid.GenerateFromFile(levelInfo, this);

            Vector2 lSize = LevelGrid.PixelSize * 0.5f;
            shape = (ConvexPolygonShape2D)MouseDetector.Collider.Shape;
            shape.Points = new Vector2[4]{
                new Vector2(lSize.X, 0),
                new Vector2(0, lSize.Y),
                new Vector2(-lSize.X, 0),
                new Vector2(0, -lSize.Y)
            };
        }

        private void FocusedAnim(float pDelta)
        {
            elapsedTime += pDelta;
            float lSinusoid, lMaxValue = MathF.Max(LevelGrid.Size.X, LevelGrid.Size.Y) * 2f - 1;
            for (int i = 0; i < lMaxValue; i++)
            {
                lSinusoid = elapsedTime + (i / lMaxValue);
                foreach (Tile lTile in LevelGrid.GetTilesDiagonally(DiagonalDirection.TOP_RIGHT, i))
                {
                    lTile.Position += Vector2.Down * Mathf.Sin(lSinusoid * sinusoidFreq) * sinusoidIntensity;
                    if (LevelGrid.ObjectDict.Contains(lTile))
                        LevelGrid.ObjectDict[lTile].Position = lTile.Position;
                }
            }
        }

        public void StartFocusedAnim()
        {
            StopFocusedAnim();
            focused = true;
        }

        public void StopFocusedAnim()
        {
            focused = false;
            elapsedTime = default;
            LevelGrid.ResetTilesPos();
        }
    }
}
