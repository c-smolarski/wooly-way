using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.GameObjects;
using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using Color = Godot.Color;
using System;
using System.Collections.Generic;
using System.Linq;
using Environment = Godot.Environment;

// Author : Camille SMOLARSKI & Tom BAGNARA && Alissa Delattre

namespace Com.IsartDigital.WoolyWay
{
    public partial class Tile : Node2D
    {
        [Export] private Node2D polygonContainer;
        [Export] private ClickableArea clickable;
        [Export] public Label debugLabel;
        [Export] public ColorRect debugRect;
        [Export] private Color tileColor = Colors.Black;
        [Export] public GpuParticles2D butterfly;
        [Export] private Sprite2D dogTreat;
        [Export] private Polygon2D polygon;
        [ExportGroup("Pen Parts")]
        [Export] private Sprite2D[] penBGParts;
        [Export] private Sprite2D[] penFGParts;
        [Export] private Sprite2D[] penSticks;
        [Export] private Sprite2D[] penBars;

        private const float CENTER_SIZE = 100f;
        private const float OUTLINE_SIZE = 4f;
        private const int POLYGON_POINT_NUMBER = 4;
        private const float START_SCALE = 0f;
        private const float FINAL_SCALE = .16f;
        private const float TWEEN_DURATION = .2f;
        private const float POLYGON_TWEEN_DURATION = .8f;
        private const float POLYGON_TWEEN_DELAY = .2f;
        private Color polygonColor = new Color(1f, 1f, 1f, 0f);
        private Tween polygonTween;

        public static readonly Vector2 SIZE = Vector2.One * (CENTER_SIZE + OUTLINE_SIZE * 0.5f) * 2f;

        public Vector2[] ColliderShapePoints => (Vector2[])((ConvexPolygonShape2D)clickable.Collider.Shape).Points.Clone();
        private Pathfinding pathfinding = new Pathfinding();
        public GameObject Object => Grid.ObjectDict[this];
        public Vector2I Index => Grid.IndexDict[this];
        public Grid Grid { get; private set; }
        public Dog Dog { get; private set; }
        public bool IsFlag { get; private set; }

        public override void _Ready()
        {
            base._Ready();
            PolygonInit();
            ColliderInit();
            clickable.Clicked += SendCoord;
            clickable.MouseEntered += Glow;
            clickable.MouseExited += StopGlow;
        }

        private void Glow()
        {
            polygonTween = CreateTween();
            polygonTween.SetLoops();
            polygonTween.TweenProperty(polygon, "color", new Color(1f, 1f, 1f, 0.3f), POLYGON_TWEEN_DURATION)
                .From(new Color(1f, 1f, 1f, 0f))
                .SetEase(Tween.EaseType.In);
            polygonTween.TweenProperty(polygon, "color", new Color(1f, 1f, 1f, 0f), POLYGON_TWEEN_DURATION)
                .SetEase(Tween.EaseType.Out)
                .SetDelay(POLYGON_TWEEN_DELAY);
        }

        private void StopGlow()
        {
            if (polygonTween != null)
            {
                polygonTween.Stop();
                polygonTween = null;
            }
            polygon.Color = polygonColor;
        }

        /*
         * INIT METHODS
         */

        private void PolygonInit()
        {
            Vector2[] lPoints;
            foreach (Polygon2D lPolygon in polygonContainer.GetChildren())
            {
                lPolygon.Color = tileColor;
                lPoints = new Vector2[POLYGON_POINT_NUMBER];

                for (int i = 0; i < POLYGON_POINT_NUMBER; i++)
                    lPoints[i] = MathS.IsoMatrix * (i == 0 || i == POLYGON_POINT_NUMBER - 1 ? Vector2.One : new Vector2(1, -1)) * (CENTER_SIZE + (i >= POLYGON_POINT_NUMBER / 2 ? OUTLINE_SIZE : 0));
                lPolygon.Polygon = lPoints;

            }
        }

        private void ColliderInit()
        {
            Vector2 lCurrentPoint = Vector2.One;
            Vector2[] lPoints = new Vector2[POLYGON_POINT_NUMBER];
            for (int i = 0; i < POLYGON_POINT_NUMBER; i++)
            {
                lPoints[i] = MathS.IsoMatrix * lCurrentPoint * (CENTER_SIZE + OUTLINE_SIZE * 0.5f);
                lCurrentPoint = lCurrentPoint.Rotated(MathF.PI * 0.5f);
            }
            ((ConvexPolygonShape2D)clickable.Collider.Shape).Points = lPoints;
        }

        /*
         * MOVEMENT METHODS
         */

        /// <summary>
        /// Checks if an object can move on Tile. If the Tile has a Sheep on it, checks if the Sheep can move out of the Tile.
        /// </summary>
        /// <param name="pMoveDirection"></param>
        /// <returns></returns>
        public bool IsWalkable(Vector2I pMoveDirection)
        {
            if (this.IsEmpty()) return true;
            if (Grid.ObjectDict[this] is Obstacle || Grid.ObjectDict[this] is Player) return false;
            if (!this.IsSheep()) return true;

            Sheep lSheep = Grid.ObjectDict[this] as Sheep;

            return lSheep != null && lSheep.CanMove(pMoveDirection);
        }

        /// <summary>
        /// Checks if GameObjects are on this Tile.
        /// </summary>
        /// <returns>true if no GameObject are on this Tile</returns>
        public bool IsEmpty()
        {
            return !Grid.ObjectDict.Contains(this);
        }

        /// <summary>
        /// Chekcs if this Tile contains a Sheep.
        /// </summary>
        /// <returns>true if Tile contains a Sheep</returns>
		public bool IsSheep()
        {
            if (!IsEmpty()) return (Grid.ObjectDict[this] is Sheep);
            return false;
        }

        /*
         * PATHFINDING METHODS
         */

        public void SetPathfinding(bool pEnabled)
        {
            clickable.SetActive(pEnabled);
        }

        /// <summary>
        /// Gets the position of the tile that was clicked on and send it to the pathfinding as the target then calls the function to move the player
        /// </summary>
        private void SendCoord()
        {
            if(LevelManager.CurrentLevel != Grid)
                return;

            Player lPlayer = Grid.Player;
            Vector2I lTargetPos = Grid.IndexDict[this];
            Vector2I LStartPos = Grid.IndexDict[Grid.ObjectDict[lPlayer]];

            List<Vector2I> lPath = pathfinding.GetPath(LStartPos.X, LStartPos.Y, lTargetPos.X, lTargetPos.Y);

            if (lPath.Count == 0) return;

            lPath.Remove(lPath.Last());
            lPlayer.path = lPath;
            lPlayer.MoveStepByStep();
        }

        /*
         * CREATION AND POSITION METHODS
         */

        public void SetDog(Dog pDog)
        {
            Dog = pDog;
            dogTreat.Visible = true;
        }

        public void SetFlag(bool pFlag)
        {
            IsFlag = pFlag;
            // Todo replace with actual sprite
            if (IsFlag)
            {
                foreach (Sprite2D lBgPart in penBGParts)
                    lBgPart.ZIndex = GetDiagonalIndex(DiagonalDirection.BOTTOM_RIGHT) - 1;
                foreach (Sprite2D lFgPart in penFGParts)
                    lFgPart.ZIndex = GetDiagonalIndex(DiagonalDirection.BOTTOM_RIGHT) + 1;
                foreach (Sprite2D lStick in penSticks)
                    lStick.Visible = true;
            }
        }

        public Vector2 GetPosFromIndex()
        {
            return GetPosFromIndex(Grid.IndexDict[this].X, Grid.IndexDict[this].Y, Grid);
        }

        /// <summary>
        /// Calculates a tile's position so that it is placed on the grid, with the grid's center at the center of the viewport.
        /// </summary>
        /// <param name="pIndexX"></param>
        /// <param name="pIndexY"></param>
        /// <param name="pGrid"></param>
        /// <returns></returns>
        private Vector2 GetPosFromIndex(int pIndexX, int pIndexY, Grid pGrid)
        {
            return MathS.IndexToPosition(SIZE, pIndexX, pIndexY) - 0.5f * MathS.PositionToIsoPosition((pGrid.Size - Vector2I.One) * SIZE);
        }

        public void SetWinAnimation()
        {
            foreach (Sprite2D lBar in penBars)
            {
                lBar.Visible = true;
                Tween lTween = CreateTween();
                lTween.TweenProperty(lBar, TweenProp.SCALE, new Vector2(FINAL_SCALE, lBar.Scale.Y), TWEEN_DURATION)
                    .From(new Vector2(START_SCALE, lBar.Scale.Y));
            }
        }

        public void RemoveWinAnim()
        {
            foreach (Sprite2D lBar in penBars)
                lBar.Visible = false;
        }

        public int GetDiagonalIndex(DiagonalDirection pDirection)
        {
            Vector2I lGridMaxIndex = Grid.Size - Vector2I.One;

            switch (pDirection)
            {
                case DiagonalDirection.TOP_RIGHT:
                    return Index.X - Index.Y + lGridMaxIndex.Y;
                case DiagonalDirection.TOP_LEFT:
                    return (lGridMaxIndex.X - Index.X) + (lGridMaxIndex.Y - Index.Y);
                case DiagonalDirection.BOTTOM_LEFT:
                    return Index.Y - Index.X + lGridMaxIndex.X;
                case DiagonalDirection.BOTTOM_RIGHT:
                    return Index.Y - (lGridMaxIndex.X - Index.X) + lGridMaxIndex.X;
            }
            return default;
        }

        public static Tile Create(int pIndexX, int pIndexY, Grid pGrid)
        {
            Tile lTile = NodeCreator.CreateNode<Tile>(GameManager.Instance.TileScene, pGrid);
            lTile.Grid = pGrid;
            lTile.Position = lTile.GetPosFromIndex(pIndexX, pIndexY, pGrid);
            return lTile;
        }
    }
}
