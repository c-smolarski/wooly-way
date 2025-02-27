using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

using Com.IsartDigital.WoolyWay.Data;

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

		[Export] private string userDataPath = "../PROJECT/Resources/Data/userData.json";
		[Export] private string userAlreadyExistsErrorText = "Impossible to perform this action. Username already present in data base.";
        [Export] private string userNotFoundErrorText = "Impossible to perform this action. Invalid username.";

		public string LoggedUser { get; private set; }

        JsonSerializerOptions globalJsonOptions;

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

            globalJsonOptions = new JsonSerializerOptions();
            globalJsonOptions.IncludeFields = true; //Default is false. If false, UserData fields are not included into the json serialization and cause the class instance to be empty.
            globalJsonOptions.WriteIndented = true;


            //TODO remove?
			//GD.Print("CreateUser");
			//GD.Print(CreateUser("usernameblabla", "thispassword", "thissalt"));

			//GD.Print("GetSecuredDataIfExists");
			//GD.Print((string[])GetSecuredDataIfExists("usernameblabla")[1]);

   //         GD.Print("Log in User");
   //         LogIntoUserAccount("unsernameblabla");
		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
		}

        /// <summary>
        /// Stores log in information of a user into a json file. Returns an array containing success/failure and an error message if a failure occured.
        /// </summary>
        /// <param name="pUserName"></param>
        /// <param name="pHachedPassword"></param>
        /// <param name="pPasswordSalt"></param>
        /// <returns></returns>
        public object[] CreateUser(string pUserName, string pHachedPassword, string pPasswordSalt)
		{
			#region DECLARATIONS

			string lGlobalJsonData;
			List<UserData> lGlobalData;

            bool lSuccess = default;
            object[] lSuccessData; //if lSuccess, contains true, else, contains false and an error message
			UserData lNewUser = new UserData();

            #endregion

            lNewUser.username = pUserName;
            lNewUser.hachedPassword = pHachedPassword;
            lNewUser.passwordSalt = pPasswordSalt;

            lGlobalData = JsonSerializer.Deserialize<List<UserData>>(GetAllUsersData(userDataPath), globalJsonOptions); //Makes the json object(s) a UserData class instance (or list of instances).

			if (!CheckIfUserExists(pUserName))
			{
                lGlobalData.Add(lNewUser);
                lGlobalJsonData = JsonSerializer.Serialize(lGlobalData, globalJsonOptions);

                OverwriteJsonFile(userDataPath, lGlobalJsonData);

                LogIntoUserAccount(pUserName);

				lSuccess = true;
            }

			lSuccessData = lSuccess ? new object[1] { lSuccess } : new object[2] { lSuccess, userAlreadyExistsErrorText };
            return lSuccessData;
        }

        /// <summary>
        /// Log user by properly setting LoggedUser property. Also starts the user data loading process from DataManager.
        /// </summary>
        /// <param name="pUserName"></param>
        public void LogIntoUserAccount(string pUserName)
        {
            LoggedUser = pUserName;
            GD.Print(LoggedUser);
            GD.Print("DataManager: Loading user data!"); //Data Manager will then load real user data once implemented.
        }

        /// <summary>
        /// Checks if the data (pUserName) is found throughout a group of data and returns a boolean representing the process success or failure.
        /// </summary>
        /// <param name="pUserName"></param>
        private bool CheckIfUserExists(string pUserName)
        {
            List<UserData> lGlobalData;

            bool lSuccess = default;

            lGlobalData = JsonSerializer.Deserialize<List<UserData>>(GetAllUsersData(userDataPath), globalJsonOptions);

            foreach (UserData lData in lGlobalData)
            {
                if (lData.username == pUserName)
                {
                    lSuccess = true;
                    break;
                }
            }

            return lSuccess;
        }

        /// <summary>
        /// Checks all usernames stored and returns an array containing both a boolean and (if pUserName exists) another array constituted of both hached password and password salt.
        /// </summary>
        /// <param name="pUserName"></param>
        public object[] GetSecuredDataIfExists(string pUserName)
		{
			List<UserData> lGlobalData;

            bool lSuccess = default;
			object[] lUserData; ; //Array supposed to contain lSuccess and an array containing both hached password and password salt if they exist.
			string[] lSecuredData = new string[] { };

            lGlobalData = JsonSerializer.Deserialize<List<UserData>>(GetAllUsersData(userDataPath), globalJsonOptions);

			foreach (UserData lData in lGlobalData)
			{
				if (lData.username == pUserName)
				{
					lSecuredData = new string[2] { lData.hachedPassword, lData.passwordSalt };
                    lSuccess = true;

                    break;
				}
			}

			lUserData = new object[2] { lSuccess, lSuccess ? lSecuredData : userNotFoundErrorText };
            return lUserData;
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
