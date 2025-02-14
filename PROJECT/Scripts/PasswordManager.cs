using Godot;
using System;
using System.Text;

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

        private RandomNumberGenerator ran = new RandomNumberGenerator();

        public override void _Ready()
		{
            ran.Randomize();

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

		public (string, string) Crypting(string pPassword)
		{
			pPassword = pPassword.Sha256Text();
			string salt = SaltGenerator();
			pPassword += salt;
			return (pPassword, salt);
		}

		private string SaltGenerator()
		{
			string salt = "";
			for (int i = 0; i < LENTGH_SALT; i++)
			{
				salt += ASCII[ran.RandiRange(0, ASCII.Length - 1)];
			}
			return salt.Sha256Text();
        }

		protected override void Dispose(bool pdisposing)
		{
			instance = null;
			base.Dispose(pdisposing);
		}
	}
}
