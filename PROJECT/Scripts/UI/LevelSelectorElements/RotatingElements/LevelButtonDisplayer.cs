using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.RotatingElements
{
    public partial class LevelButtonDisplayer : RotatingElement
    {
        [Export(PropertyHint.Range, "1,1000")] public int LevelNumber { get; private set; }

        private const string RIGHT_ARROW_PATH = "rightArrow";
        private const string LEFT_ARROW_PATH = "leftArrow";
        private const string LEVEL_BUTTON_PATH = "levelButton";

        private LevelSelectorArrow leftArrow, rightArrow;
        private LevelButton levelButton;

        public override void _Ready()
        {
            base._Ready();
            levelButton = GetNode<LevelButton>(LEVEL_BUTTON_PATH);
            levelButton.Scale = levelScale;

            Mountain.LevelsVisibilityChanged += DisplayLevel;

            ArrowsInit();
        }

        private void ArrowsInit()
        {
            leftArrow = GetNodeOrNull<LevelSelectorArrow>(LEFT_ARROW_PATH);
            rightArrow = GetNodeOrNull<LevelSelectorArrow>(RIGHT_ARROW_PATH);

            foreach (LevelSelectorArrow lArrow in new LevelSelectorArrow[] { leftArrow, rightArrow })
            {
                if (!LevelManager.LevelExists(Mountain.WorldNumber, LevelNumber + (lArrow == rightArrow ? 1 : -1)))
                    lArrow?.QueueFree();
                if (lArrow == null || lArrow.IsQueuedForDeletion())
                    continue;
                lArrow.Position = MathS.PolarToCartesian(
                    (levelButton.LevelGrid.PixelSize * levelScale).Length() * 0.5f,
                    lArrow.Rotation + (lArrow.Scale.Sign().X >= 0 ? 0 : MathF.PI)
                );
            }
        }

        public void DisplayLevel(int pLevelToDisplay)
        {
            ChangeVisibilty(IsNeighbour(pLevelToDisplay));

            if (LevelNumber == pLevelToDisplay)
            {
                Mountain.RotateToButton(this);
                levelButton.StartFocusedAnim();
            }
            else
                levelButton.StopFocusedAnim();
        }

        private bool IsNeighbour(int pFromLevel)
        {
            return LevelNumber >= pFromLevel - 1 && LevelNumber <= pFromLevel + 1;
        }
    }
}
