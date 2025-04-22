using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.AnimatedButtons.MenuButtons.UserButtons
{
	public partial class Create : UserButton
	{
        private const string BANNED_CHAR = ";/:%*£$=+°)@]^_`-|[({'#&§àéèç²ù,";
        private const string BANNED_CHAR_USERNAME = "Username contains banned character";

        /// <summary>
        /// Creates an account based on given info
        /// </summary>
        protected override void OnPressed()
		{
			if(usernameText.Text.Length == 0 || passwordText.Text.Length == 0 ) return;
			username = usernameText.Text;
			if (!IsUsernameValid(username))
			{
				errorMessage.Text = BANNED_CHAR_USERNAME;
				return;
            }
            (password, salt) = passwordManager.Encryption(passwordText.Text);
            Object[] lNewUser = dataManager.CreateUser(username, password, salt);
			if ((bool)lNewUser[0]) base.OnPressed();
			else errorMessage.Text = (string)lNewUser[1];
		}

		private bool IsUsernameValid(string pUsername)
		{
			foreach(char character in BANNED_CHAR)
			{
                if (pUsername.Contains(character)) return false;
            }
			return true;
		}
	}
}
