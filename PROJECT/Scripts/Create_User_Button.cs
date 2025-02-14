using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
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
