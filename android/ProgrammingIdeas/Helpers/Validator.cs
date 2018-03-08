using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Widget;
using System;
using System.Text.RegularExpressions;

namespace ProgrammingIdeas.Helpers
{
    /// <summary>
    /// Class that exposes methods that allows you to validate an input field.
    /// </summary>
    public class Validator : IDisposable
    {
        private bool passed = true, alreadyFailed;
        public bool PassedValidation => passed;
        private Drawable errorIcon;

        public Validator()
        {
            errorIcon = ContextCompat.GetDrawable(App.CurrentActivity, Resource.Drawable.circle);
        }

        /// <summary>
        /// Validates if the text in a field is a valid phone number
        /// </summary>
        /// <param name="editText"></param>
        public void ValidatePhoneNumber(EditText editText)
        {
            if (!alreadyFailed)
            {
                if (editText.Text.Length < 6 || editText.Text.Length > 15)
                {
                    passed = Regex.Match(editText.Text, @"^(\+[0-9]{9})$").Success;
                    alreadyFailed = true;
                    string response = editText.Text.Length == 0 ? "Phone number is empty." : "Invalid phone number.";
                    editText.SetError(response, errorIcon);
                    editText.RequestFocus();
                }
                else
                    passed = true;
            }
        }

        /// <summary>
        /// Validates if the text in a field is not null or empty
        /// </summary>
        /// <param name="editText"></param>
        /// <param name="isRequired"></param>
        public void ValidateIsNotEmpty(EditText editText, bool isRequired = false)
        {
            if (!alreadyFailed)
            {
                if (string.IsNullOrEmpty(editText.Text) || string.IsNullOrWhiteSpace(editText.Text))
                {
                    passed = false;
                    alreadyFailed = true;
                    string response = isRequired == false ? "is empty." : "is required.";
                    editText.SetError($"This {response}", errorIcon);
                    editText.RequestFocus();
                }
                else
                    passed = true;
            }
        }

        public void Dispose() => alreadyFailed = false;
    }
}