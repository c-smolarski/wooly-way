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

        private string username;
		private string password;
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
			Object[] newUser;
			username = usernameText.Text;
            (password, salt) = passwordManager.Crypting(passwordText.Text);
			newUser = userManager.CreateUser(username, password, salt);
			if ((bool)newUser[0])
			{
				GD.Print("account created loading next scene");
			}
			else errorMessage.Text = (string)newUser[1];
		}
	}
}
