using Godot;
using System;
using System.Formats.Asn1;
using System.Text;

	//Author: Alissa Delattre
namespace Com.IsartDigital.ProjectName
{
	public partial class PasswordManager : Node
	{
		#region Singleton
		static private PasswordManager instance;

		private PasswordManager() { }

		static public PasswordManager GetInstance()
		{
			if (instance == null) instance = new PasswordManager();
			return instance;
		}

		#endregion

		private const string ASCII = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private const int LENTGH_SALT = 16;
		private const int LEFT_SHIFT = 5;
		private const int RIGHT_SHIFT = 27;
        private uint hashValue = 0x811C9DC5;
		private uint primeValue = 0x01000193;

        private RandomNumberGenerator rand = new RandomNumberGenerator();

        public override void _Ready()
		{
            rand.Randomize();

            #region Singleton
            if (instance != null)
			{
				QueueFree();
				GD.Print(nameof(PasswordManager) + "Instance already exists, destroying the last added");
				return;
			}

			instance = this;
			#endregion
		}

        /// <summary>
        /// call function to crypt a password ans s end it back crypted with its salt
        /// </summary>
        public (uint, string) Crypting(string pPassword)
		{	
			string lSalt = SaltGenerator();
			uint lResult = Crypting(pPassword, lSalt);
            return (lResult, lSalt);
        }

        /// <summary>
        /// Crypt the password using a custom hashing method inspired by FNV-1a
        /// </summary>
        private uint Crypting(string pPassword, string pSalt)
		{
			uint lHash = hashValue;
			uint lPrime = primeValue;
            string pResult = pPassword + pSalt;
			foreach(char character in pResult)
			{
				lHash ^= character;
				lHash *= lPrime;
                lHash = (lHash << LEFT_SHIFT) | (lHash >> RIGHT_SHIFT);
            }
			return lHash;
        }

        /// <summary>
        /// Checks if a password sent corresponds to the crypted one saved in the dataBase
        /// </summary>
        public bool CheckPassword(string pPassword, string pPasswordToCheck, string pSalt)
		{
			uint lCryptPassword = Crypting(pPasswordToCheck, pSalt);
			GD.Print(pSalt, pPasswordToCheck);
			return lCryptPassword.ToString() == pPassword;
		}

        /// <summary>
        /// Generates a random list of asscii characters that are added at the end of the password to add safety
        /// </summary>
        private string SaltGenerator()
		{
			string lSalt = "";
			int lAsciiPossibility = ASCII.Length - 1;
			for (int i = 0; i < LENTGH_SALT; i++)
			{
				lSalt += ASCII[rand.RandiRange(0, lAsciiPossibility)];
			}
			return lSalt;
        }

		protected override void Dispose(bool pdisposing)
		{
			instance = null;
			base.Dispose(pdisposing);
		}
	}
}
