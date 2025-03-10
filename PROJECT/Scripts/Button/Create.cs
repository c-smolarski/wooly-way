using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;
using System.Formats.Asn1;
using System.Security.Principal;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class Create : UserButton
	{

        public override void _Ready()
		{
			base._Ready();
		}

        /// <summary>
        /// Creates an account based on given info
        /// </summary>
        protected override void OnPressed()
		{
			Object[] newUser;
			username = usernameText.Text;
            (password, salt) = passwordManager.Encryption(passwordText.Text);
			newUser = dataManager.CreateUser(username, password, salt);
			if ((bool)newUser[0])
			{
				GD.Print("account created loading next scene");
				transition.Play(TRANSITION);
				Transition.shake = true;
				transition.AnimationFinished += (StringName pName) => ChangeScene(pName);
			}
			else errorMessage.Text = (string)newUser[1];
		}

		private void ChangeScene(string pName)
		{
			GetTree().ChangeSceneToFile(NEXT_SCENE);
		}
	}
}
