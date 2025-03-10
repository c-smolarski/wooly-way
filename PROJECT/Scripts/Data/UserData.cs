using System.Collections.Generic;

// author : DUCROQUET Clément

namespace Com.IsartDigital.WoolyWay.Data
{
	public class UserData
	{
        //User log in data
        public string username;
        public string hachedPassword;
        public string passwordSalt;

        //User statistics data
        public List<bool> unlockedLevels;
        public List<uint> levelScores;
    }
}
