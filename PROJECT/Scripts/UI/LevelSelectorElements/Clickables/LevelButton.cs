using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables
{
    public partial class LevelButton : ClickableLevelSelectorElement
    {
        [Signal] public delegate void FocusedEventHandler(bool pFocused);

        [Export] private LevelSelector mountain;
        [Export(PropertyHint.Range, "1, 100, 1")] public int LevelNumber { get; private set; } = 1;

        private const int SHAPE_POINT_NUMBER = 4;

        public Grid LevelGrid { get; private set; }

        private MapInfo levelInfo;

        public override void _Ready()
        {
            base._Ready();
            levelInfo = LevelManager.GetLevel(mountain.WorldNumber, LevelNumber);
            GridInit();
        }

        private void GridInit()
        {
            LevelGrid = Grid.GenerateFromFile(levelInfo, this);

            Vector2 lSize = Vector2.One * Mathf.Max(LevelGrid.PixelSize.X, LevelGrid.PixelSize.Y) * 0.5f;
            Vector2[] lPoints = new Vector2[4];

            for (int i = 0; i < SHAPE_POINT_NUMBER; i++)
            {
                lPoints[i] = MathS.IsoMatrix * lSize;
                lSize = lSize.Rotated(MathF.PI * 0.5f);
            }

            ((ConvexPolygonShape2D)MouseDetector.Collider.Shape).Points = lPoints;
        }
    }
}
