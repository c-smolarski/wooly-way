using Com.IsartDigital.WoolyWay.Scripts.Utils;
using Godot;
using Godot.Collections;
using System;
using System.Text.Json;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class Login_Button : Button
	{
        [Export] private TextEdit usernameText;
        [Export] private TextEdit passwordText;
        public override void _Ready()
		{
            Pressed += Test;
		}
		
        private void Test()
        {
        }

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
		}

		private void CheckUser()
		{

		}

		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
