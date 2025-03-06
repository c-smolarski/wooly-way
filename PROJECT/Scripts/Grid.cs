using Com.IsartDigital.WoolyWay.Utils.TwoWayDictionnaries;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.WoolyWay.Scripts.Utils;
using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Utils.Data;

// Author : Alissa DELATTRE & Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay
{
    public partial class Grid : Node2D
    {
        public Vector2I Size { get; private set; }
        public ReadOnlyTwoWayDictionary<Tile, GameObject> ObjectDict { get; private set; } = new();
        public ReadOnlyTwoWayDictionary<Vector2I, Tile> IndexDict { get; private set; } = new();

        /// <summary>
        /// DiagonalDirection is based off a standard (Non-isometric) table. Max value on grid for pIndex is (Max(myGrid.Size.X, myGrid.Size.Y) - 1) * 2.
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

            Grid lGrid = CreateEmpty(new Vector2I(lXSizeLevel, lYSizeLevel), pContainer, pPos);

            TwoWayDictionary<Tile, GameObject> lTempDict = new();
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
                            lObj = Sheep.Create(lPacked, lGrid.IndexDict[new Vector2I(x, y)], default);
                            //Ici recuperer si la chevre va a gauche droit etc... quand le script existera
                            //TODO : Replace default with sheep direction.
                            break;
                        case LevelChar.FAKE_SHEEP:
                            lObj = Sheep.Create(lPacked, lGrid.IndexDict[new Vector2I(x, y)], default, false);
                            //Ici recuperer si la chevre va a gauche droit etc... quand le script existera
                            //ici envoyer un truc au script chevre comme quoi celle la doit pas pouvoir gagner
                            //TODO : Replace default with sheep direction.
                            break;
                        default:
                            lObj = GameObject.Create(lPacked, lGrid.IndexDict[new Vector2I(x, y)]);
                            break;
                    }
                    lTempDict.Add(lGrid.IndexDict[new Vector2I(x, y)], lObj);
                }
            }
            lGrid.ObjectDict = lTempDict.ToReadOnly();
            return lGrid;
        }
    }
}
