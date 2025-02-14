using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class Login : Button
	{
        [Export] private TextEdit usernameText;
        [Export] private TextEdit passwordText;

        private string username;
        private string password;
        private string salt;

        PasswordManager passwordManager;

        public override void _Ready()
		{
			passwordManager = PasswordManager.GetInstance();
			Pressed += OnPressed;
		}
		
		private void OnPressed()
		{
			username = usernameText.Text;
			password = passwordText.Text;

			//(string pass, salt) = DataManager.CheckIfUserExist(username);
			//if(!pass == null && passwordManager.CheckPassword(pass, password, salt)){
			//  DataManager.NonDuTrucQuiStockLeCompteDansLequelTesActuellementConnecter = username;
			//	GetTree().ChangeSceneToFile(chemin vers le level selector)
			//else GD.Print("wrong username or password")
		}
	}
}
