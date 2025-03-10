using Godot;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

using Com.IsartDigital.WoolyWay.Data;

// author : DUCROQUET Clément

namespace Com.IsartDigital.WoolyWay.Managers
{
	public partial class DataManager : Node
	{
        #region EXPORTS

        [Export] private string userDataPath = "../PROJECT/Resources/Data/userData.json";
        [Export] private string userAlreadyExistsErrorText = "Impossible to perform this action. Username already present in data base.";
        [Export] private string userNotFoundErrorText = "Impossible to perform this action. Invalid username.";
        #endregion

        #region DATA
        private JsonSerializerOptions globalJsonOptions;

        private List<UserData> globalData;
        private UserData userData;

        private uint levelCount;

        public string LoggedUser { get; private set; }
        public List<bool> UserUnlockedLevels { get; private set; }
        public List<uint> UserLevelScores { get; private set; }

        #endregion

        //TO REMOVE!
        uint NumLevel() //To delete: waiting for MapManager method to replace this one.
        {
            return 7; //(just for testing purposes)
        }

        #region Singleton

        static private DataManager instance;

		private DataManager() { }

		static public DataManager GetInstance()
		{
			if (instance == null) instance = new DataManager();
			return instance;
		}

        #endregion

        public override void _Ready()
		{
			#region Singleton

			if (instance != null)
			{
				QueueFree();
				GD.PrintErr(nameof(DataManager) + "Instance already exists, destroying the last added.");
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

            GD.Print("Log in User");
            LogIntoUserAccount("usernameblabla");

            GD.Print("UpdateUserStats");
            UpdateUserStats(3, 1500, 1);
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

            string lUpdatedData;

            bool lSuccess = default;
            UserData lNewUser = new UserData();

            #endregion

            globalData = GetUserData();

			if (!CheckIfUserExists(pUserName, globalData))
			{
                //User log in data
                lNewUser.username = pUserName;
                lNewUser.hachedPassword = pHachedPassword;
                lNewUser.passwordSalt = pPasswordSalt;

                if (levelCount == default) levelCount = NumLevel(); //Optimization (better than calling multiple time the exact same function)

                //User statistics data
                lNewUser.unlockedLevels = GetDefaultUnlockedLevels(levelCount);
                lNewUser.levelScores = GetDefaultLevelScores(levelCount);

                globalData.Add(lNewUser);
                lUpdatedData = JsonSerializer.Serialize(globalData, globalJsonOptions);

                OverwriteJsonFile(lUpdatedData);

                LogIntoUserAccount(pUserName, levelCount);

				lSuccess = true;
            }

            return lSuccess ? new object[1] { lSuccess } : new object[2] { lSuccess, userAlreadyExistsErrorText };
        }

        /// <summary>
        /// Log user by properly setting LoggedUser property. Also loads user data into public fields of this class.
        /// </summary>
        /// <param name="pUserName"></param>
        public void LogIntoUserAccount(string pUserName, uint pNumLevel = default)
        {
            object[] lUserStats = GetUserStats(pUserName);
            if (lUserStats == null) return;

            levelCount = pNumLevel != default ? pNumLevel : NumLevel();
            LoggedUser = pUserName;

            UserStatsSizeAdaptator(lUserStats);

            UserUnlockedLevels = (List<bool>)lUserStats[0];
            UserLevelScores = (List<uint>)lUserStats[1];
        }

        /// <summary>
        /// Checks if pUserName is found throughout a group of data and returns a boolean representing the process success or failure.
        /// </summary>
        /// <param name="pUserName"></param>
        private bool CheckIfUserExists(string pUserName, List<UserData> pGlobalData)
        {
            bool lSuccess = default;

            foreach (UserData lData in pGlobalData)
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
        /// Generates default data for UserData.unlockedLevels field.
        /// </summary>
        /// <param name="pLength"></param>
        /// <returns></returns>
        private List<bool> GetDefaultUnlockedLevels(uint pLength)
        {
            List<bool> lDefaultUnlockedLevels = new List<bool>();

            for (int lLevelIndex = 0; lLevelIndex < pLength; lLevelIndex++)
            {
                if (lLevelIndex == 0) lDefaultUnlockedLevels.Add(true);
                else lDefaultUnlockedLevels.Add(default);
            }

            return lDefaultUnlockedLevels;
        }

        /// <summary>
        /// Generates default data for UserData.levelScores field.
        /// </summary>
        /// <param name="pLength"></param>
        /// <returns></returns>
        private List<uint> GetDefaultLevelScores(uint pLength)
        {
            List<uint> lDefaultLevelScores = new List<uint>();

            for (int lLevelIndex = 0; lLevelIndex < pLength; lLevelIndex++) lDefaultLevelScores.Add(default);

            return lDefaultLevelScores;
        }

        /// <summary>
        /// Returns json serialized data from a file as a list of UserData.
        /// </summary>
        /// <returns></returns>
        private List<UserData> GetUserData()
        {
            string lSerializedData;
            lSerializedData = File.ReadAllText(userDataPath);

            globalData = JsonSerializer.Deserialize<List<UserData>>(lSerializedData, globalJsonOptions); //Makes the json object(s) a UserData class instance (or list of instances).

            return globalData;
        }

        /// <summary>
        /// Returns the unique instance of UserData matching the specified pUserName, otherwise returs null.
        /// </summary>
        /// <param name="pUserName"></param>
        /// <returns></returns>
        private UserData GetUserData(string pUserName)
        {
            List<UserData> lFileData = GetUserData();

            foreach (UserData lData in lFileData) if (lData.username == pUserName) return lData;
            return null;
        }

        /// <summary>
        /// Returns an array containing a boolean representing success/failure and (if pUserName exists) another array constituted of both hached password and password salt.
        /// </summary>
        /// <param name="pUserName"></param>
        public object[] GetSecuredDataIfExists(string pUserName)
        {
            userData = GetUserData(pUserName);

            if (userData == null) return new object[2] { false, userNotFoundErrorText };
            return new object[2] { true, new string[2] { userData.hachedPassword, userData.passwordSalt } };
        }

        /// <summary>
        /// Returns an array containing user statistics.
        /// </summary>
        /// <param name="pUserName"></param>
        /// <returns></returns>
        private object[] GetUserStats(string pUserName)
        {
            userData = GetUserData(pUserName);

            if (userData == null) return null;
            return new object[2] { userData.unlockedLevels, userData.levelScores };
        }

        /// <summary>
        /// Refreshes default user stats according to current level number (in case a level has been added/removed after a project update). Current stored data will be safe except data linked with removed levels.
        /// </summary>
        /// <param name="pUserStats"></param>
        private void UserStatsSizeAdaptator(object[] pUserStats)
        {
            int lItemsCount = ((List<bool>)pUserStats[0]).Count;

            if (lItemsCount < levelCount)
            {
                for (int lLevelIndex = lItemsCount - 1; lLevelIndex < levelCount; lLevelIndex++)
                {
                    ((List<bool>)pUserStats[0]).Add(default);
                    ((List<bool>)pUserStats[1]).Add(default);
                }
            }
            else if (lItemsCount > levelCount)
            {
                for (int lLevelIndex = lItemsCount - 1; lLevelIndex > levelCount; lLevelIndex--)
                {
                    ((List<bool>)pUserStats[0]).RemoveAt(lLevelIndex);
                    ((List<bool>)pUserStats[1]).RemoveAt(lLevelIndex);
                }
            }
        }

        /// <summary>
        /// Refreshes user statistics after a round is over. Only data higher than stored data will be taken into accound.
        /// </summary>
        /// <param name="pLevel"></param>
        /// <param name="pScore"></param>
        /// <param name="pStars"></param>
        public void UpdateUserStats(int pLevel, uint pScore, uint pStars)
        {
            #region DECLARATIONS

            string lUpdatedData;

            uint lNextLevel = (uint)pLevel + 1;

            #endregion

            globalData = GetUserData();

            foreach (UserData lData in globalData)
            {
                if (lData.username == LoggedUser)
                {
                    if (pScore > lData.levelScores[pLevel]) lData.levelScores[pLevel] = pScore;
                    if (lData.unlockedLevels[pLevel + 1] == default && pStars != 0) lData.unlockedLevels[pLevel + 1] = true;

                    break;
                }
            }

            lUpdatedData = JsonSerializer.Serialize(globalData, globalJsonOptions);

            OverwriteJsonFile(lUpdatedData);
        }

        /// <summary>
        /// Overwrites a file with pTextToWrite data.
        /// </summary>
        /// <param name="pTextToWrite">Text to write into the file. Former data will be automatically deleted if not included into this param.</param>
        private void OverwriteJsonFile(string pTextToWrite)
		{
			File.WriteAllText(userDataPath, pTextToWrite);
        }

        protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}
	}
}
