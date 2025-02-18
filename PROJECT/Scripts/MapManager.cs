using Com.IsartDigital.WoolyWay;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName
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

        //Objects
        private Node2D wall;
        private Node2D player;
        private Node2D dog;
        private Node2D sheep;
        private Node2D arrival;
        private Dictionary<Node2D, Node2D> gameObject = new();

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
            GetWindow().SizeChanged += UpdatePos;
        }

        public override void _Process(double pDelta)
        {
            float lDelta = (float)pDelta;

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
            for (int i = 0; i < currentLevel.Count; i++)
            {
                for (int j = 0; j < currentLevel[i].Length; j++)
                {
                    //Checks every characters of the strings in the List and instanciate to scene
                    switch (currentLevel[i][j])
                    {
                        case '#':
                            wall = wallPacked.Instantiate() as Node2D;
                            CreateObject(wall, i, j);
                            break;

                        case '@':
                            player = playerPacked.Instantiate() as Node2D;
                            CreateObject(player, i, j);
                            break ;

                        case 'o':
                            dog = dogPacked.Instantiate() as Node2D;
                            CreateObject(dog, i, j);
                            break;

                        case '$':
                            sheep = sheepPacked.Instantiate() as Node2D;
                            CreateObject(sheep, i, j);
                            //Ici recuperer si la chevre va a gauche droit etc... quand le script existera
                            break;

                        case 'N':
                            sheep = sheepPacked.Instantiate() as Node2D;
                            CreateObject(sheep, i, j);
                            //Ici recuperer si la chevre va a gauche droit etc... quand le script existera
                            //ici envoyer un truc au script chevre comme quoi celle la doit pas pouvoir gagner
                            break;

                        case '.':
                            arrival = arrivalPacked.Instantiate() as Node2D;
                            CreateObject(arrival, i, j);
                            break;

                        default:
                            break;
                    }   
                }
            }
        }

        //instanciates the objects at the right place
        private void CreateObject(Node2D pObject, int pI, int pJ)
        {
            pObject.GlobalPosition = GridManager.TileDict[new Vector2I(pI, pJ)].GlobalPosition;
            gameContainer.AddChild(pObject);
            gameObject[pObject] = GridManager.TileDict[new Vector2I(pI, pJ)];
        }

        //Update the position of all gameObject when the screenSize changes
        private void UpdatePos()
        {
            foreach(KeyValuePair<Node2D,Node2D> lPair in gameObject)
            {
                lPair.Key.GlobalPosition = lPair.Value.GlobalPosition;
            }
        }
        protected override void Dispose(bool pDisposing)
        {
            instance = null;
            base.Dispose(pDisposing);
        }
    }
}
