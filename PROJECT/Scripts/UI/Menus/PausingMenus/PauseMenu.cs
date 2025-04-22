using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI.Menus.PausingMenus
{
    public partial class PauseMenu : PausingMenu
    {
        [Export] private BaseButton settingsButton;
        [Export] private BaseButton backButton;
        [Export] private Panel basePanel;
        [Export] private Panel settingsPanel;

        public override void _Ready()
        {
            base._Ready();
            settingsButton.Pressed += ShowSettings;
            backButton.Pressed += HideSettings;
        }

        private void ShowSettings()
        {
            settingsPanel.Visible = true;
            basePanel.Visible = false;
        }

        private void HideSettings()
        {
            settingsPanel.Visible = false;
            basePanel.Visible = true;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            if (Input.IsActionJustPressed(InputManager.MENU_PAUSE))
                QueueFree();
        }
    }
}
