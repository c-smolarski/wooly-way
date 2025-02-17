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
        public List<string> currentLevel;
        private string pathJson = "Properties/leveldesign.json";

        [Export] private PackedScene wallPacked;
        [Export] private Node2D gameContainer;
        [Export] private PackedScene playerPacked;
        [Export] private PackedScene dogPacked;
        [Export] private PackedScene sheepPacked;

        private Node2D wall;
        private Node2D player;
        private Node2D dog;
        private Node2D sheep;
        private Dictionary<Node2D, Node2D> gameObject = new();

        private MapData mapData = new MapData();
        public override void _Ready()
        {
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
                mapData = JsonSerializer.Deserialize<MapData>(data)!;
                GD.Print(mapData.Level1.Map);

                GenerateLevel();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
        }

        //TODO changer cet horreur c trop moche
        private void GenerateLevel()
        {
            
            currentLevel = mapData.Level1.Map;
            
            GridManager.Instance.GenerateNewGrid(new Vector2I(currentLevel[1].Length, currentLevel.Count));
            for (int i = 0; i < currentLevel.Count; i++)
            {
                for (int j = 0; j < currentLevel[i].Length; j++)
                {
                    //TODO switch case?
                    if (currentLevel[i][j] == '#')
                    {
                        wall = wallPacked.Instantiate() as Node2D;
                        wall.GlobalPosition = GridManager.TileDict[new Vector2I(i, j)].GlobalPosition;
                        GD.Print(wall.GlobalPosition, GridManager.TileDict[new Vector2I(i, j)].GlobalPosition);
                        gameContainer.AddChild(wall);
                        GD.Print(gameObject);
                        gameObject[wall] = GridManager.TileDict[new Vector2I(i, j)];
                    }
                    else if (currentLevel[i][j] == '@')
                    {
                        player = playerPacked.Instantiate() as Node2D;
                        player.GlobalPosition = GridManager.TileDict[new Vector2I(i, j)].GlobalPosition;
                        gameContainer.AddChild(player);
                        gameObject[player] = GridManager.TileDict[new Vector2I(i, j)];
                    }
                    else if (currentLevel[i][j] == 'o')
                    {
                        dog = dogPacked.Instantiate() as Node2D;
                        dog.GlobalPosition = GridManager.TileDict[new Vector2I(i, j)].GlobalPosition;
                        gameContainer.AddChild(dog);
                        gameObject[dog] = GridManager.TileDict[new Vector2I(i, j)];
                    }
                    else if (currentLevel[i][j] == '$' || currentLevel[i][j] == 'N')
                    {
                        sheep = sheepPacked.Instantiate() as Node2D;
                        sheep.GlobalPosition = GridManager.TileDict[new Vector2I(i, j)].GlobalPosition;
                        gameContainer.AddChild(sheep);
                        gameObject[sheep] = GridManager.TileDict[new Vector2I(i, j)];
                    }
                }
            }
        }

        private void UpdatePos()
        {
            foreach(KeyValuePair<Node2D,Node2D> lPair in gameObject)
            {
                lPair.Key.GlobalPosition = lPair.Value.GlobalPosition;
            }
        }

        protected override void Dispose(bool pDisposing)
        {

        }
    }
}
