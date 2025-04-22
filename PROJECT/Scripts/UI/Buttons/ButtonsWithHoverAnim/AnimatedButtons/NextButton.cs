using Com.IsartDigital.WoolyWay.Assets;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables;
using Com.IsartDigital.WoolyWay.Data;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.AnimatedButtons
{
    public partial class NextButton : ResumeButton
    {
        private int newLevelNumber;

        public override void _Ready()
        {
            newLevelNumber = LevelManager.CurrentLevel.Info.LevelName + 1;
            if (newLevelNumber > LevelManager.GetInstance().NumLevels())
            {
                Disable();
                Visible = false;
            }
            else
                base._Ready();
        }

        protected override void OnPressed()
        {
            base.OnPressed();

            for (int i = History.historyInstances.Count - 1; i < 0; i--)
            {
                History.historyInstances[i].Visible = false;
                History.historyInstances[i].MouseFilter = MouseFilterEnum.Ignore;
                History.historyInstances[i].Free();
            }
            
            History.historyInstances.Clear();

            LevelButton lButton = GD.Load<PackedScene>(GetLevelButton(newLevelNumber)).Instantiate<LevelButton>();
            lButton.Visible = false;
            GetTree().Root.AddChild(lButton);

            GridInit(lButton.LevelGrid);
            BackgroundInit(
                lButton.sceneBackground.Instantiate<LevelBackground>(),
                GameManager.Instance.AssetsContainer
                );

            EnvironmentManager.GetInstance().ChangeWeather(lButton.Weather);

            lButton.QueueFree();
        }

        private string GetLevelButton(int pIndex)
        {
            DirAccess lDir = DirAccess.Open(FilePath.LEVEL_BUTTONS_PATH);
            int lFileNumber = 0; ;
            if (lDir != null)
            {
                lDir.ListDirBegin();
                string lFilePath = lDir.GetNext();
                while (lFilePath != "")
                {
                    lFileNumber++;
                    if(lFileNumber == pIndex)
                        return FilePath.LEVEL_BUTTONS_PATH + lFilePath.Substring(0, 11);
                    lFilePath = lDir.GetNext();
                }
            }
            return "";
        }

        private void GridInit(Grid pGrid)
        {
            pGrid.GlobalPosition = GetViewport().GetCamera2D().GlobalPosition = default;
            pGrid.GlobalScale = Vector2.One;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.StartLevel, pGrid);
        }

        private void BackgroundInit(LevelBackground pBackground, Node2D pContainer)
        {
            foreach (Node lNode in pContainer.GetChildren())
                lNode.QueueFree();
            pContainer.AddChild(pBackground);
            pBackground.PlayAppearAnim();
        }
    }
}
