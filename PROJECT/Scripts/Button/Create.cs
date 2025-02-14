using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class Create : Button
	{
		PasswordManager passwordManager;

		[Export] private TextEdit usernameText;
		[Export] private LineEdit passwordText;

		private string username;
		private string password;
		private string salt;

		
		public override void _Ready()
		{
			passwordManager = PasswordManager.GetInstance();
			Pressed += OnPressed;
		}
		
		private void OnPressed()
		{
			

			username = usernameText.Text;
            //TODO verifier si le username existe deja 
            (password, salt) = passwordManager.Crypting(passwordText.Text);

			//Todo envoyer le user mdp et salt au data manager
		}
	}
}
