using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim
{
    public partial class LanguageButton : ButtonWithHoverAnim
    {
        [Export] private string language;

        public override void _Ready()
        {
            Pressed += OnPressed;
        }

        protected override void OnPressed()
        {
            base.OnPressed();
            TranslationServer.SetLocale(language);
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.LanguageChanged);
        }
    }
}
