using Com.IsartDigital.WoolyWay;
using Godot;
using System;
using Com.IsartDigital.WoolyWay.Utils;
using Godot.NativeInterop;
using System.Diagnostics;

// Author : Daniel Moussouni--Lepilliez

namespace Com.IsartDigital.ProjectName {
	
	public partial class HudManager :  Node
	{
		[Export] PackedScene hudScene;
        [Export] CanvasLayer canvasLayer;

        private string pathPar = "Par";
        private string pathSteps = "Steps";

        private VBoxContainer vBoxContainer;
        private Label par;
        private Label steps;

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
            par = vBoxContainer.GetChild<Label>(0);
            steps = vBoxContainer.GetNode<Label>(pathSteps);
            vBoxContainer.Position += new Vector2(0, -150);
            Tween lTween = CreateTween();
            lTween.TweenProperty(vBoxContainer, "position", new Vector2(0, 150), 3f).SetTrans(Tween.TransitionType.Back).AsRelative();

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
