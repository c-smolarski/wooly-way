using Godot;
using System;
using System.Collections.Generic;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.Data
{
    public static class FilePath
    {
        public const string LEVELS_JSON = "res://Data/leveldesign.json";
        public const string PATH_WINSCREEN = "res://Scenes/UI/Menus/WinScreen.tscn";
        public const string LEVEL_BUTTONS_PATH = "res://Scenes/UI/Menus/LevelSelector/LevelButtons/";
        
        private const string OBSTACLES_TEXTURES_DIR = "res://Assets/GameObjects/Obstacles/";
        public const string BUSHES_TEXTURES_DIR = OBSTACLES_TEXTURES_DIR + "Bushes/";
        public const string ROCKS_TEXTURES_DIR = OBSTACLES_TEXTURES_DIR + "Rocks/";

        private const string UNWANTED_EXTENSION = ".import";

        public static void FetchAllFromFile<T>(out List<T> pList, string pPath) where T : class
        {
            DirAccess lDir = DirAccess.Open(pPath);
            pList = new();

            if (lDir != null)
            {
                lDir.ListDirBegin();
                string lFilePath = lDir.GetNext();
                while (lFilePath != "")
                {
                    if (lFilePath.Contains(UNWANTED_EXTENSION))
                        pList.Add(
                            GD.Load<T>(
                                pPath + lFilePath.Substring(
                                    0, lFilePath.Length - UNWANTED_EXTENSION.Length)));
                    lFilePath = lDir.GetNext();
                }
            }
        }
    }
}
