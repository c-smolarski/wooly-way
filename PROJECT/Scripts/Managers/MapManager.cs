using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Utils.TwoWayDictionnaries;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay.Managers
{

    public partial class MapManager : Node2D
    {
        #region Singleton
        static private MapManager instance;
        private MapManager() { }

        static public MapManager GetInstance()
        {
            if (instance == null) instance = new MapManager();
            return instance;
        }

        #endregion


        public List<string> currentLevel;
        private string pathJson = "Properties/leveldesign.json";
        
        //Exports
        [Export] private PackedScene wallPacked;
        [Export] private Node2D gameContainer;
        [Export] private PackedScene playerPacked;
        [Export] private PackedScene dogPacked;
        [Export] private PackedScene sheepPacked;
        [Export] private PackedScene arrivalPacked;

        public static ReadOnlyTwoWayDictionary<Tile, GameObject> ObjectDict { get; private set; } = new();

        private MapData mapData = new MapData();
        public override void _Ready()
        {
            #region Singleton
            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(MapManager) + "Instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion

            ExtractData();
        }

        private void ExtractData()
        {
            
            string data = File.ReadAllText(pathJson);
            try
            {
                //Deserialize json data into MapData class
                mapData = JsonSerializer.Deserialize<MapData>(data)!;
                GD.Print(mapData.Level1.Map);

                GenerateLevel();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
        }

        private void GenerateLevel()
        {
            //A changer en fonction du niveau que vous voulez tester, a mettre dans le level selector plus tard
            currentLevel = mapData.Level6.Map;

            //Creates grid depending on the size of the level
            GridManager.Instance.GenerateNewGrid(new Vector2I(currentLevel[1].Length, currentLevel.Count));
            TwoWayDictionary<Tile, GameObject> lTempDict = new();
            GameObject lObj;

            for (int y = 0; y < currentLevel.Count; y++)
            {
                for (int x = 0; x < currentLevel[y].Length; x++)
                {
                    //Checks every characters of the strings in the List and instanciate to scene
                    switch (currentLevel[y][x])
                    {
                        case '#':
                            lObj = GameObject.Create(wallPacked, x, y);
                            break;
                        case '@':
                            lObj = GameObject.Create(playerPacked, x, y);
                            break;
                        case 'o':
                            lObj = GameObject.Create(dogPacked, x, y);
                            break;
                        case '$':
                            lObj = Sheep.Create(sheepPacked, new Vector2I(x, y), default);
                            //Ici recuperer si la chevre va a gauche droit etc... quand le script existera
                            //TODO : Replace default with sheep direction.
                            break;
                        case 'N':
                            lObj = Sheep.Create(sheepPacked, new Vector2I(x, y), default, false);
                            //Ici recuperer si la chevre va a gauche droit etc... quand le script existera
                            //ici envoyer un truc au script chevre comme quoi celle la doit pas pouvoir gagner
                            //TODO : Replace default with sheep direction.
                            break;
                        case '.':
                            lObj = GameObject.Create(arrivalPacked, x, y);
                            break;
                        default:
                            lObj = null;
                            break;
                    }

                    if (lObj is not null)
                        lTempDict.Add(GridManager.TileDict[new Vector2I(x, y)], lObj);
                }
            }
            ObjectDict = lTempDict.ToReadOnly();
        }

        protected override void Dispose(bool pDisposing)
        {
            if(instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}
