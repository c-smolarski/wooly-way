using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;
using System.Formats.Asn1;
using System.Security.Principal;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class Create : Button
	{
		PasswordManager passwordManager;
		UserManager userManager;

		[Export] private LineEdit usernameText;
		[Export] private LineEdit passwordText;
        [Export] private Label errorMessage;
		private string error = "Username already exist";

        private string username;
		private uint password;
		private string salt;

		
		public override void _Ready()
		{
			passwordManager = PasswordManager.GetInstance();
			userManager = UserManager.GetInstance();
			Pressed += OnPressed;
		}

        /// <summary>
        /// Creates an account based on given info
        /// </summary>
        private void OnPressed()
		{
			Object[] isConnected;
			username = usernameText.Text;
            (password, salt) = passwordManager.Crypting(passwordText.Text);
			isConnected = userManager.CreateUser(username, password.ToString(), salt);
			if ((bool)isConnected[0])
			{
				GD.Print("account created loading next scene");
			}
			else errorMessage.Text = error;
		}
	}
}
