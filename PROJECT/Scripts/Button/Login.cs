using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;
using System.Reflection.Emit;
using System.Xml.Serialization;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName
{

    public partial class Login : UserButton
    {
        private string errorPassword = "Wrong Password";
        public override void _Ready()
        {
            base._Ready();
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
                Object[] lProtectedData = (object[])lDataUser[1];
                if (passwordManager.CheckPassword((string)lProtectedData[0], password, (string)lProtectedData[1]))
                {

                    dataManager.LogIntoUserAccount(username);
                    GD.Print("logged in");
                    transition.Play(TRANSITION);
                    Transition.shake = true;
                    transition.AnimationFinished += (StringName pName) => ChangeScene(pName);
                }
                else
                {
                    errorMessage.Text = errorPassword;
                }
            }
            else
            {
                errorMessage.Text = (string)lDataUser[1];
            }
        }

        private void ChangeScene(string pName)
        {
            GetTree().ChangeSceneToFile(NEXT_SCENE);
        }
    }
}
