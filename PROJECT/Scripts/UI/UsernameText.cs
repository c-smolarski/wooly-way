using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay.UI
{
    public partial class UsernameText : RichTextLabel
    {
        [Export] private LineEdit textInput;
        [Export] private bool secret = false;
        [Export] private Button showPass;
        [Export] private TextureRect sleepSheep;
        [Export] private TextureRect sheep;

        private const string EFFECT = "[shake]";
        private const string END_EFFECT = "[/shake]";
        private const string HIDE = "•";
        private const string DELETE = "ui_back";

        private string currentText = "";

        public override void _Ready()
        {
            textInput.TextChanged += TextEffect;
            showPass.ButtonDown += ShowPassword;
            showPass.ButtonUp += HidePassword;

            
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            DeleteChar();
            ShowSleepSheep();
        }
        /// <summary>
        /// Deletes the last character when Backspace is pressed
        /// </summary>
        private void DeleteChar()
        {
            if (Input.IsActionJustPressed(DELETE) && textInput.HasFocus())
            {
                textInput.DeleteCharAtCaret();
            }
        }
        private void ShowSleepSheep()
        {
            if (secret && textInput.HasFocus())
            {
                sheep.Visible = false;
            }
            else
            {
                sheep.Visible = true;
            }
        }

        /// <summary>
        /// Shows the hidden password while the button is pressed
        /// </summary>
        private void ShowPassword()
        {
            Text = EFFECT + textInput.Text + END_EFFECT;
        }

        /// <summary>
        /// hides the password once the button is let go
        /// </summary>
        private void HidePassword()
        {
            if (!secret) return;
            Text = EFFECT + currentText + END_EFFECT;
        }
        /// <summary>
        /// Takes the input of the LineEdit and adds an effect to it, checks if it needs to add a character or delete one and check if the password is hidden
        /// </summary>
        private void TextEffect(string pText)
        {
            if (secret && textInput.Text.Length > currentText.Length)
            {
                currentText += HIDE;
                Text = EFFECT + currentText + END_EFFECT;
            }
            else if (secret)
            {

                currentText = currentText.Substr(0, currentText.Length - 1);
                Text = EFFECT + currentText + END_EFFECT;
            }
            else
            {
                Text = EFFECT + textInput.Text + END_EFFECT;
            }
        }
    }
}

