using Godot;
using System;
using System.Formats.Asn1;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class Create : Button
	{
		PasswordManager passwordManager;

		[Export] private LineEdit usernameText;
		[Export] private LineEdit passwordText;

		private string username;
		private uint password;
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
