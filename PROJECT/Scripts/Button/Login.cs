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

        private void OnPressed()
        {
            username = usernameText.Text;
            password = passwordText.Text;

            //transition.Play(TRANSITION);
            //transition.AnimationFinished += (StringName pName) => ChangeScene(pName);

            Object[] lDataUser = userManager.GetSecuredDataIfExists(username);
            if ((bool)lDataUser[0])
            {
                Object[] lProtectedData = (object[])lDataUser[1];
                if (passwordManager.CheckPassword((string)lProtectedData[0], password, (string)lProtectedData[1]))
                {

                    userManager.LogIntoUserAccount(username);
                    GD.Print("logged in");
                }
                else
                {
                    GD.Print(lProtectedData[0]);
                    GD.Print("Wrong password");
                }
            }
            else
            {
                GD.Print("wrong password or something");
            }
        }

        private void ChangeScene(string pName)
        {
            GetTree().ChangeSceneToFile(NEXT_SCENE);
        }

    }
}
