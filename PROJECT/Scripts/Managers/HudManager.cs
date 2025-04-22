using Com.IsartDigital.WoolyWay.UI;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

// Author : Daniel Moussouni--Lepilliez

namespace Com.IsartDigital.WoolyWay.Managers
{
	
	public partial class HudManager :  Node
	{
		[Export] PackedScene hudScene;
        [Export] CanvasLayer canvasLayer;

        private const string DOTS = " : ";

        private string pathAuthor = "Author";
        private string pathPar = "Par";
        private string pathLevel = "Level";
        private string pathSteps = "Steps";

        private VBoxContainer vBoxContainer;
        public MapInfo currentLevelInfo;
        private TranslatableLabel authorLabel;
        private TranslatableLabel parLabel;
        private TranslatableLabel stepsLabel;
        private TranslatableLabel levelLabel;

        public static HudManager Instance { get; private set; }

        private GameManager gameManager;

        public override void _Ready()
		{
            #region Singleton
            if (Instance != null)
            {
                GD.Print("Error : " + nameof(HudManager) + " already exists. The new one is being freed...");
                QueueFree();
                return;
            }
            Instance = this;
            #endregion
            SignalBus.Instance.PausingMenuDisplayed += OnMenuDisplayed;
            SignalBus.Instance.StartLevel += OnLevelStarted;
        }

        private void OnLevelStarted(Grid pGrid)
        {
            currentLevelInfo = pGrid.Info;
            CreateHud();
        }

        public void CreateHud()
		{
            gameManager = GameManager.Instance;
            var lHud = NodeCreator.CreateNode(hudScene, canvasLayer);
            vBoxContainer = lHud.GetChild<VBoxContainer>(0);
            authorLabel = vBoxContainer.GetNode<TranslatableLabel>(pathAuthor);
            parLabel = vBoxContainer.GetNode<TranslatableLabel>(pathPar);
            stepsLabel = vBoxContainer.GetNode<TranslatableLabel>(pathSteps);
            levelLabel = lHud.GetNode<TranslatableLabel>(pathLevel);
            vBoxContainer.Position += new Vector2(0, -250);
            levelLabel.Position += new Vector2(0, -150);
            Tween lTween = CreateTween();
            InitializeHud();
            lTween.TweenProperty(vBoxContainer, "position", new Vector2(0, 250), 3f).SetTrans(Tween.TransitionType.Back).AsRelative();
            lTween.Parallel().TweenProperty(levelLabel, "position", new Vector2(0, 150), 2.4f).SetTrans(Tween.TransitionType.Back).AsRelative();

        }

        private void OnMenuDisplayed()
        {
            if (!AreLabelsValid())
                return;

            parLabel.Visible = stepsLabel.Visible = levelLabel.Visible = authorLabel.Visible = !GetTree().Paused;
        }

        private string IntToText(int pInt)
        {
            return DOTS + pInt;
        }

        public void InitializeHud()
        {
            if (!AreLabelsValid())
                return;

            stepsLabel.SetText(IntToText(0));
            parLabel.SetText(IntToText(LevelManager.CurrentLevel.Info.Par));
            levelLabel.SetText(IntToText(LevelManager.CurrentLevel.Info.LevelName));
            authorLabel.SetText(LevelManager.CurrentLevel.Info.Author);
        }

        public void ActualizeHud(int pSteps)
        {
            if(!AreLabelsValid())
                return;

            parLabel.SetText(IntToText(LevelManager.CurrentLevel.Info.Par));
            levelLabel.SetText(IntToText(LevelManager.CurrentLevel.Info.LevelName));
            stepsLabel.SetText(IntToText(pSteps));
        }

        private bool AreLabelsValid()
        {
            return IsInstanceValid(parLabel) && IsInstanceValid(stepsLabel) && IsInstanceValid(levelLabel);
        }

		protected override void Dispose(bool pDisposing)
		{
            SignalBus.Instance.PausingMenuDisplayed -= OnMenuDisplayed;
            SignalBus.Instance.StartLevel -= OnLevelStarted;

            if (Instance == this)
                Instance = null;
            base.Dispose(pDisposing);
        }
	}
}
