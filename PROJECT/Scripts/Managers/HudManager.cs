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

        private string pathPar = "Par";
        private string pathSteps = "Steps";
        private string pathLevel = "Level";

        private VBoxContainer vBoxContainer;
        private Label parLabel;
        private Label stepsLabel;
        private Label levelLabel;

        public static HudManager Instance { get; private set; }
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
        }
        public void CreateHud()
		{
			var lHud = NodeCreator.CreateNode(hudScene, canvasLayer);
            vBoxContainer = lHud.GetChild<VBoxContainer>(0);
            parLabel = vBoxContainer.GetNode<Label>(pathPar);
            stepsLabel = vBoxContainer.GetNode<Label>(pathSteps);
            levelLabel = lHud.GetNode<Label>(pathLevel);
            vBoxContainer.Position += new Vector2(0, -150);
            levelLabel.Position += new Vector2(0, -150);
            Tween lTween = CreateTween();
            lTween.TweenProperty(vBoxContainer, "position", new Vector2(0, 150), 3f).SetTrans(Tween.TransitionType.Back).AsRelative();
            lTween.Parallel().TweenProperty(levelLabel, "position", new Vector2(0, 150), 2.4f).SetTrans(Tween.TransitionType.Back).AsRelative();
        }
        public void ActualizeHud()
        {
            var lCurrentLevel = MapManager.GetInstance().currentLevel;
            parLabel.Text = "Par : " + lCurrentLevel.Par;
            levelLabel.Text = lCurrentLevel.Title;
        }
		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
		}
		protected override void Dispose(bool pDisposing)
		{
            if (Instance == this)
                Instance = null;
            base.Dispose(pDisposing);
        }
	}
}
