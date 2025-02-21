using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName
{

    public partial class UsernameText : RichTextLabel
    {
        [Export] private LineEdit textInput;
        [Export] private bool secret = false;
        private string currentText = "";
        public override void _Ready()
        {
            textInput.TextChanged += TextEffect;
        }

        public override void _Process(double pDelta)
        {
            float lDelta = (float)pDelta;
        }

        private void TextEffect(string pText)
        {
            if (!secret)
            {
                Text = "[shake]" + textInput.Text + "[/shake]";
                currentText = textInput.Text;
            }
            else
            {
                currentText += "•";
                Text = "[shake]" + currentText + "[/shake]";

            }
        }

        protected override void Dispose(bool pDisposing)
        {

        }
    }
}
