using Com.IsartDigital.WoolyWay.Utils;
using Com.IsartDigital.WoolyWay.Utils.Data;
using Godot;
using System;
using System.Collections.Generic;
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

        private const string WORLD_KEYWORD = "World";
        private const string LEVEL_KEYWORD = "Level";

        private const int ONE_STAR = 1;
        private const int TWO_STAR = 2;
        private const int THREE_STAR = 3;
        private const int SCORE_HIGH = 5000;
        private const int SCORE_MID = 2000;
        private const int SCORE_LOW = 1000;
        private const float SCORE_MULTIPLICATOR = 1.5f;

        public static Grid currentLevel;

        public Dictionary<string, Dictionary<string, Dictionary<string , object>>> data = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
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

        /// <summary>
        /// Extracts all info from the json file to get all the levels and their specifities
        /// </summary>
        private static void ExtractData()
        {
            string lData = File.ReadAllText(FilePath.LEVELS_JSON);

            try
            {
                //Deserialize json data into MapData class
                    MapData = JsonSerializer.Deserialize<MapData>(lData);

            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
        }

        /// <summary>
        /// Returns the total number of level in the game
        /// </summary>
        public uint NumLevels()
        {
            uint lNumLevel = 0;
            foreach (KeyValuePair<string, Dictionary<string, MapInfo>> lWorld in MapData.Worlds)
            {
                lNumLevel += (uint)lWorld.Value.Count;
            }
            return lNumLevel;
        }

        /// <summary>
        /// Call to generate grid from wanted level
        /// </summary>
        public MapInfo GenerateLevel(MapInfo pMap)
        {
            currentLevel?.QueueFree();
            currentLevel = Grid.GenerateFromFile(
                pMap,
                GameManager.Instance.GameContainer,
                GetViewport().GetVisibleRect().Size / 2f
            );
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

        /// <summary>
        /// Transforms a score to the coressponding number of stars 
        /// </summary>
        private int ScoreToStar(int pScore)
        {
            int lNumStar;
            switch (pScore)
            {
                case > SCORE_MID:
                    lNumStar = THREE_STAR;
                    break;
                case > SCORE_LOW:
                    lNumStar = TWO_STAR;
                    break;
                default:
                    lNumStar = ONE_STAR;
                    break;
            }
            return lNumStar;
        }

        /// <summary>
        /// calculates the score at the end of a level
        /// </summary>
        public void StepToScore(int pStep)
        {
            int lPar = GameManager.Instance.currentLevelInfos.Par;
            int lScore;

            if (lPar >= pStep)
            {
                lScore = SCORE_HIGH - pStep;
            }
            else if (pStep < pStep * SCORE_MULTIPLICATOR)
            {
                lScore = SCORE_MID - pStep;
            }
            else
            {
                lScore = SCORE_LOW - pStep;
            }

            int lNumStar = ScoreToStar(lScore);
            GD.Print(lScore + " " + lNumStar);
            DataManager.GetInstance().UpdateUserStats(GameManager.Instance.currentLevelInfos.LevelName, (uint)lScore, (uint)lNumStar);
        }

        protected override void Dispose(bool pDisposing)
        {
            if (instance == this) instance = null;
            base.Dispose(pDisposing);
        }
        
    }
}
