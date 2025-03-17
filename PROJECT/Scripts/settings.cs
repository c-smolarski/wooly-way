using Godot;
using System;

// Author : Julien Lim

namespace Com.IsartDigital.WoolyWay 
{
	
	public partial class settings : Control
	{
        [Export] private Label MusicLabel;
        [Export] private Label LanguageLabel;
        [Export] private Button QuitButton;
        [Export] private Button BackButton;
        [Export] private Button EnglishButton;
        [Export] private Button FrenchButton;
        public override void _Ready()
		{
            BackButton.Pressed += BackButton_Pressed;
			QuitButton.Pressed += OnQuitButtonPressed;
            EnglishButton.Pressed += OnEnglishButtonPressed;
            FrenchButton.Pressed += OnFrenchButtonPressed;

            TranslationServer.SetLocale("en");
            updateUI();
        }

        private void BackButton_Pressed()
        {
            QueueFree();
        }

        private void OnEnglishButtonPressed()
        {
            TranslationServer.SetLocale("en");
            updateUI();
        }
        private void OnFrenchButtonPressed()
        {
            TranslationServer.SetLocale("fr");
            updateUI();
        }

        private void OnQuitButtonPressed()
        {
            GetTree().Quit();
        }

        private void updateUI()
        {
            MusicLabel.Text = Tr("MUSIC");
            LanguageLabel.Text = Tr("LANGUAGE");
            QuitButton.Text = Tr("QUIT");
            BackButton.Text = Tr("BACK");

        }
	}
}
