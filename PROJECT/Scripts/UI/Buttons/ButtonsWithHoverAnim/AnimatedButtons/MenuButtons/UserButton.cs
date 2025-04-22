using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.AnimatedButtons.MenuButtons
{
	public abstract partial class UserButton : MenuButton
	{
        protected PasswordManager passwordManager;

        [Export] protected LineEdit usernameText;
        [Export] protected LineEdit passwordText;
        [Export] protected Label errorMessage;

        protected DataManager dataManager;

        protected string username;
        protected string password;
        protected string salt;

        
        public override void _Ready()
		{
            base._Ready();
            passwordManager = PasswordManager.GetInstance();
            dataManager = DataManager.GetInstance();
        }
	}
}
