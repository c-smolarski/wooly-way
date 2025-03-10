using Com.IsartDigital.WoolyWay.GameObjects;
using Com.IsartDigital.WoolyWay.Utils.TwoWayDictionnaries;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Utils.Data;
using System.Linq;

// Author : Alissa DELATTRE & Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay
{
    public partial class Grid : Node2D
    {
        public Vector2I Size { get; private set; }
        public Vector2 PixelSize => Tile.SIZE * Size * Scale;
        public ReadOnlyTwoWayDictionary<Tile, GameObject> ObjectDict { get; private set; } = new();
        public ReadOnlyTwoWayDictionary<Vector2I, Tile> IndexDict { get; private set; } = new();
        public ReadOnlyTwoWayDictionary<Tile, Target> IndexTarget { get; private set; } = new();


        private static Dictionary<string, Vector2I> Directions = new Dictionary<string, Vector2I>(){
            {"Left", Vector2I.Left},
            {"Right", Vector2I.Right},
            {"Up", Vector2I.Up},
            {"Down", Vector2I.Down},
        };

        public void ResetTilesPos()
        {
            foreach (Tile lTile in IndexDict.Values)
                lTile.Position = lTile.GetPosFromIndex();
            foreach (GameObject lObject in ObjectDict.Values)
                lObject.Position = lObject.CurrentTile.Position;
        }

        /// <summary>
        /// DiagonalDirection is based off a standard (Non-isometric) table. Max value on grid for pIndex is (Max(myGrid.Size.X, myGrid.Size.Y) * 2) - 1.
        /// <para>Ex : For GetTilesDiagonally(BottomRight, 2) it will return tiles at index (0,2), (1,1) and (2,0).</para>
		/// </summary>
        /// <param name="pDirection"></param>
        /// <param name="pIndex"></param>
        /// <returns>A list of diagonally adjacent tiles at specified index on grid.</returns>
        /// <exception cref="Exception"></exception>
        public List<Tile> GetTilesDiagonally(DiagonalDirection pDirection, int pIndex)
        {

            List<Tile> lTileList = new();
            Func<Vector2I, int> lAction = default;
            Vector2I lGridMaxIndex = Size - Vector2I.One;

            switch (pDirection)
            {
                case DiagonalDirection.TOP_RIGHT:
                    lAction = pVector => pVector.X - pVector.Y + lGridMaxIndex.Y;
                    break;
                case DiagonalDirection.TOP_LEFT:
                    lAction = pVector => (lGridMaxIndex.X - pVector.X) + (lGridMaxIndex.Y - pVector.Y);
                    break;
                case DiagonalDirection.BOTTOM_LEFT:
                    lAction = pVector => pVector.Y - pVector.X + lGridMaxIndex.X;
                    break;
                case DiagonalDirection.BOTTOM_RIGHT:
                    lAction = pVector => pVector.Y - (lGridMaxIndex.X - pVector.X) + lGridMaxIndex.X;
                    break;
            }

            foreach (Vector2I lIndex in IndexDict.Keys)
                if (lAction(lIndex) == pIndex)
                    lTileList.Add(IndexDict[lIndex]);

            return lTileList;
        }

        /// <summary></summary>
        /// <param name="pGridSize"></param>
        /// <param name="pContainer"></param>
        /// <param name="pPos"></param>
        /// <returns>An empty grid.</returns>
        public static Grid CreateEmpty(Vector2I pGridSize, Node2D pContainer, Vector2 pPos = default)
        {
            Grid lGrid = new Grid();
            lGrid.Position = pPos;
            pContainer.AddChild(lGrid);

            TwoWayDictionary<Vector2I, Tile> lDict = new();

            lGrid.Size = pGridSize;
            for (int y = 0; y < pGridSize.Y; y++)
                for (int x = 0; x < pGridSize.X; x++)
                {
                    Tile lTile = Tile.Create(x, y, lGrid);
                    lTile.debug.Text = x.ToString() + y.ToString();
                    lDict.Add(new Vector2I(x, y), lTile);
                }
            lGrid.IndexDict = lDict.ToReadOnly();
            return lGrid;
        }


        //Creates grid depending on the size of the level
        //GridManager.Instance.GenerateNewGrid(new Vector2I(currentLevel.Map[1].Length, currentLevel.Map.Count));
        public static Grid GenerateFromFile(MapInfo pMap, Node2D pContainer, Vector2 pPos = default)
        {

            int lYSizeLevel = pMap.Map.Count;
            int lXSizeLevel = pMap.Map[0].Length;

            int lSheepCount = 0;
            int lDogCount = 0;
            
            List<string> lSheepDirections = pMap.SheepDirection;
            List<string> lDogDirections = pMap.DogDirection;
            
            Grid lGrid = CreateEmpty(new Vector2I(lXSizeLevel, lYSizeLevel), pContainer, pPos);

            TwoWayDictionary<Tile, GameObject> lTempDictObj = new();
            TwoWayDictionary<Tile, Target> lTempDictTarget = new();
            PackedScene lPacked;
            GameObject lObj;
            char lChar;

            for (int y = 0; y < lYSizeLevel; y++)
            {
                for (int x = 0; x < lXSizeLevel; x++)
                {
                    lChar = pMap.Map[y][x];
                    lPacked = LevelChar.ToPackedScene.ContainsKey(lChar) ? LevelChar.ToPackedScene[lChar] : null;
                    if (lPacked == null) continue;

                    //Checks every characters of the strings in the List and instanciate to scene
                    switch (lChar)
                    {
                        case LevelChar.SHEEP:
                            lObj = Sheep.Create(lPacked, lGrid.IndexDict[new Vector2I(x, y)], Directions[lSheepDirections[lSheepCount]]);
                            lSheepCount++;
                            break;
                        case LevelChar.FAKE_SHEEP:
                            lObj = Sheep.Create(lPacked, lGrid.IndexDict[new Vector2I(x, y)], Directions[lSheepDirections[lSheepCount]], false);
                            lSheepCount++;
                            break;
                        case LevelChar.DOG:
                            lObj = Dog.Create(lPacked, lGrid.IndexDict[new Vector2I(x, y)], Directions[lDogDirections[lDogCount]]);
                            lDogCount++;
                            break;
                        case LevelChar.TARGET:
                            lObj = null;
                            lGrid.IndexDict[new Vector2I(x, y)].SetFlag(true);
                            break;

                        default:
                            lObj = GameObject.Create(lPacked, lGrid.IndexDict[new Vector2I(x, y)]);
                            break;
                    }
                    if (lObj != null) lTempDictObj.Add(lGrid.IndexDict[new Vector2I(x, y)], lObj);
                }
            }
            lGrid.ObjectDict = lTempDictObj.ToReadOnly();
            lGrid.IndexTarget = lTempDictTarget.ToReadOnly();
            return lGrid;
        }

        public List<Tile> Neighbors(Tile pCurrentTile)
        {
            List<Tile> lNeighbors = new List<Tile>();
            List<Vector2I> lDirectionPossible = new List<Vector2I> { Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };
            int lSizeList = lDirectionPossible.Count;

            for (int i = 0; i < lSizeList; i++)
            {
                Vector2I lPossibleNeighbor = IndexDict[pCurrentTile] + lDirectionPossible[i];
                if (lPossibleNeighbor >= Vector2I.Zero && lPossibleNeighbor <= Size - Vector2I.One)
                {
                    lNeighbors.Add(IndexDict[lPossibleNeighbor]);
                }
            }
            return lNeighbors;
        }
    }
}
