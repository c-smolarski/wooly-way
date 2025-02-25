using Godot;
using System;
using System.Globalization;
using System.Linq;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName
{
    public partial class UsernameText : RichTextLabel
    {
        [Export] private LineEdit textInput;
        [Export] private bool secret = false;
        [Export] private Button showPass;

        private const string EFFECT = "[shake]";
        private const string END_EFFECT = "[/shake]";
        private const string HIDE = "•";

        private string currentText = "";
        public override void _Ready()
        {
            textInput.TextChanged += TextEffect;
            showPass.ButtonDown += ShowPassword;
            showPass.ButtonUp += HidePassword;
        }

        private void ShowPassword()
        {
            Text = EFFECT + textInput.Text + END_EFFECT;
        }

        private void HidePassword()
        {
            Text = EFFECT + currentText + END_EFFECT;
        }
        private void TextEffect(string pText)
        {
            if (secret)
            {
                if (textInput.Text.Length > currentText.Length)
                {
                    currentText += HIDE;
                    Text = EFFECT + currentText + END_EFFECT;
                }
                else
                {
                    currentText = currentText.Substr(0, currentText.Length -1);
                    Text = EFFECT + currentText + END_EFFECT;
                }
            }
            else
            {
                Text = EFFECT + textInput.Text + END_EFFECT;
            }
        }
    }
}
