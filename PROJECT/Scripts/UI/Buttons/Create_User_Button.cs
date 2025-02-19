using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay
{
	
	public partial class Create_User_Button : Button
	{
        [Export] private TextEdit usernameText;
        [Export] private TextEdit passwordText;

        private string username;
		private string password;

        public override void _Ready()
		{

		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

		}

		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
