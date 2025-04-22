using Godot;
using System;

// Author : Daniel Moussouni--Lepilliez

namespace Com.IsartDigital.WoolyWay.Data
{
    public static class SoundPath
    {
        //SOUND BUSES
        public const string MAIN_SOUND_BUS = "Main";
        public const string MUSIC_SOUND_BUS = "Music";
        public const string AMBIENT_SOUND_BUS = "Ambient";
        public const string SFX_SOUND_BUS = "SFX";

        //MUSICS
        public const string MUSIC_MAINMENU = "res://Sounds/Musics/mainmenu.ogg";
        public const string MUSIC_LEADERBOARD = "res://Sounds/Musics/leaderboard.ogg";
        public const string MUSIC_LEVELS = "res://Sounds/Musics/level.ogg";
        public const string DOG_BARK_FOLDER = "res://Sounds/SFX/Dog/";
        public const string SHEEP_SOUND = "res://Sounds/SFX/Sheep/sheep1.ogg";


        //SFXs
        public const string UI_BUTTON_SFX_FOLDER = "res://Sounds/SFX/UI/Button/";
        public const string UI_BUTTON_APPEAR_SFX = "res://Sounds/SFX/plop.ogg";
        public const string UI_TRANSITION_SOUND_PATH = "res://Sounds/SFX/Sheep/stampede.ogg";
    }
}