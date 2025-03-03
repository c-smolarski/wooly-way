using Com.IsartDigital.WoolyWay.Scripts.Utils;
using Com.IsartDigital.WoolyWay.Utils.Data;
using Godot;
using System;
using System.IO;
using System.Text.Json;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay.Managers
{
    //DO NOT ADD AS NODE, IT IS NOW AN AUTOLOAD
    public partial class LevelManager : Node
    {
        #region Singleton
        static private LevelManager instance;
        private LevelManager() { }

        static public LevelManager GetInstance()
        {
            if (instance == null) instance = new LevelManager();
            return instance;
        }

        #endregion

        private static Grid currentLevel;
        public static MapData MapData { get; private set; } = new MapData();

        public override void _Ready()
        {
            #region Singleton
            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(LevelManager) + "Instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion

            ExtractData();
        }

        private static void ExtractData()
        {
            string data = File.ReadAllText(FilePath.LEVELS_JSON);
            
            try
            {
                //Deserialize json data into MapData class
                MapData = JsonSerializer.Deserialize<MapData>(data)!;
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
        }

        public void GenerateLevel(MapInfo pMap)
        {
            currentLevel?.QueueFree();
            currentLevel = Grid.GenerateFromFile(
                pMap, //A changer en fonction du niveau que vous voulez tester, a mettre dans le level selector plus tard
                GameManager.Instance.GameContainer,
                GetViewport().GetVisibleRect().Size / 2
            );
        }

        protected override void Dispose(bool pDisposing)
        {
            if (instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}
