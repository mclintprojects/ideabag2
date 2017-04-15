using System;
using Android.App;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views.Animations;
using Android.Widget;
using ProgrammingIdeas.Animation;

namespace ProgrammingIdeas.Helpers
{
	public class Validator
	{
		private static bool valid;
		public bool Result { get { return valid; } }
		Context ctx;

		public Validator(Context ctx)
		{
			this.ctx = ctx;
		}

		public void CheckIfEmpty(EditText editText, string inputDescription)
		{
			editText.Background.SetTint(ContextCompat.GetColor(ctx, Resource.Color.primaryLight));
			if (!String.IsNullOrEmpty(editText.Text) || !String.IsNullOrWhiteSpace(editText.Text))
				valid = true;
			else
			{
				string errorString = $"{inputDescription} is required.";
				valid = false;
				editText.SetError(errorString, null);
				editText.Background.SetTint(ContextCompat.GetColor(ctx, Android.Resource.Color.HoloRedLight));
				AnimHelper.Animate(editText, "rotationY", 700, new AnticipateOvershootInterpolator(), 20, 0, -20, 0);
			}
		}
	}
}
