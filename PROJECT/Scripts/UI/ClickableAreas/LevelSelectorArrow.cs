using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI.ClickableAreas
{
    public partial class LevelSelectorArrow : ClickableArea
    {
        [Export] private LevelButton levelButton;
        [Export] private Color hoverColor = new Color(1.0f, 1.0f, 0.812f, 1.0f);
        [Export] private bool right;


        private const string POLYGON_PATH = "polygon";
        private const string SHADOW_PATH = "shadow";

        private Polygon2D polygon, shadow;
        private Color defaultColor;

        public override void _Ready()
        {
            base._Ready();
            polygon = GetNode<Polygon2D>(POLYGON_PATH);
            shadow = GetNode<Polygon2D>(SHADOW_PATH);
            defaultColor = polygon.Color;

            MouseEntered += OnHovered;
            MouseExited += OnUnhovered;
            Clicked += OnClick;
            if (!right)
                Scale *= new Vector2(-1, 1);
        }

        private void OnClick()
        {
            levelButton.Mountain.DisplayButton(levelButton.LevelNumber + (right ? 1 : -1));
        }

        private void OnButtonUp()
        {
            levelButton.Mountain.DisplayButton(levelButton.LevelNumber + (right ? 1 : -1));
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
