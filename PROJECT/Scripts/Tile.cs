using Com.IsartDigital.WoolyWay.GameObjects;
using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay
{
    public partial class Tile : Node2D
    {
        public static readonly Vector2 SIZE = new(88, 88);

        public Grid Grid { get; private set; }
        public GameObject CurrentObject => Grid.ObjectDict[this];

        public bool IsWalkable()
        {
            return CurrentObject is not Obstacle || (CurrentObject is Sheep && ((Sheep)CurrentObject).CanMove());
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

        public static Tile Create(int pIndexX, int pIndexY, Grid pGrid)
        {
            Tile lTile = NodeCreator.CreateNode<Tile>(GameManager.Instance.TileScene, pGrid);
            lTile.Grid = pGrid;
            lTile.Position = lTile.GetPosFromIndex(pIndexX, pIndexY, pGrid);
            return lTile;
        }
    }
}
