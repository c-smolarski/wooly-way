using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.Buttons;
using Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;
using Color = Godot.Color;
using MenuButton = Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.AnimatedButtons.MenuButton;

// Author : Alissa Delattre && Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.UI.Menus
{

    public partial class WinScreen : PausingMenu
    {
        [Export] private SheepButton[] sheepStars;
        [Export] private MenuButton creditsButton;
        [Export] private Label scoreText;
        [Export] private Panel container;
        [Export] private Label winMessage;
        
        private const float SHEEP_APPEAR_TIME = 0.5f;
        private const float SHEEP_APPEAR_SCALE = 5f;
        private const float SHEEP_BLEAT_DELAY = 0.25f;
        private const string FINISH_MESSAGE = "LABEL_WIN_GAME";


        public static Color NoStarColor => new Color(0.1f, 0.1f, 0.1f);

        private float timerWaitTime = .3f;
        private int score;
        private int star;

        public override void _Ready()
        {
            base._Ready();
            Init();
            HideSheeps();
            WinCheck();
        }

        private void Init()
        {
            (score, star) = DataManager.GetInstance().StepToScore(LevelManager.CurrentLevel.Player.steps);

            scoreText.Text += score.ToString();

            Tween lTween = CreateTween();
            lTween.TweenProperty(container, TweenProp.SCALE, Vector2.One, .5f)
                .From(Vector2.Zero)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.Out);
            lTween.TweenInterval(timerWaitTime);
            lTween.Finished += SheepAppearance;
        }

        private void HideSheeps()
        {
            foreach (ButtonWithHoverAnim lSheep in sheepStars)
                lSheep.Visible = false;
        }

        private void WinCheck()
        {
            if (LevelManager.CurrentLevel.Info.LevelName + 1 > LevelManager.GetInstance().NumLevels())
            {
                creditsButton.Visible = true;
                winMessage.Text = Tr(FINISH_MESSAGE);
            }
        }

        private void SheepAppearance()
        {
            Vector2 lSheepScale;
            Tween lTween = CreateTween();
            
            lTween.SetParallel();
            for (int i = 0; i < DataManager.MAX_STAR; i++)
            {
                sheepStars[i].Visible = true;
                if (i < star)
                {
                    ((ShaderMaterial)sheepStars[i].Material).SetShaderParameter("alpha", 0.5);

                    lSheepScale = sheepStars[i].Scale;
                    sheepStars[i].Scale = Vector2.Zero;

                    lTween.TweenProperty(sheepStars[i], TweenProp.MODULATE, sheepStars[i].Modulate, SHEEP_APPEAR_TIME)
                        .From(NoStarColor)
                        .SetDelay(i * (SHEEP_APPEAR_TIME / 2));
                    lTween.TweenProperty(sheepStars[i], TweenProp.SCALE, lSheepScale, SHEEP_APPEAR_TIME)
                        .From(sheepStars[i].Scale * SHEEP_APPEAR_SCALE)
                        .SetTrans(Tween.TransitionType.Bounce)
                        .SetEase(Tween.EaseType.Out)
                        .SetDelay(i * (SHEEP_APPEAR_TIME / 2));
                }
                else
                {
                    sheepStars[i].Disable();
                    ((ShaderMaterial)sheepStars[i].Material).SetShaderParameter("alpha", 0);
                    sheepStars[i].Modulate = NoStarColor;
                }
            }
            lTween.Finished += JuicyScore;
        }

        private void JuicyScore()
        {
            int lIndex = 0;
            Tween lTween = CreateTween();
            lTween.SetParallel();
            for (int i = 0; i < star; i++)
                if (i < star)
                {
                    lTween.TweenInterval(i * SHEEP_BLEAT_DELAY).Finished +=
                        () => sheepStars[lIndex++].EmitSignal(BaseButton.SignalName.Pressed);
                }
        }
    }
}
