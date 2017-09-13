using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Widget;
using System;
using System.Text.RegularExpressions;

namespace ProgrammingIdeas.Helpers
{
    public class Validator : IDisposable
    {
        private bool passed = true, alreadyFailed;
        public bool PassedValidation => passed;
        private Context ctx;
        private Drawable errorIcon;

        public Validator()
        {
            ctx = App.CurrentActivity;
            errorIcon = ContextCompat.GetDrawable(ctx, Resource.Drawable.circle);
        }

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

        public decimal ValidateAmount(EditText editText)
        {
            decimal output = 0;
            if (!alreadyFailed)
            {
                if (editText.Text != string.Empty && decimal.TryParse(editText.Text, out output))
                    passed = true;
                else
                {
                    passed = false;
                    alreadyFailed = true;
                    editText.SetError("This is not a valid amount.", errorIcon);
                    editText.RequestFocus();
                }
            }

            return output;
        }

        internal void ValidateIsSame(EditText passwordTb, EditText retypePasswordTb)
        {
            if (!alreadyFailed)
            {
                if (passwordTb.Text == retypePasswordTb.Text)
                    passed = true;
                else
                {
                    passed = false;
                    alreadyFailed = true;
                    Toast.MakeText(ctx, "Passwords are not the same.", ToastLength.Long).Show();
                }
            }
        }

        public void Dispose() => alreadyFailed = false;
    }
}