using Godot;
using System;
using System.Reflection.Emit;
using System.Xml.Serialization;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class Login : Button
	{
        [Export] private LineEdit usernameText;
        [Export] private LineEdit passwordText;
		[Export] private AnimationPlayer transition;

        private string username;
        private string password;
        private string salt;

        PasswordManager passwordManager;

        public override void _Ready()
		{
			passwordManager = PasswordManager.GetInstance();
			Pressed += OnPressed;
		}
		
		private void OnPressed()
		{
			username = usernameText.Text;
			password = passwordText.Text;

			transition.Play("fade In");
            transition.AnimationFinished += (StringName pName) => ChangeScene(pName);


            //(string pass, salt) = DataManager.CheckIfUserExist(username);
            //if(!pass == null && passwordManager.CheckPassword(pass, password, salt)){
            //  DataManager.NonDuTrucQuiStockLeCompteDansLequelTesActuellementConnecter = username;
            //	GetTree().ChangeSceneToFile(chemin vers le level selector)
            //else GD.Print("wrong username or password")
        }

        private void ChangeScene(string pName)
        {
            GetTree().ChangeSceneToFile("res://Scenes/Tile.tscn");
        }

    }
}
