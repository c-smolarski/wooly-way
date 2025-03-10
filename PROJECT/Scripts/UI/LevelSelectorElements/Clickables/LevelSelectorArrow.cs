using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.RotatingElements;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables
{
    public partial class LevelSelectorArrow : Node2D
    {
        [Export] private LevelButtonDisplayer displayer;
        [Export] private Color hoverColor = new Color(1.0f, 1.0f, 0.812f, 1.0f);
        [Export] private bool right;

        private const string AREA_PATH = "mouseDetector";
        private const string POLYGON_PATH = "polygon";
        private const string SHADOW_PATH = "shadow";

        private ClickableArea mouseDetector;
        private Polygon2D polygon, shadow;
        private Color defaultColor;

        public override void _Ready()
        {
            base._Ready();
            mouseDetector = GetNode<ClickableArea>(AREA_PATH);
            polygon = GetNode<Polygon2D>(POLYGON_PATH);
            shadow = GetNode<Polygon2D>(SHADOW_PATH);
            defaultColor = polygon.Color;

            mouseDetector.MouseEntered += OnHovered;
            mouseDetector.MouseExited += OnUnhovered;
            mouseDetector.Clicked += OnClick;
            if (!right)
                Scale *= new Vector2(-1, 1);
        }

        private void OnClick()
        {
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.DisplayLevelButton, displayer.LevelNumber + (right ? 1 : -1));
        }

        private void OnHovered()
        {
            polygon.Color = hoverColor;
        }
        private void OnUnhovered()
        {
            polygon.Color = defaultColor;
        }
    }
}
