using Godot;
using System;

// author : Moussouni-lepilliez Daniel

namespace Com.IsartDigital.WoolyWay.Managers
{
    public partial class EnvironmentManager : Node
    {
        #region Singleton

        static private EnvironmentManager instance;

        private EnvironmentManager()
        {
        }

        static public EnvironmentManager GetInstance()
        {
            if (instance == null) instance = new EnvironmentManager();
            return instance;
        }

        #endregion

        #region WorldEnvironment
        [Export] WorldEnvironment worldEnvironment;
        private float DayExposure = 1f;
        private float NightExposure = 0.375f;
        private const string MODULATE_A = "modulate:a";
        private const string TONEMAP_EXPOSURE = "tonemap_exposure";
        #endregion WorldEnvironment

        [Signal] public delegate void WeatherChangedEventHandler(int pNewWeather);

        [Export] PackedScene sceneFireflies;
        [Export] PackedScene sceneGodRay;
        [Export] PackedScene sceneRain;
        [Export] PackedScene sceneSnow;
        [Export] private CanvasLayer canvasLayer;

        private float NightBlueValue
        {
            get => ((GradientTexture2D)worldEnvironment.Environment.AdjustmentColorCorrection).Gradient.GetColor(0).B * 255f;
            set
            {
                float lValue = value / 255f;
                ((GradientTexture2D)worldEnvironment.Environment.AdjustmentColorCorrection).Gradient.SetColor(0, new Color(0f, 0f, lValue, 1f));
                ((GradientTexture2D)worldEnvironment.Environment.AdjustmentColorCorrection).Gradient.SetColor(1, new Color(1f - lValue, 1f - (lValue /2f), 1f, 1f));
            }
        }

        public const float WEATHER_TRANSITION_TIME = 1.3f;
        public const float DELETE_TRANSITION_TIME = 1.3f;
        private const float NIGHT_BLUE_RATIO = 40f;

        public enum Weather
        {
            DAY,
            NIGHT,
            NIGHT_RAIN,
            SNOW,
            NONE
        }

        public static Weather CurrentWeather { get; private set; } = Weather.NONE;
        private GpuParticles2D currentFirefly;
        private ColorRect currentRain;
        private ColorRect currentSnow;
        private Control currentDay;

        public override void _Ready()
        {
            #region Singleton

            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(EnvironmentManager) + "Instance already exists, destroying the last added");
                return;
            }

            instance = this;

            #endregion
            worldEnvironment.Environment.AdjustmentEnabled = true;
        }

        public void ChangeWeather(Weather pWeather)
        {

            if (pWeather != CurrentWeather)
                EmitSignal(SignalName.WeatherChanged, (int)pWeather);

            switch (pWeather)
            {
                case Weather.DAY:
                    SetDay();
                    break;
                case Weather.NIGHT:
                    SetNight();
                    break;
                case Weather.SNOW:
                    SetSnow();
                    break;
                case Weather.NIGHT_RAIN:
                    SetNightRain();
                    break;
            }
        }

        private void RemoveEffect(Node pEffect)
        {
            if (IsInstanceValid(pEffect) && pEffect.IsInsideTree())
            {
                if (pEffect is ColorRect)
                    pEffect.GetChild<AnimationPlayer>(0).Play("Disappear");
                Tween lTween = CreateTween();
                lTween.TweenProperty(pEffect,MODULATE_A, 0, DELETE_TRANSITION_TIME);
                lTween.Finished += () => DeleteEffect(pEffect);
            }
        }

        private void DeleteEffect(Node pEffect)
        {
            if (IsInstanceValid(pEffect) && pEffect.IsInsideTree())
                pEffect.QueueFree();
        }

        private void SetDay()
        {
            RemoveEffect(currentFirefly);
            RemoveEffect(currentRain);
            RemoveEffect(currentSnow);
            if (CurrentWeather != Weather.DAY)
            {
                currentDay = sceneGodRay.Instantiate<Control>();
                canvasLayer.AddChild(currentDay);
                Tween lTween = CreateTween();
                lTween.TweenProperty(worldEnvironment.Environment,TONEMAP_EXPOSURE, DayExposure, WEATHER_TRANSITION_TIME);
                lTween.Parallel().TweenProperty(this, nameof(NightBlueValue), 0f, WEATHER_TRANSITION_TIME);
                CurrentWeather = Weather.DAY;
            }
        }

        private void SetNight()
        {
            RemoveEffect(currentRain);
            RemoveEffect(currentDay);
            RemoveEffect(currentSnow);
            if (CurrentWeather != Weather.NIGHT)
            {
                currentFirefly = sceneFireflies.Instantiate<GpuParticles2D>();
                canvasLayer.AddChild(currentFirefly);
                Tween lTween = CreateTween();
                lTween.TweenProperty(worldEnvironment.Environment,TONEMAP_EXPOSURE, NightExposure, WEATHER_TRANSITION_TIME);
                lTween.Parallel().TweenProperty(this, nameof(NightBlueValue), NIGHT_BLUE_RATIO, WEATHER_TRANSITION_TIME);
                currentFirefly.Modulate = new Color(currentFirefly.Modulate.R, currentFirefly.Modulate.G,
                    currentFirefly.Modulate.B, 0);
                lTween.Parallel().TweenProperty(currentFirefly, MODULATE_A, 1, WEATHER_TRANSITION_TIME);
                CurrentWeather = Weather.NIGHT;
            }
        }

        private void SetNightRain()
        {
            RemoveEffect(currentFirefly);
            RemoveEffect(currentDay);
            RemoveEffect(currentSnow);
            if (CurrentWeather != Weather.NIGHT_RAIN)
            {
                currentRain = sceneRain.Instantiate<ColorRect>();
                canvasLayer.AddChild(currentRain);
                currentRain.GetChild<AnimationPlayer>(0).Play("RainAppear");
                Tween lTween = CreateTween();
                lTween.TweenProperty(worldEnvironment.Environment,TONEMAP_EXPOSURE, NightExposure, WEATHER_TRANSITION_TIME);
                lTween.Parallel().TweenProperty(this, nameof(NightBlueValue), NIGHT_BLUE_RATIO, WEATHER_TRANSITION_TIME);
                CurrentWeather = Weather.NIGHT_RAIN;
            }
        }

        private void SetSnow()
        {
            RemoveEffect(currentRain);
            RemoveEffect(currentDay);
            RemoveEffect(currentFirefly);
            if (CurrentWeather != Weather.SNOW)
            {
                currentSnow = sceneSnow.Instantiate<ColorRect>();
                canvasLayer.AddChild(currentSnow);
                Tween lTween = CreateTween();
                lTween.TweenProperty(worldEnvironment.Environment, TONEMAP_EXPOSURE, DayExposure, WEATHER_TRANSITION_TIME);
                lTween.Parallel().TweenProperty(this, nameof(NightBlueValue), 0f, WEATHER_TRANSITION_TIME);
                currentRain.GetChild<AnimationPlayer>(0).Play("RainAppear");
                CurrentWeather = Weather.SNOW;
            }
        }

        protected override void Dispose(bool pDisposing)
        {
            instance = null;
            base.Dispose(pDisposing);
        }
    }
}