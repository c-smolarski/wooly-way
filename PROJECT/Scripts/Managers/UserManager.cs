using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

using Com.IsartDigital.WoolyWay.Data;
using Com.IsartDigital.WoolyWay.Utils.Converters;
using Com.IsartDigital.WoolyWay.Utils.Data;

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

		//[Export] Resource userDataFile;
		private string userDataPath = "../PROJECT/Ressources/Data/userData.json";

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

			//userDataPath = userDataFile.ResourcePath; // convert into string -> remove root (as string) -> ../{path}

			/*GD.Print("CreateUser");
			CreateUser(new Dictionary<string, List<string>>() { { "usernameblabla", new List<string>() { "thispassword", "thissalt" } } });*/
		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
		}

		public void CreateUser(Dictionary<string, List<string>> pUserData)
		{
			JsonSerializerOptions lJsonOptions;
			string lUserJsonData; string lJsonData;
			List<UserData> lAllUsersData;
			
			UserData lNewUser = new UserData();

            foreach (KeyValuePair<string, List<string>> lData in pUserData)
			{
                lNewUser.username = lData.Key;
				lNewUser.hachedPassword = lData.Value[0];
				lNewUser.passwordSalt = lData.Value[1];
            }

            lJsonOptions = new JsonSerializerOptions();
            lJsonOptions.IncludeFields = true;
			lJsonOptions.WriteIndented = true;

			lUserJsonData = JsonSerializer.Serialize(lNewUser, lJsonOptions);

			lAllUsersData = JsonSerializer.Deserialize<List<UserData>>(GetAllUsersData(userDataPath), lJsonOptions);
			lAllUsersData.Add(lNewUser);

			lJsonData = JsonSerializer.Serialize(lAllUsersData, lJsonOptions);

            OverwriteJsonFile(userDataPath, lJsonData);
        }

		private string GetAllUsersData(string pPathOfFileToRead)
		{
			string lData;
			lData = File.ReadAllText(pPathOfFileToRead);

			return lData;
		}

		private void OverwriteJsonFile(string pPathOfFileToOverwrite, string pTextToWrite)
		{
			File.WriteAllText(pPathOfFileToOverwrite, pTextToWrite);
        }

        protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}
	}
}
