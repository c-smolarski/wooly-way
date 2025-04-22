using Com.IsartDigital.WoolyWay.Utils;
using Com.IsartDigital.WoolyWay.Data;
using Godot;
using System;
using System.Collections.Generic;
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

        private const string WORLD_KEYWORD = "World";
        private const string LEVEL_KEYWORD = "Level";
        private const string TUTORIAL_WORLD_KEYWORD = "Tutorial";
        public static Grid CurrentLevel { get; set; }
        public static MapData MapData { get; private set; } = new MapData();

        public static bool isTutorial = false;
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
            SignalBus.Instance.StartLevel += LoadLevel;
            ExtractData();
        }

        /// <summary>
        /// Extracts all info from the json file to get all the levels and their specifities
        /// </summary>
        private static void ExtractData()
        {
            string lData;
            FileAccess lFile = FileAccess.Open(FilePath.LEVELS_JSON, FileAccess.ModeFlags.Read);
            lData = lFile.GetAsText();
            lFile.Close();

            MapData = JsonSerializer.Deserialize<MapData>(lData);
        }


        /// <summary>
        /// Returns the total number of level in the game
        /// </summary>
        public uint NumLevels()
        {
            uint lNumLevel = 0;
            foreach (KeyValuePair<string, Dictionary<string, MapInfo>> lWorld in MapData.Worlds)
            {
                if (lWorld.Key != TUTORIAL_WORLD_KEYWORD) lNumLevel += (uint)lWorld.Value.Count;
            }
            return lNumLevel;
        }

        /// <summary>
        /// Call to generate grid from wanted level
        /// </summary>
        public MapInfo GenerateLevel(MapInfo pMap, Node2D pContainer = default)
        {
            if (pContainer == default)
                pContainer = GameManager.Instance.GameContainer;
                CurrentLevel = Grid.GenerateFromFile(
                pMap,
                pContainer
            );
            CurrentLevel.GlobalPosition = GetViewport().GetCamera2D().GlobalPosition;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.LevelLoaded);
            return pMap;
        }

        public static MapInfo GetLevel(int pWorld, int pLevel)
        {
            return MapData.Worlds[WORLD_KEYWORD + pWorld][LEVEL_KEYWORD + pLevel];
        }

        public static bool LevelExists(int pWorld, int pLevel)
        {
            return MapData.Worlds.ContainsKey(WORLD_KEYWORD + pWorld) && MapData.Worlds[WORLD_KEYWORD + pWorld].ContainsKey(LEVEL_KEYWORD + pLevel);
        }

        private void LoadLevel(Grid pGrid)
        {
            if (IsInstanceValid(CurrentLevel))
                CurrentLevel?.QueueFree();

            CurrentLevel = pGrid;
            if (CurrentLevel.Info.LevelName != 1 && IsInstanceValid(GameManager.Instance.buttonTuto))
                GameManager.Instance.buttonTuto.QueueFree();

            pGrid.Reparent(GameManager.Instance.GameContainer);
            NodeCreator.CreateNode<History>(
                GameManager.Instance.history,
                GameManager.Instance.canvasLayer);
            pGrid.SetPathfinding(true);

            SignalBus.Instance.EmitSignal(SignalBus.SignalName.LevelLoaded);
        }

        protected override void Dispose(bool pDisposing)
        {
            SignalBus.Instance.StartLevel -= LoadLevel;
            if (instance == this)
                instance = null;
            base.Dispose(pDisposing);
        }

    }
}
