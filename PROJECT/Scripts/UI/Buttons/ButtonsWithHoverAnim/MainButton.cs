using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim
{
    public abstract partial class MainButton : ButtonWithHoverAnim
    {
        [Export] private Transition transition;

        public override void _Ready()
        {
            base._Ready();
            SignalBus.Instance.PausingMenuDisplayed += OnPausingMenuDisplayed;
            if(IsInstanceValid(transition))
            {
                transition.AnimationFinished += Appear;
                Visible = false;
                Disable();
            }
        }

        private void Appear(StringName animName)
        {
            Visible = true;
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.SCALE, Scale, 0.25f)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.Out)
                .From(Vector2.Zero);
            lTween.Finished += () => Disabled = false;
        }

        private void OnPausingMenuDisplayed()
        {
            if (IsInsideTree())
                Visible = !GetTree().Paused;
        }

        protected override void Dispose(bool pDisposing)
        {
            SignalBus.Instance.PausingMenuDisplayed -= OnPausingMenuDisplayed;
            if(IsInstanceValid(transition))
                transition.AnimationFinished -= Appear;
            base.Dispose(pDisposing);
        }
    }
}
