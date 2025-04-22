using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements
{
    /// <summary>
    /// Base class for all LevelSelectorElement that contain a ClickableArea.
    /// </summary>
    public abstract partial class ClickableLevelSelectorElement : Node2D
    {
        protected ClickableArea MouseDetector { get; private set; }

        private const string AREA_PATH = "mouseDetector";

        public override void _Ready()
        {
            base._Ready();
            SignalBus.Instance.StartLevel += OnLevelStart;
            MouseDetector = GetNode<ClickableArea>(AREA_PATH);
            MouseDetector.Clicked += OnClick;
        }

        protected virtual void OnLevelStart(Grid pGrid)
        {
            MouseDetector.SetActive(false);
        }

        protected virtual void OnClick()
        {
            SoundManager.GetInstance().PlaySFXFromArray(SoundManager.GetInstance().UIButtonClick, 0.1f);
        }

        protected override void Dispose(bool pDisposing)
        {
            SignalBus.Instance.StartLevel -= OnLevelStart;
            base.Dispose(pDisposing);
        }
    }
}
