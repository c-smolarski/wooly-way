using Com.IsartDigital.WoolyWay.Data;
using Com.IsartDigital.WoolyWay.GameObjects;
using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils.TwoWayDictionnaries;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// Author : Alissa DELATTRE & Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay
{
    public partial class Grid : Node2D
    {
        [Signal] public delegate void FinishedTutorialGridEventHandler(Grid pOldGrid);

        public MapInfo Info { get; private set; }
        public Player Player { get; private set; }
        public Vector2I Size { get; private set; }
        public Vector2 PixelSize => Tile.SIZE * Size;
        public ReadOnlyTwoWayDictionary<Tile, GameObject> ObjectDict { get; set; } = new();
        public ReadOnlyTwoWayDictionary<Vector2I, Tile> IndexDict { get; private set; } = new();
        public ReadOnlyTwoWayDictionary<Tile, Target> IndexTarget { get; private set; } = new();

        public List<Sheep> SheepList { get; private set; } = new();

        private bool hasAlreadyWon;

        public override void _Ready()
        {
            base._Ready();
            SignalBus.Instance.LevelRestarted += RemoveWinAnim;
        }

        public void SetPathfinding(bool pEnabled)
        {
            foreach (Tile lTile in IndexDict.Values)
                lTile.SetPathfinding(pEnabled);
        }

        /*
         * WIN METHODS
         */

        /// <summary>
        /// Checks if all sheep are correctly placed, then either instantiate the win screen or the next level
        /// if it's a tutorial
        /// </summary>
        public bool WinCheck()
        {
            if (SheepList.Any(lSheep => !lSheep.IsWin))
                return false;

            foreach (Tile lTile in IndexDict.Values)
                if (lTile.IsFlag && !hasAlreadyWon)
                    lTile.SetWinAnimation();

            return hasAlreadyWon = true;
        }

        public void InitWin()
        {
            if (LevelManager.isTutorial)
            {
                Tutorial.Instance.NexTutorial();
                EmitSignal(SignalName.FinishedTutorialGrid, this);
            }
            else
                GameManager.Instance.InitWin();
        }

        private void RemoveWinAnim()
        {
            hasAlreadyWon = false;
            foreach (Tile lTile in IndexDict.Values)
                if (lTile.IsFlag)
                    lTile.RemoveWinAnim();
        }

        /*
         * TILE METHODS
         */

        public void ResetTilesPos()
        {
            foreach (Tile lTile in IndexDict.Values)
                lTile.Position = lTile.GetPosFromIndex();
            foreach (GameObject lObject in ObjectDict.Values)
                lObject.Position = lObject.CurrentTile.Position;
        }

        public List<Tile> Neighbors(Tile pCurrentTile)
        {
            List<Tile> lNeighbors = new List<Tile>();
            List<Vector2I> lDirectionPossible = new List<Vector2I> { Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };
            int lSizeList = lDirectionPossible.Count;

            for (int i = 0; i < lSizeList; i++)
            {
                Vector2I lPossibleNeighbor = IndexDict[pCurrentTile] + lDirectionPossible[i];
                if (lPossibleNeighbor.X >= 0 && lPossibleNeighbor.X < Size.X &&
                    lPossibleNeighbor.Y >= 0 && lPossibleNeighbor.Y < Size.Y)

                {
                    lNeighbors.Add(IndexDict[lPossibleNeighbor]);
                }
            }
            return lNeighbors;
        }

        /*
         * GRID CREATION METHODS
         */

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
            lGrid.ZIndex = lGrid.GetParent<Node2D>().ZIndex;

            TwoWayDictionary<Vector2I, Tile> lDict = new();

            lGrid.Size = pGridSize;
            for (int y = 0; y < pGridSize.Y; y++)
                for (int x = 0; x < pGridSize.X; x++)
                {
                    Tile lTile = Tile.Create(x, y, lGrid);
                    lTile.debugLabel.Text = x.ToString() + y.ToString();
                    lDict.Add(new Vector2I(x, y), lTile);
                }
            lGrid.IndexDict = lDict.ToReadOnly();
            return lGrid;
        }


        //Creates grid depending on the size of the level
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
            List<Sheep> lSheepList = new();
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
                        case LevelChar.FAKE_SHEEP:
                        case LevelChar.SHEEP:
                            bool lUseful = lChar == LevelChar.SHEEP;
                            lObj = Sheep.Create(lPacked, lGrid.IndexDict[new Vector2I(x, y)], StringDirection.GetDirection(lSheepDirections[lSheepCount]), lUseful);
                            StringDirection.SetSpriteDirection(StringDirection.GetDirection(lSheepDirections[lSheepCount]), ((Sheep)lObj).Renderer);
                            lSheepCount++;
                            if(lUseful)
                                lSheepList.Add((Sheep)lObj);
                            break;
                        case LevelChar.DOG:
                            lObj = Dog.Create(lPacked, lGrid.IndexDict[new Vector2I(x, y)], StringDirection.GetDirection(lDogDirections[lDogCount]));
                            lDogCount++;
                            break;
                        case LevelChar.TARGET:
                            lObj = null;
                            lGrid.IndexDict[new Vector2I(x, y)].SetFlag(true);
                            break;
                        default:
                            lObj = GameObject.Create(lPacked, lGrid.IndexDict[new Vector2I(x, y)]);
                            if (lObj is Player && lGrid.Player == null)
                                lGrid.Player = (Player)lObj;
                            break;
                    }
                    if (lObj != null) lTempDictObj.Add(lGrid.IndexDict[new Vector2I(x, y)], lObj);
                }
            }

            lGrid.ObjectDict = lTempDictObj.ToReadOnly();
            lGrid.IndexTarget = lTempDictTarget.ToReadOnly();
            lGrid.SheepList = lSheepList;
            lGrid.Info = pMap;

            foreach (KeyValuePair<Tile, GameObject> item in lGrid.ObjectDict)
            {
                if (item.Value is Obstacle && item.Value is not Dog)
                {
                    Obstacle lWall = item.Value as Obstacle;
                    lWall.InitWall();
                }
            }
            return lGrid;
        }

        protected override void Dispose(bool pDisposing)
        {
            EmitSignal(SignalName.FinishedTutorialGrid, this);
            if (LevelManager.CurrentLevel == this)
                LevelManager.CurrentLevel = null;
            SignalBus.Instance.LevelRestarted -= RemoveWinAnim;
            base.Dispose(pDisposing);
        }
    }
}
