using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : SMOLARSKI Camille & Julien Lim

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.AnimatedButtons
{
    public partial class MenuButton : AnimatedButton
    {
        [Export] private string pathSceneToLoad;
        [Export] protected Transition Transition { get; private set; }

        public override void _Ready()
        {
            base._Ready();
            Disabled = true;
            Transition.AnimationFinished += ButtonInit;
        }

        private void ButtonInit(StringName animName)
        {
            Disabled = false;
            Transition.AnimationFinished -= ButtonInit;
        }

        protected override void OnPressed()
        {
            base.OnPressed();
            Transition?.Start();
            if (Transition != null) Transition.AnimationFinished += (StringName pName) => ChangeScene();
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.AnyMenuButtonPressed);
        }

        protected void ChangeScene()
        {
            GetTree().ChangeSceneToFile(pathSceneToLoad);
        }
    }
}
