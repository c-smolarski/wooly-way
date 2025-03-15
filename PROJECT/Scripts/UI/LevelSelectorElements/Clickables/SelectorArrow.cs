using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables
{
    public abstract partial class SelectorArrow : ClickableLevelSelectorElement
    {
        [Export] private Color hoverColor = new Color(1.0f, 1.0f, 0.812f, 1.0f);

        private const string POLYGON_PATH = "polygon";
        private const string SHADOW_PATH = "shadow";

        private Polygon2D polygon, shadow;
        private Color defaultColor;

        public override void _Ready()
        {
            base._Ready();
            MouseDetector.MouseEntered += OnHovered;
            MouseDetector.MouseExited += OnUnhovered;

            polygon = GetNode<Polygon2D>(POLYGON_PATH);
            shadow = GetNode<Polygon2D>(SHADOW_PATH);
            defaultColor = polygon.Color;
        }

        private void OnHovered()
        {
            polygon.Color = hoverColor;
        }

        private void OnUnhovered()
        {
            polygon.Color = defaultColor;
        }

        protected override void Dispose(bool disposing)
        {
            MouseDetector.MouseEntered -= OnHovered;
            MouseDetector.MouseExited -= OnUnhovered;
            base.Dispose(disposing);
        }
    }
}
