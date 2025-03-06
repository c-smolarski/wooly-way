using Com.IsartDigital.WoolyWay.Utils.Data;
using Godot;
using System.Collections.Generic;

// author : Moussouni-lepilliez Daniel

namespace Com.IsartDigital.WoolyWay.Managers
{
    public partial class SoundManager : Node
    {
        #region Singleton
        static private SoundManager instance;

        private SoundManager() { }

        static public SoundManager GetInstance()
        {
            if (instance == null) instance = new SoundManager();
            return instance;
        }

        #endregion

        [Export] AudioStreamPlayer MusicPlayer;
        [Export] AudioStream bossMusic;
        public float percentageVolumeCache = 100f;

        private List<AudioStreamPlayer> playersList = new();

        public override void _Ready()
        {
            #region Singleton

            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(SoundManager) + "Instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion
        }

        public void StartMusic(string pSoundPath)
        {
            AudioStreamOggVorbis lSound = GD.Load<AudioStreamOggVorbis>(pSoundPath);
            MusicPlayer.Stream = lSound;
            MusicPlayer.Play();
        }

        public float GetVolumePercentage(string pBusName)
        {
            int lBusIndex = AudioServer.GetBusIndex(pBusName);
            float lDecibels = AudioServer.GetBusVolumeDb(lBusIndex);
            return Mathf.Pow(10, lDecibels / 20) * 100;
        }

        public void SetVolumeFromPercentage(float pPercentage, string pBusName)
        {
            pPercentage = Mathf.Clamp(pPercentage, 0, 100);
            float lDecibels = 20 * (Mathf.Log(pPercentage / 100) / Mathf.Log(10));
            lDecibels = Mathf.Max(lDecibels, -80);
            int lBusIndex = AudioServer.GetBusIndex(pBusName);
            AudioServer.SetBusVolumeDb(lBusIndex, lDecibels);
        }

        public AudioStreamPlayer Play(string pSoundPath, bool pIsLooping = false)
        {
            AudioStreamOggVorbis lSound = GD.Load<AudioStreamOggVorbis>(pSoundPath);
            AudioStreamPlayer lPlayer = new();
            lSound.Loop = pIsLooping;
            lPlayer.Stream = lSound;
            lPlayer.Bus = SoundPath.MAIN_SOUND_BUS;
            playersList.Add(lPlayer);
            lPlayer.Finished += () => isPlayerFinished(lPlayer);
            lSound.Loop = pIsLooping;
            AddChild(lPlayer);
            lPlayer.Play();
            return lPlayer;
        }

        private void isPlayerFinished(AudioStreamPlayer pPlayer)
        {
            pPlayer.QueueFree();
        }

        public void PauseAllSounds()
        {
            foreach (AudioStreamPlayer lPlayer in playersList)
            {
                lPlayer.StreamPaused = true;
            }
        }

        public void ResumeAllSounds()
        {
            foreach (AudioStreamPlayer lPlayer in playersList)
            {
                lPlayer.StreamPaused = false;
            }
        }

        protected override void Dispose(bool pDisposing)
        {
            if (instance == this) instance = null;
            base.Dispose(pDisposing);
        }

    }
}
