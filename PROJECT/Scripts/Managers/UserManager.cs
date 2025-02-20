using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using Com.IsartDigital.WoolyWay.Data;
using System.Text.Json;

// author : DUCROQUET Clément

namespace Com.IsartDigital.WoolyWay.Managers
{
	public partial class UserManager : Node
	{
		#region Singleton
		static private UserManager instance;

		private UserManager() { }

		static public UserManager GetInstance()
		{
			if (instance == null) instance = new UserManager();
			return instance;
		}

		#endregion

		[Export] Resource userDataFile;

		public override void _Ready()
		{
			#region Singleton
			if (instance != null)
			{
				QueueFree();
				GD.PrintErr(nameof(UserManager) + "Instance already exists, destroying the last added.");
				return;
			}

			instance = this;
			#endregion
			GD.Print("CreateUser");
			CreateUser(new Dictionary<string, List<string>>() { { "usernameblabla", new List<string>() { "thispassword", "thissalt" } } });
		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
		}

		public void CreateUser(Dictionary<string, List<string>> pUserData)
		{
			string lUserJsonData;
			UserData lNewUser = new UserData();

			foreach (KeyValuePair<string, List<string>> data in pUserData)
			{
                lNewUser.username = data.Key;
				lNewUser.hachedPassword = data.Value[0];
				lNewUser.passwordSalt = data.Value[1];
            }

			JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
			jsonOptions.IncludeFields = true;

            lUserJsonData = JsonSerializer.Serialize(lNewUser, jsonOptions);
			GD.Print(lUserJsonData);

			GD.Print(userDataFile.ResourcePath);

			//OverwriteJsonFile(userDataFile, lUserJsonData);

		}

		/*private UserData[] GetAllUsersData(Json pFileToRead)
		{
			File.ReadAllLines();
			return new UserData[] { };
		}*/

		private void OverwriteJsonFile(Resource pFileToOverwrite, string pTextToWrite)
		{
			//File.WriteAllText(pFileToOverwrite.ResourcePath, pTextToWrite);
			//File.ReadAllText(@"\Ressources\Data\userData.json");
        }

        protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}
	}
}
