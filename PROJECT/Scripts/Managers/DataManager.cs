using Com.IsartDigital.WoolyWay.Data;
using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;
using FileAccess = Godot.FileAccess;

// author : DUCROQUET Clément && Moussouni--Lepilliez Daniel (small changes)

namespace Com.IsartDigital.WoolyWay.Managers
{

    public partial class DataManager : Node
    {
        static DataManager()
        {
            if (!FileAccess.FileExists(USER_DATA_PATH))
            {
                DirAccess lDir = DirAccess.Open("user://");
                lDir.MakeDir("Data");
                OverwriteJsonFile("[]");
            }
        }

        #region EXPORTS
        [Export] private string userAlreadyExistsErrorText = "Username already present in data base.";
        [Export] private string userNotFoundErrorText = "Invalid username.";
        #endregion
        
        private const string USER_DATA_PATH = "user://Data/userData.json";

        public const int MAX_STAR = 3;
        private const int MID_STAR = 2;
        private const int MIN_STAR = 1;
        private const int SCORE_HIGH = 5000;
        private const int SCORE_MID = 2000;
        private const int SCORE_LOW = 1000;
        private const float SCORE_MULTIPLICATOR = 1.5f;

        #region DATA
        private JsonSerializerOptions globalJsonOptions;

        private List<UserData> globalData; //The list containing the most recent user data.
        private UserData userData; //All the data attached to a specific account (not always the logged one).

        private uint levelCount;

        public static string LoggedUser { get; private set; }
        public static List<bool> UserUnlockedLevels { get; private set; }
        public static List<uint> UserLevelScores { get; private set; }
        #endregion

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

            globalData = GetUserData();
            SignalBus.Instance.UnlockLevel += OnLevelUnlocked;
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

            if (!CheckIfUserExists(pUserName))
            {
                //User log in data
                lNewUser.username = pUserName;
                lNewUser.hashedPassword = pHachedPassword;
                lNewUser.passwordSalt = pPasswordSalt;

                if (levelCount == default)
                    levelCount = LevelManager.GetInstance().NumLevels(); //Optimization (better than calling multiple time the exact same function)

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
            #region DECLARATIONS
            List<bool> lUnlockedLevels;
            List<uint> lLevelScores;
            #endregion

            (lUnlockedLevels, lLevelScores) = GetUserStats(pUserName);
            if (lUnlockedLevels == null) return;

            levelCount = pNumLevel != default ? pNumLevel : LevelManager.GetInstance().NumLevels();
            LoggedUser = pUserName;

            UserStatsSizeAdaptator(lUnlockedLevels, lLevelScores); //If the game files have been updated and a new level is available, stats will adapt automatically.

            UserUnlockedLevels = lUnlockedLevels;
            UserLevelScores = lLevelScores;
        }

        /// <summary>
        /// Checks if pUserName is found throughout a group of data and returns a boolean representing the process success or failure.
        /// </summary>
        /// <param name="pUserName"></param>
        private bool CheckIfUserExists(string pUserName)
        {
            bool lSuccess = default;

            foreach (UserData lData in globalData)
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
                lDefaultUnlockedLevels.Add(default);

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

            for (int lLevelIndex = 0; lLevelIndex < pLength; lLevelIndex++)
                lDefaultLevelScores.Add(default);

            return lDefaultLevelScores;
        }

        /// <summary>
        /// Returns json serialized data from a file as a list of UserData.
        /// </summary>
        /// <returns></returns>
        private List<UserData> GetUserData()
        {
            string lSerializedData = default;

            if (Godot.FileAccess.FileExists(USER_DATA_PATH))
                using (Godot.FileAccess file = Godot.FileAccess.Open(USER_DATA_PATH, Godot.FileAccess.ModeFlags.Read))
                    lSerializedData = file.GetAsText();

            globalData = JsonSerializer.Deserialize<List<UserData>>(lSerializedData, globalJsonOptions);
            return globalData ?? new List<UserData>();
        }

        /// <summary>
        /// Returns the unique instance of UserData matching the specified pUserName, otherwise returs null.
        /// </summary>
        /// <param name="pUserName"></param>
        /// <returns></returns>
        private UserData GetUserData(string pUserName)
        {
            foreach (UserData lData in globalData)
                if (lData.username == pUserName) return lData;
            return null;
        }

        /// <summary>
        /// Returns an array containing a boolean representing success/failure and (if pUserName exists) another array constituted of both hached password and password salt.
        /// </summary>
        /// <param name="pUserName"></param>
        public object[] GetSecuredDataIfExists(string pUserName)
        {
            bool lSuccess = default;

            userData = GetUserData(pUserName);

            if (userData != null) lSuccess = true;

            return lSuccess ?
                new object[2] { lSuccess, new string[2] { userData.hashedPassword, userData.passwordSalt } } :
                new object[2] { lSuccess, userNotFoundErrorText };
        }

        /// <summary>
        /// Returns an array containing user statistics.
        /// </summary>
        /// <param name="pUserName"></param>
        /// <returns></returns>
        private (List<bool>, List<uint>) GetUserStats(string pUserName)
        {
            userData = GetUserData(pUserName);

            if (userData == null) return (null, null);

            return (userData.unlockedLevels, userData.levelScores);
        }

        /// <summary>
        /// Returns a dictionary storing the total amount of points earned through all levels for each player.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, uint> GetAllUsersTotalScore()
        {
            uint lUserTotalScore;
            Dictionary<string, uint> lAllUsersTotalScore = new Dictionary<string, uint>();

            foreach (UserData lData in globalData)
            {
                lUserTotalScore = default;
                foreach (uint lLevelScore in lData.levelScores)
                    lUserTotalScore += lLevelScore;

                lAllUsersTotalScore[lData.username] = lUserTotalScore;
            }

            return lAllUsersTotalScore;
        }

        /// <summary>
        /// Refreshes default user stats according to current level number (in case a level has been added/removed after a project update). Current stored data will be safe except data linked with removed levels.
        /// </summary>
        /// <param name="pUserStats"></param>
        private void UserStatsSizeAdaptator(List<bool> pUnlockedLevels, List<uint> pLevelScores)
        {
            int lItemsCount = pUnlockedLevels.Count; //Checks for the amount of items inside of one of the given lists.

            if (lItemsCount < levelCount) //Adds
            {
                for (int lLevelIndex = lItemsCount - 1; lLevelIndex < levelCount; lLevelIndex++)
                {
                    pUnlockedLevels.Add(default);
                    pLevelScores.Add(default);
                }
            }
            else if (lItemsCount > levelCount) //Removes
            {
                for (int lLevelIndex = lItemsCount - 1; lLevelIndex > levelCount; lLevelIndex--)
                {
                    pUnlockedLevels.RemoveAt(lLevelIndex);
                    pLevelScores.RemoveAt(lLevelIndex);
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

            foreach (UserData lData in globalData)
            {
                if (lData.username == LoggedUser)
                {
                    if (pScore > lData.levelScores[pLevel - 1])
                        lData.levelScores[pLevel - 1] = pScore;
                    if (lData.unlockedLevels.Count > pLevel && lData.unlockedLevels[pLevel] == default && pStars != 0)
                        lData.unlockedLevels[pLevel] = true;

                    UserUnlockedLevels = lData.unlockedLevels;
                    UserLevelScores = lData.levelScores;

                    break;
                }
            }

            lUpdatedData = JsonSerializer.Serialize(globalData, globalJsonOptions);

            OverwriteJsonFile(lUpdatedData);
        }

        /// <summary>
        /// Used to unlock all levels at once. Specified parameter must be the amount of levels available in game.
        /// </summary>
        /// <param name="pLevelNumber"></param>
        private void OnLevelUnlocked(int pLevelNumber)
        {
            for (int i = 1; i <= pLevelNumber; i++)
            {
                if (!UserUnlockedLevels[i - 1])
                    UserUnlockedLevels[i - 1] = true;
            }

            foreach (UserData lData in globalData)
            {
                if (lData.username == LoggedUser)
                {
                    lData.unlockedLevels = UserUnlockedLevels;
                    break;
                }
            }

            string lUpdatedData = JsonSerializer.Serialize(globalData, globalJsonOptions);

            OverwriteJsonFile(lUpdatedData);
        }

        /// <summary>
        /// Overwrites a file with pTextToWrite data.
        /// </summary>
        /// <param name="pTextToWrite">Text to write into the file. Former data will be automatically deleted if not included into this param.</param>
        private static void OverwriteJsonFile(string pTextToWrite)
        {
            FileAccess lFile = FileAccess.Open(USER_DATA_PATH, FileAccess.ModeFlags.Write);
            lFile.StoreString(pTextToWrite);
            lFile.Close();
        }

        /*
         * SCORING METHODS
         */

        /// <summary>
        /// Calculates the score at the end of a level.
        /// </summary>
        public int ScoreToStar(int pScore)
        {
            int lNumStar;
            switch (pScore)
            {
                case SCORE_HIGH:
                    lNumStar = MAX_STAR;
                    break;
                case > SCORE_MID:
                    lNumStar = MID_STAR;
                    break;
                default:
                    lNumStar = MIN_STAR;
                    break;
            }
            return lNumStar;
        }

        /// <summary>
        /// calculates the score at the end of a level
        /// </summary>
        public (int, int) StepToScore(int pStep)
        {
            int lPar = LevelManager.CurrentLevel.Info.Par;
            int lScore;

            if (lPar >= pStep) lScore = SCORE_HIGH;
            else if (pStep < lPar * SCORE_MULTIPLICATOR) lScore = SCORE_MID + (int)((lPar/(float)pStep) * (SCORE_HIGH - SCORE_MID));
            else lScore = SCORE_LOW + (int)((lPar / (float)pStep) * SCORE_MULTIPLICATOR * (SCORE_MID - SCORE_LOW));

            int lNumStar = ScoreToStar(lScore);
            UpdateUserStats(LevelManager.CurrentLevel.Info.LevelName, (uint)lScore, (uint)lNumStar);
            return (lScore, lNumStar);
        }

        protected override void Dispose(bool pDisposing)
		{
            if(instance == this) instance = null;
            SignalBus.Instance.UnlockLevel -= OnLevelUnlocked;
			base.Dispose(pDisposing);
		}
	}
}
