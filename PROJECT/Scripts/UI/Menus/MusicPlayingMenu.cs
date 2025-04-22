using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI.Menus
{
    public partial class MusicPlayingMenu : Control
    {
        [Export] private AudioStreamOggVorbis musicToPlay;

        public override void _Ready()
        {
            base._Ready();
            SoundManager.PlayMusic(musicToPlay, GetTree().Root);
        }
    }
}
