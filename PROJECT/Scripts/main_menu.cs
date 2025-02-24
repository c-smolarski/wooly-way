using Godot;
using System;

// Author : Julien Lim

namespace Com.IsartDigital.ProjectName
{

    public partial class main_menu : Control
    {
        //private CanvasLayer canvasLayer;


        public override void _Ready()
        {
            //canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
            GetNode<Button>("VBoxContainer/StartButton").Pressed += OnStartButtonPressed;
            GetNode<Button>("VBoxContainer/SettingsButton").Pressed += OnSettingsButtonPressed;
            GetNode<Button>("VBoxContainer/CreditsButton").Pressed += OnCreditsButtonPressed;
            GetNode<Button>("VBoxContainer/QuitButton").Pressed += OnQuitButtonPressed;
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
        }

        private void OnQuitButtonPressed()
        {
            GD.Print("Quit button pressed");
        }
    }

}


