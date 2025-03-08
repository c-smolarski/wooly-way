using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.RotatingElements;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables
{
    public partial class LevelSelectorArrow : LevelSelectorElement
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
            if (!right)
                Scale *= new Vector2(-1, 1);
            base._Ready();

            mouseDetector = GetNode<ClickableArea>(AREA_PATH);
            polygon = GetNode<Polygon2D>(POLYGON_PATH);
            shadow = GetNode<Polygon2D>(SHADOW_PATH);
            defaultColor = polygon.Color;

            mouseDetector.MouseEntered += OnHovered;
            mouseDetector.MouseExited += OnUnhovered;
            mouseDetector.Clicked += OnClick;

            ScaleModifierChanged += () => Scale = ScaleModifier;
            displayer.Focused += ChangeVisibilty;
            ChangeVisibilty(false);
        }

        private void OnClick()
        {
            displayer.Mountain.DisplayButton(displayer.LevelNumber + (right ? 1 : -1));
        }

        private void OnHovered()
        {
            polygon.Color = hoverColor;
        }

        private void OnUnhovered()
        {
            polygon.Color = defaultColor;
        }

        protected override void Dispose(bool pDisposing)
        {
            displayer.Focused -= ChangeVisibilty;
            base.Dispose(pDisposing);
        }
    }
}
