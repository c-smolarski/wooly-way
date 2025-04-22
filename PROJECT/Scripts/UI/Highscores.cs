using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay.UI
{
    public partial class Highscores : VBoxContainer
	{
        private const string LEADERBOARD_CHARACTERS = ") ";
		private const string NAME_CHARACTERS = ": ";
		private const string SPACE_CHARACTERS = "\n";
		private const string PATH_LABELSETTING = "res://Resources/Theme/Text.tres";
		private const int MAX_LEADERBOARDS = 11;

        Dictionary<string, uint> scores = new Dictionary<string, uint>();

        public override void _Ready()
		{
			SortScores();
		}
		
		private void SortScores()
		{
			scores = DataManager.GetInstance().GetAllUsersTotalScore();
			scores = scores.OrderByDescending(sorting => sorting.Value).ToDictionary(sorting => sorting.Key, sorting => sorting.Value);
			Display();
        }

		private void Display()
        {

            int lIndex = 1;
			Label lCurrentLabel = new Label();

            lCurrentLabel.Text = DataManager.LoggedUser + NAME_CHARACTERS + scores[DataManager.LoggedUser];
            lCurrentLabel.LabelSettings = ResourceLoader.Load<LabelSettings>(PATH_LABELSETTING);
            AddChild(lCurrentLabel);

            foreach (KeyValuePair<string, uint> kvp in scores)
			{
				lCurrentLabel = new Label();
                AddChild(lCurrentLabel);

				lCurrentLabel.Text = lIndex + LEADERBOARD_CHARACTERS + kvp.Key + NAME_CHARACTERS + kvp.Value + SPACE_CHARACTERS;
				lCurrentLabel.LabelSettings = ResourceLoader.Load<LabelSettings>(PATH_LABELSETTING);
				lIndex++;
				if(lIndex == MAX_LEADERBOARDS) return;
            }
        }
	}
}
