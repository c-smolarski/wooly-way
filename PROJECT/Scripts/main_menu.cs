using Godot;
using System;

// Author : Julien Lim

namespace Com.IsartDigital.WoolyWay
{

    public partial class main_menu : Control
    {
        [Export] private Control Credits;
        [Export] private Control Settings;
        [Export] private Button StartButton;
        [Export] private Button SettingsButton;
        [Export] private Button CreditsButton;
        [Export] private Button QuitButton;

        public override void _Ready()
        {
            CreditsButton.Pressed += OnCreditsButtonPressed;
            SettingsButton.Pressed += OnSettingsButtonPressed;
            StartButton.Pressed += OnStartButtonPressed;
            QuitButton.Pressed += OnQuitButtonPressed;
        }
        
        private void OnStartButtonPressed()
        {
            GD.Print("Start button pressed");
        }

        private void OnSettingsButtonPressed()
        {
            GD.Print("Settings button pressed");
            Settings.Show();
        }

        private void OnCreditsButtonPressed()
        {
            GD.Print("Credits button pressed");
            Credits.Show();
        }

        private void OnQuitButtonPressed()
        {
            GetTree().Quit();
        }

    }

}


