using System;
using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Scripts.Assets;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.Assets.SingleAssets
{
    public partial class LightableAsset : SingleAsset
    {
        [Export] private PointLight2D light;
        [Export] private ClickableArea mouseDetector;

        private const float LIGHT_VALUE = 0.6f;
        private const float LIGHT_FLICKER_RANGE = 0.15f;
        private const float LIGHT_FLICKER_SPEED = 3f;
        protected override float AnimStepDuration => 0.5f;


        private float LightBaseEnergy
        {
            get => baseEnergy;
            set => baseEnergy = light.Energy = value;
        }
        private float baseEnergy, currentEnergy, energyTarget, elapsedTime;

        public override void _Ready()
        {
            base._Ready();
            EnvironmentManager.GetInstance().WeatherChanged += OnWeatherChange;
            mouseDetector.Clicked += OnClick;
        }

        protected virtual void OnClick()
        {
            LightUp(LightBaseEnergy == 0);
        }

        public override void _Process(double pDelta)
        {
            if (LightBaseEnergy == LIGHT_VALUE)
            {
                if (!Mathf.IsEqualApprox(light.Energy, energyTarget, 0.02f))
                {
                    elapsedTime += (float)pDelta;
                    light.Energy = Mathf.Lerp(currentEnergy, energyTarget, Mathf.Clamp(elapsedTime / LIGHT_FLICKER_SPEED, 0, 1));
                }
                else
                {
                    elapsedTime = 0f;
                    currentEnergy = light.Energy;
                    energyTarget = LightBaseEnergy + GD.RandRange(-1, 1) * LIGHT_FLICKER_RANGE;
                }
            }
            else
                light.Energy = currentEnergy = energyTarget = LightBaseEnergy;
        }

        public override void PlayAppearAnim()
        {
            base.PlayAppearAnim();
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.SCALE, Scale, AnimStepDuration)
                .From(Vector2.Zero)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Bounce);
            lTween.Finished += () => OnWeatherChange((int)EnvironmentManager.CurrentWeather);
            mouseDetector.SetActive();
        }

        public override void PlayDisappearAnim()
        {
            LightUp(false, AnimStepDuration * 3f).Finished += () =>
            {
                Tween lTween = CreateTween();
                lTween.TweenProperty(this, TweenProp.SCALE, Vector2.Zero, AnimStepDuration)
                        .SetEase(Tween.EaseType.In)
                        .SetTrans(Tween.TransitionType.Bounce);
                lTween.Finished += ResetValues;
            };
            mouseDetector.SetActive(false);
        }

        private void OnWeatherChange(int pWeather = 0)
        {
            LightUp(
                (EnvironmentManager.Weather)pWeather == EnvironmentManager.Weather.NIGHT || (EnvironmentManager.Weather)pWeather == EnvironmentManager.Weather.NIGHT_RAIN,
                AnimStepDuration * 3f
            );
        }

        private Tween LightUp(bool pLit, float pTime = 0)
        {
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, nameof(LightBaseEnergy), pLit ? LIGHT_VALUE : 0f, pTime);
            lTween.Finished += () => currentEnergy = energyTarget = LightBaseEnergy;
            return lTween;
        }

        protected override void ResetValues()
        {
            base.ResetValues();
            LightBaseEnergy = 0f;
            mouseDetector.SetActive(false);
        }

        protected override void Dispose(bool pDisposing)
        {
            EnvironmentManager.GetInstance().WeatherChanged -= OnWeatherChange;
            base.Dispose(pDisposing);
        }
    }
}
