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

			GD.Print("CreateUser");
			CreateUser(new Dictionary<string, List<string>>() { { "usernameblabla", new List<string>() { "thispassword", "thissalt" } } });
		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
		}

		/// <summary>
		/// Stores log in information of a user into a json file.
		/// </summary>
		/// <param name="pUserData">A dictionary of which the key matches the username, and the value is a List containing both hached password and password salt.</param>
		public void CreateUser(Dictionary<string, List<string>> pUserData)
		{
            #region DECLARATIONS
            JsonSerializerOptions lJsonOptions;
			string lUserJsonData;
			string lGlobalJsonData;
			List<UserData> lAllUsersData;
			
			UserData lNewUser = new UserData();
            #endregion

            foreach (KeyValuePair<string, List<string>> lData in pUserData)
			{
                lNewUser.username = lData.Key;
				lNewUser.hachedPassword = lData.Value[0];
				lNewUser.passwordSalt = lData.Value[1];
            }

            lJsonOptions = new JsonSerializerOptions();
            lJsonOptions.IncludeFields = true; //Default is false. If false, UserData fields are not included into the json serialization and cause the class instance to be empty.
            lJsonOptions.WriteIndented = true;

			lUserJsonData = JsonSerializer.Serialize(lNewUser, lJsonOptions); //Makes the class instance a json object.

			lAllUsersData = JsonSerializer.Deserialize<List<UserData>>(GetAllUsersData(userDataPath), lJsonOptions); //Makes the json object(s) a UserData class instance.
			lAllUsersData.Add(lNewUser);

			lGlobalJsonData = JsonSerializer.Serialize(lAllUsersData, lJsonOptions);

            OverwriteJsonFile(userDataPath, lGlobalJsonData);
        }

		/// <summary>
		/// Returns data from a file according to its path (pPathOfFileToRead) as a json serialized string value.
		/// </summary>
		/// <param name="pPathOfFileToRead">Path of the file to be open and read. Returned value will be equal to the content of this file.</param>
		/// <returns></returns>
		private string GetAllUsersData(string pPathOfFileToRead)
		{
			string lData;
			lData = File.ReadAllText(pPathOfFileToRead);

			return lData;
		}

        /// <summary>
        /// Overwrites a file according to its path (pPathOfFileToOverwrite) with pTextToWrite data.
        /// </summary>
        /// <param name="pPathOfFileToOverwrite">Path of the file to be open and overwritten.</param>
        /// <param name="pTextToWrite">Text to write into the file. Former data will be automatically deleted if not included into this param.</param>
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
