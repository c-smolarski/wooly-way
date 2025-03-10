using Godot;
using System;

// Author : Julien Lim

namespace Com.IsartDigital.ProjectName {
	
	public partial class settings : Control
	{
		private Label MusicLabel;
		private Label LanguageLabel;
		private Button QuitButton;
		private Button BackButton;
        private Button EnglishButton;
        private Button FrenchButton;
        private HBoxContainer HBoxContainer;
        public override void _Ready()
		{
			MusicLabel = GetNode<Label>("VBoxContainer2/MusicLabel");
            LanguageLabel = GetNode<Label>("VBoxContainer2/LanguageLabel");
			QuitButton = GetNode<Button>("HBoxContainer2/QuitButton");
			BackButton = GetNode<Button>("HBoxContainer2/BackButton");
            EnglishButton = GetNode<Button>("HBoxContainer/EnglishButton");
            FrenchButton = GetNode<Button>("HBoxContainer/FrenchButton");
            HBoxContainer = GetNode<HBoxContainer>("HBoxContainer2");

			QuitButton.Pressed += OnQuitButtonPressed;
            EnglishButton.Pressed += OnEnglishButtonPressed;
            FrenchButton.Pressed += OnFrenchButtonPressed;

            TranslationServer.SetLocale("en");
            updateUI();
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
