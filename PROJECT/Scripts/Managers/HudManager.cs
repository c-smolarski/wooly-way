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

        private VBoxContainer vBoxContainer;
        private Label parLabel;
        private Label stepsLabel;

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
            vBoxContainer.Position += new Vector2(0, -150);
            Tween lTween = CreateTween();
            lTween.TweenProperty(vBoxContainer, "position", new Vector2(0, 150), 3f).SetTrans(Tween.TransitionType.Back).AsRelative();

        }
        public void ActualizeHud()
        {
            parLabel.Text = "Par : " + MapManager.GetInstance().currentLevel.Par;
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
