using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class UserButton : Button
	{
        protected PasswordManager passwordManager;
        protected UserManager userManager;

        [Export] protected LineEdit usernameText;
        [Export] protected LineEdit passwordText;
        [Export] protected Label errorMessage;
        [Export] protected AnimationPlayer transition;


        protected string username;
        protected string password;
        protected string salt;

        protected const string TRANSITION = "fade In";
        protected const string NEXT_SCENE = "res://Scenes/Main.tscn";
        public override void _Ready()
		{
            Pressed += OnPressed;
            passwordManager = PasswordManager.GetInstance();
            userManager = UserManager.GetInstance();
        }

        protected virtual void OnPressed()
        {
        }
	}
}
