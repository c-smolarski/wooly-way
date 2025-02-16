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

        private MapData mapData;
        public override void _Ready()
        {
            ExtractData();
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
            currentLevel = mapData.level1.map;
            GridManager.Instance.GenerateNewGrid(new Vector2I(currentLevel[1].Length, currentLevel.Count));
            for (int i = 0; i < currentLevel.Count; i++)
            {
                for (int j = 0; j < currentLevel[i].Length; j++)
                {
                    if (currentLevel[i][j] == '#') {
                        wall = wallPacked.Instantiate() as Node2D;
                        wall.GlobalPosition = MathS.IndexToPosition(new Vector2(127f, 127f), new Vector2(j, i)) + GetViewportRect().Size/2;
                        gameContainer.AddChild(wall);
                    }
                    else if (currentLevel[i][j] == '@')
                    {
                        player = playerPacked.Instantiate() as Node2D;
                        player.GlobalPosition = MathS.IndexToPosition(new Vector2(127f, 127f), new Vector2(j, i)) + GetViewportRect().Size / 2;
                        gameContainer.AddChild(player);
                    }
                    else if (currentLevel[i][j] == 'o')
                    {
                        dog = dogPacked.Instantiate() as Node2D;
                        dog.GlobalPosition = MathS.IndexToPosition(new Vector2(127f, 127f), new Vector2(j, i)) + GetViewportRect().Size / 2;
                        gameContainer.AddChild(dog);
                    }
                    else if (currentLevel[i][j] == '$' || currentLevel[i][j] == 'N')
                    {
                        sheep = sheepPacked.Instantiate() as Node2D;
                        sheep.GlobalPosition = MathS.IndexToPosition(new Vector2(127f, 127f), new Vector2(j, i)) + GetViewportRect().Size / 2;
                        gameContainer.AddChild(sheep);
                    }
                }
            }
        }

        protected override void Dispose(bool pDisposing)
        {

        }
    }
}
