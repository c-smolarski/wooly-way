using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.Data;
using Godot;
using System;
using System.Collections.Generic;

// Author : MOUSSOUNI-LEPILLIEZ Daniel && SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.Managers
{
    public partial class SoundManager : Node
    {
        #region Singleton
        static private SoundManager instance;

        private SoundManager() { }

        static public SoundManager GetInstance()
        {
            return instance;
        }

        #endregion

        #region Sounds
        [ExportCategory("Musics")]
        [Export] private AudioStreamOggVorbis musicMainMenu;
        [Export] private AudioStreamOggVorbis musicLeaderboad;
        [Export] private AudioStreamOggVorbis musicLevels;
        [ExportCategory("SFX")]
        [Export] public AudioStreamOggVorbis PopSFX { get; private set; }
        [Export] public AudioStreamOggVorbis PlopSFX { get; private set; }
        [Export] public AudioStreamOggVorbis FwopSFX { get; private set; }
        [Export] public AudioStreamOggVorbis RoosterSFX { get; private set; }
        [Export] public AudioStreamOggVorbis[] DogSFXList { get; private set; }
        [Export] public AudioStreamOggVorbis[] SheepSFXList { get; private set; }
        [ExportGroup("Ambients")]
        [Export] public AudioStreamOggVorbis DayAmbient { get; private set; }
        [Export] public AudioStreamOggVorbis ClearNightAmbient { get; private set; }
        [Export] public AudioStreamOggVorbis RainyNightAmbient { get; private set; }
        [Export] public AudioStreamOggVorbis SnowAmbient { get; private set; }
        [ExportGroup("Footsteps")]
        [Export] private AudioStreamOggVorbis[] grassyFootsteps;
        [Export] private AudioStreamOggVorbis[] snowyFootsteps;
        [ExportGroup("UI")]
        [Export] public AudioStreamOggVorbis UISwitchOn { get; private set; }
        [Export] public AudioStreamOggVorbis UISwitchOff { get; private set; }
        [Export] public AudioStreamOggVorbis[] UIButtonClick { get; private set; }
        #endregion

        public AudioStreamOggVorbis[] FootstepsSFX { get; private set; }
        public AudioStreamPlayer UITransitionPlayer { get; private set; }

        public static AudioStreamPlayer MusicPlayer { get; private set; }

        public float percentageVolumeCache = 100f;
        private AudioTransitioner ambientTransitioner;
        private AudioStreamPlayer ambientPlayer;

        private List<AudioStreamOggVorbis> cooldownList = new();
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
            FootstepsSFX = grassyFootsteps;
            ProcessMode = ProcessModeEnum.Always;
            PlayMusic(musicLevels, GetTree().Root);
            GetTree().TreeProcessModeChanged += OnTreeProcessModeChanged;
            EnvironmentManager.GetInstance().WeatherChanged += OnWeatherChanged;
        }

        //Useful for transitions
        private void OnTreeProcessModeChanged()
        {
            if (GetTree().Paused)
                PauseAllSounds();
            else
                ResumeAllSounds();
        }

        private void OnWeatherChanged(int pNewWeather)
        {
            EnvironmentManager.Weather lWeather = (EnvironmentManager.Weather)pNewWeather;
            AudioStreamOggVorbis lStream;

            switch (lWeather)
            {
                case EnvironmentManager.Weather.NIGHT:
                    lStream = ClearNightAmbient;
                    break;
                case EnvironmentManager.Weather.NIGHT_RAIN:
                    lStream = RainyNightAmbient;
                    break;
                case EnvironmentManager.Weather.SNOW:
                    lStream = SnowAmbient;
                    FootstepsSFX = snowyFootsteps;
                    break;
                case EnvironmentManager.Weather.DAY:
                default:
                    PlaySFX(RoosterSFX, (float)RoosterSFX.GetLength());
                    lStream = DayAmbient;
                    break;
            }

            PlayAmbient(lStream);

            if(lWeather != EnvironmentManager.Weather.SNOW)
                FootstepsSFX = grassyFootsteps;
        }

        /*
         * PLAY METHODS
         */

        public static void PlayMusic(AudioStreamOggVorbis pStream, Node pContainer)
        {
            if (MusicPlayer != null && MusicPlayer.Stream == pStream)
                return;

            AudioStreamPlayer lOldMusic = MusicPlayer;
            MusicPlayer = new AudioStreamPlayer();
            pStream.Loop = true;
            MusicPlayer.Stream = pStream;
            MusicPlayer.Bus = SoundPath.MUSIC_SOUND_BUS;
            MusicPlayer.ProcessMode = ProcessModeEnum.Always;
            MusicPlayer.Autoplay = true;
            pContainer.CallDeferred("add_child", MusicPlayer);

            if (IsInstanceValid(lOldMusic))
                AudioTransitioner.Create(MusicPlayer, lOldMusic, 1.5f, pContainer);
        }

        public void PlayAmbient(AudioStreamOggVorbis pStream)
        {
            AudioStreamPlayer lOldAmbient = ambientPlayer;
            ambientPlayer = Play(pStream, true, SoundPath.AMBIENT_SOUND_BUS);
            playersList.Remove(ambientPlayer);

            if (ambientTransitioner != null && IsInstanceValid(ambientTransitioner))
                ambientTransitioner.QueueFree();

            if (IsInstanceValid(lOldAmbient))
            {
                ambientTransitioner = AudioTransitioner.Create(ambientPlayer, lOldAmbient, EnvironmentManager.WEATHER_TRANSITION_TIME);
                ambientTransitioner.TreeExiting += () => OnAmbientTransitionFinished(ambientTransitioner);
            }
        }

        private void OnAmbientTransitionFinished(AudioTransitioner pTransitioner)
        {
            if(pTransitioner == ambientTransitioner)
                ambientTransitioner = null;
        }

        public bool PlaySFX(AudioStreamOggVorbis pStream, float pTimeBetweenSFX = default)
        {
            if(pTimeBetweenSFX != default)
            {
                if (cooldownList.Contains(pStream))
                    return false;

                cooldownList.Add(pStream);
                if(IsInsideTree())
                    GetTree().CreateTimer(pTimeBetweenSFX, false).Timeout += () => cooldownList.Remove(pStream);
            }

            Play(pStream, false, SoundPath.SFX_SOUND_BUS);
            return true;
        }

        public bool PlaySFXFromArray(AudioStreamOggVorbis[] pSFXArray, float pTimeBetweenSFX = default)
        {
            if(pTimeBetweenSFX != default)
                foreach(AudioStreamOggVorbis lSFX in pSFXArray)
                    if (cooldownList.Contains(lSFX))
                        return false;
            return PlaySFX(pSFXArray[new Random().Next(pSFXArray.Length)], pTimeBetweenSFX);
        }

        private void OnPlayerFinished(AudioStreamPlayer pPlayer)
        {
            playersList.Remove(pPlayer);
            pPlayer.QueueFree();
        }

        private AudioStreamPlayer Play(AudioStreamOggVorbis pStream, bool pIsLooping = false, string pBus = SoundPath.MAIN_SOUND_BUS)
        {
            AudioStreamPlayer lPlayer = new();
            pStream.Loop = pIsLooping;
            lPlayer.Stream = pStream;
            lPlayer.Bus = pBus;
            playersList.Add(lPlayer);
            lPlayer.Finished += () => OnPlayerFinished(lPlayer);
            AddChild(lPlayer);
            lPlayer.Play();
            return lPlayer;
        }

        /*
         * PAUSE/RESUME METHODS
         */

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

        /*
         * VOLUME METHODS
         */

        public static float DbFromPercentage(float pPercentage)
        {
            pPercentage = Mathf.Clamp(pPercentage, 0, 100);
            return Mathf.LinearToDb(pPercentage / 100);
        }

        public static float GetVolumePercentage(string pBusName)
        {
            int lBusIndex = AudioServer.GetBusIndex(pBusName);
            float lDecibels = AudioServer.GetBusVolumeDb(lBusIndex);
            return Mathf.Pow(10, lDecibels / 20) * 100;
        }

        public static void SetVolumeFromPercentage(float pPercentage, string pBusName)
        {
            int lBusIndex = AudioServer.GetBusIndex(pBusName);
            AudioServer.SetBusVolumeDb(lBusIndex, DbFromPercentage(pPercentage));
        }

        /*
         * DISPOSE
         */

        protected override void Dispose(bool pDisposing)
        {
            if (instance == this)
                instance = null;
            if(IsInsideTree())
                GetTree().TreeProcessModeChanged -= OnTreeProcessModeChanged;
            EnvironmentManager.GetInstance().WeatherChanged -= OnWeatherChanged;

            base.Dispose(pDisposing);
        }

    }
}
