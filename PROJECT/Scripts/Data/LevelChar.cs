using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using Godot.Collections;
using System;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.Data
{
    public static class LevelChar
    {
        public const char PLAYER = '@';
        public const char WALL = '#';
        public const char DOG = 'o';
        public const char SHEEP = '$';
        public const char FAKE_SHEEP = 'N';
        public const char TARGET = '.';

        private static GameManager GManager => GameManager.Instance;

        public static readonly Dictionary<char, PackedScene> ToPackedScene = new()
        {
            { PLAYER, GManager.PlayerScene },
            { WALL, GManager.WallScene },
            { DOG, GManager.DogScene },
            { SHEEP, GManager.SheepScene },
            { FAKE_SHEEP, GManager.SheepScene },
            { TARGET, GManager.TargetScene }
        };
    }
}
