using Godot;
using System;

// Author : Julien Lim

namespace Com.IsartDigital.ProjectName
{

    public partial class main_menu : Control
    {
        private Control Credits;
        public override void _Ready()
        {
            GetNode<Button>("VBoxContainer/StartButton").Pressed += OnStartButtonPressed;
            GetNode<Button>("VBoxContainer/SettingsButton").Pressed += OnSettingsButtonPressed;
            GetNode<Button>("VBoxContainer/CreditsButton").Pressed += OnCreditsButtonPressed;
            GetNode<Button>("VBoxContainer/QuitButton").Pressed += OnQuitButtonPressed;
            Credits = GetNode<Control>("Credits");
            
        }

        private void OnStartButtonPressed()
        {
            GD.Print("Start button pressed");
        }

        private void OnSettingsButtonPressed()
        {
            GD.Print("Settings button pressed");
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


