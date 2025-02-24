using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName
{

    public partial class UsernameText : RichTextLabel
    {
        [Export] private LineEdit textInput;
        [Export] private bool secret = false;
        [Export] private Button showPass;

        private string currentText = "";
        public override void _Ready()
        {
            textInput.TextChanged += TextEffect;
            showPass.ButtonDown += ShowPassword;
            showPass.ButtonUp += HidePassword;
        }

        public override void _Process(double pDelta)
        {
            float lDelta = (float)pDelta;
        }

        //DU DUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUURE
        private void ShowPassword()
        {
            Text = "[shake]" + textInput.Text + "[/shake]";
        }

        private void HidePassword()
        {
            Text = "[shake]" + currentText + "[/shake]";
        }
        private void TextEffect(string pText)
        {

                //DU DUUUUUUUUUUUUUURE AHHHHHHHHHHHHHHH
                currentText += "•";
                Text = "[shake]" + currentText + "[/shake]";

        }

        protected override void Dispose(bool pDisposing)
        {

        }
    }
}
