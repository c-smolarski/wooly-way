using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;
using System.Collections.Generic;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI
{
    public partial class ControlsItemList : ItemList
    {
        private List<string> itemsTranslationKeys;
        public override void _Ready()
        {
            base._Ready();
            SignalBus.Instance.LanguageChanged += InitTranslation;
            InitTranslation();
            ItemSelected += OnItemSelected;
            Select(GameManager.currentControlScheme == Player.ControlScheme.CLASSIC ? 0 : 1);
        }

        private void InitTranslation()
        {
            if (itemsTranslationKeys == null)
            {
                itemsTranslationKeys = new();
                for (int i = 0; i < ItemCount; i++)
                    itemsTranslationKeys.Add(GetItemText(i));
            }

            for (int i = 0; i < ItemCount; i++)
                SetItemText(i, Tr(itemsTranslationKeys[i]));
        }

        private void OnItemSelected(long pIndex)
        {
            GameManager.currentControlScheme = pIndex == 0 ? Player.ControlScheme.CLASSIC : Player.ControlScheme.ALTERNATIVE;
        }

        protected override void Dispose(bool pDisposing)
        {
            SignalBus.Instance.LanguageChanged -= InitTranslation;
            base.Dispose(pDisposing);
        }
    }
}
