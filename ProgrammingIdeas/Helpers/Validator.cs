using Android.Content;
using Android.Graphics;
using Android.Views.Animations;
using Android.Widget;
using ProgrammingIdeas.Animation;
using System;

namespace ProgrammingIdeas.Helpers
{
    public class Validator
    {
        private static bool valid;
        public bool Result { get { return valid; } }
        private Context ctx;

        public Validator(Context ctx)
        {
            this.ctx = ctx;
        }

        public void CheckIfEmpty(EditText editText, string inputDescription)
        {
            editText.SetBackgroundResource(Resource.Color.primaryLight);
            if (!String.IsNullOrEmpty(editText.Text) || !String.IsNullOrWhiteSpace(editText.Text))
                valid = true;
            else
            {
                string errorString = $"{inputDescription} is required.";
                valid = false;
                editText.SetError(errorString, null);
                editText.Background.SetColorFilter(Color.Tomato, PorterDuff.Mode.SrcIn);
                AnimHelper.Animate(editText, "rotationY", 500, new AnticipateOvershootInterpolator(), 20, 0, -20, 0);
            }
        }
    }
}