using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.AnimatedButtons.MenuButtons.UserButtons
{
    public partial class Login : UserButton
    {
        private const string ERROR_PASSWORD = "Wrong password.";

        public override void _Input(InputEvent pEvent)
        {
            base._Input(pEvent);
            if(Input.IsKeyPressed(Key.Enter) || Input.IsKeyPressed(Key.KpEnter))
                OnPressed();
        }

        /// <summary>
        /// Get the entered user data, checks if the user exist and check if the password is right then logs you in
        /// </summary>
        protected override void OnPressed()
        {
            username = usernameText.Text;
            password = passwordText.Text;

            Object[] lDataUser = dataManager.GetSecuredDataIfExists(username);
            if ((bool)lDataUser[0])
            {
                string[] lProtectedData = (string[])lDataUser[1];
                if (passwordManager.CheckPassword(lProtectedData[0], password,lProtectedData[1]))
                {
                    dataManager.LogIntoUserAccount(username);
                    base.OnPressed();
                }
                else errorMessage.Text = ERROR_PASSWORD;

            }
            else errorMessage.Text = (string)lDataUser[1];
        }
    }
}
