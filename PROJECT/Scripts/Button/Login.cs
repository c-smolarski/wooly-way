using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;
using System.Reflection.Emit;
using System.Xml.Serialization;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName
{

    public partial class Login : Button
    {
        [Export] private LineEdit usernameText;
        [Export] private LineEdit passwordText;
        [Export] private AnimationPlayer transition;
        [Export] private Godot.Label errorMessage;

        private string errorPassword = "Wrong Password";

        private string username;
        private string password;
        private string salt;

        private const string TRANSITION = "fade In";
        private const string NEXT_SCENE = "res://Scenes/Tile.tscn";

        PasswordManager passwordManager;
        UserManager userManager;

        public override void _Ready()
        {
            passwordManager = PasswordManager.GetInstance();
            userManager = UserManager.GetInstance();
            Pressed += OnPressed;
        }

        /// <summary>
        /// Get the entered user data, checks if the user exist and check if the password is right then logs you in
        /// </summary>
        private void OnPressed()
        {
            username = usernameText.Text;
            password = passwordText.Text;

            Object[] lDataUser = userManager.GetSecuredDataIfExists(username);
            if ((bool)lDataUser[0])
            {
                Object[] lProtectedData = (object[])lDataUser[1];
                if (passwordManager.CheckPassword((string)lProtectedData[0], password, (string)lProtectedData[1]))
                {

                    userManager.LogIntoUserAccount(username);
                    GD.Print("logged in");
                    transition.Play(TRANSITION);
                    Transition.shake = true;
                    //transition.AnimationFinished += (StringName pName) => ChangeScene(pName);
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
