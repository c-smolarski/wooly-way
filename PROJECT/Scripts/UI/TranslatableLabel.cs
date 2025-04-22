using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI
{
    public partial class TranslatableLabel : Label
    {
        [Export] private string translationKey;

        public void SetText(string pText)
        {
            Text = Tr(translationKey) + pText;
        }
    }
}
