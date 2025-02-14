using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class Create : Button
	{
		PasswordManager passwordManager;

		[Export] private TextEdit usernameText;
		[Export] private TextEdit passwordText;

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
			//TODO verifier si le username existe deja
			username = usernameText.Text;
			(password, salt) = passwordManager.Crypting(passwordText.Text);
		}

		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
