using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.MainButtons
{
    public partial class PauseButton : MainButton
    {
        public override void _Ready()
        {
            base._Ready();
            SignalBus.Instance.TutorialChangedVisibility += Visibility;
        }

        private void Visibility(bool pVisible)
        {
            if(pVisible)
            {
                Disable();
                Visible = false;
            }
            else
            {
                Disabled = false;
                Visible = true;
            }
        }

        protected override void OnPressed()
        {
            base.OnPressed();
            GameManager.Instance.LoadPauseMenu();
        }

        protected override void Dispose(bool pDisposing)
        {
            SignalBus.Instance.TutorialChangedVisibility -= Visibility;
            base.Dispose(pDisposing);
        }
    }
}
