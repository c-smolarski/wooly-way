using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Scripts.Assets;
using Com.IsartDigital.WoolyWay.UI;
using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables;
using Com.IsartDigital.WoolyWay.UI.Menus;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.Assets.SingleAssets
{
    public partial class LevelSelectorSign : SingleAsset
    {
        [Export] private ClickableArea area;
        [Export] private LevelButton level;
        [Export] private TranslatableLabel label;
        [Export] private TextureRect[] sheepStars;

        private const string LABEL_TEXT = " : ";
        private const float INITIAL_ROTATION = Mathf.Pi / 1.5f;

        protected override float AnimStepDuration => 0.35f;

        public override void _Ready()
        {
            base._Ready();
            label.SetText(LABEL_TEXT + level.LevelNumber);
            area.Clicked += OnClick;
            SignalBus.Instance.MoveToLevelButton += OnDeparted;
            SignalBus.Instance.ArrivedAtLevelButton += OnArrived;
            SignalBus.Instance.StartLevel += OnLevelStart;
            SheepStarsInit();
        }

        private void SheepStarsInit()
        {
            int lLevelScore = (int)DataManager.UserLevelScores[level.LevelNumber - 1];
            for (int i = 0; i < sheepStars.Length; i++)
            {
                sheepStars[i].Material = (ShaderMaterial)sheepStars[i].Material.Duplicate();
                if (lLevelScore == 0 || DataManager.GetInstance().ScoreToStar(lLevelScore) < i + 1)
                {
                    ((ShaderMaterial)sheepStars[i].Material).SetShaderParameter("alpha", 0f);
                    sheepStars[i].Modulate = WinScreen.NoStarColor;
                }
                else
                {
                    ((ShaderMaterial)sheepStars[i].Material).SetShaderParameter("alpha", 0.5f);
                }
            }
        }

        private void OnLevelStart(Grid pGrid)
        {
            PlayDisappearAnim();
        }

        private void OnClick()
        {
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.LevelClicked, level.LevelNumber);
        }

        private void OnDeparted(int pNumber, bool pToNext)
        {
            if (Visible)
                PlayDisappearAnim();
        }

        private void OnArrived(int pNumber)
        {
            if (pNumber == level.LevelNumber)
                PlayAppearAnim();
        }

        public override void PlayAppearAnim()
        {
            base.PlayAppearAnim();
            area.SetActive();
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.ROTATION, Rotation, AnimStepDuration)
                .From(INITIAL_ROTATION)
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.Out);
            lTween.Parallel().TweenProperty(this, TweenProp.SCALE, Scale, AnimStepDuration)
                .From(Vector2.Zero)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.Out);
            SoundManager.GetInstance().PlaySFX(SoundManager.GetInstance().FwopSFX);
        }

        public override void PlayDisappearAnim()
        {
            Tween lTween = CreateTween();
            lTween.Parallel().TweenProperty(this, TweenProp.SCALE, Vector2.Zero, AnimStepDuration)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.In);
            lTween.Finished += ResetValues;
        }

        protected override void ResetValues()
        {
            base.ResetValues();
            area.SetActive(false);
        }

        protected override void Dispose(bool pDisposing)
        {
            SignalBus.Instance.MoveToLevelButton -= OnDeparted;
            SignalBus.Instance.ArrivedAtLevelButton -= OnArrived;
            SignalBus.Instance.StartLevel -= OnLevelStart;
            base.Dispose(pDisposing);
        }
    }
}
