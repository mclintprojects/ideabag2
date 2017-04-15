using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using ProgrammingIdeas.Animation;
using Android.Views.Animations;

namespace ProgrammingIdeas.Activities
{
    [Activity(Label = "Donate", Theme = "@style/AppTheme")]
    public class DonateActivity : AppCompatActivity
    {
        TextView amountLbl;
        Button nextAmountBtn, donateAmountBtn;
        int currentIndex;
        string baseUrl = @"https://www.paypal.me/elormvowotor/";
        string[] amounts = new string[] { "$1", "$2", "$5", "$10", "Your choice" };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.donateactivity);
        }

        protected override void OnResume()
        {
            ShowDisclaimerDialog();
            amountLbl = FindViewById<TextView>(Resource.Id.amountLbl);
            nextAmountBtn = FindViewById<Button>(Resource.Id.nextAmountBtn);
            donateAmountBtn = FindViewById<Button>(Resource.Id.donateAmountBtn);

            nextAmountBtn.Click += delegate
            {
				++currentIndex;

                if (currentIndex > 4)
					currentIndex = 0;
                AnimHelper.Animate(amountLbl, "rotationY", 1000, new AnticipateInterpolator(), 0, 360);
                amountLbl.Text = amounts[currentIndex];
            };

            donateAmountBtn.Click += delegate
            {
                var intent = new Intent(Intent.ActionView);
                var url = amountLbl.Text != "Your choice" ? $"{baseUrl}{amountLbl.Text.Substring(1, amountLbl.Text.Length - 1)}" : baseUrl;
                intent.SetData(Android.Net.Uri.Parse(url));
                StartActivity(intent);
            };

            base.OnResume();
        }

        private void ShowDisclaimerDialog()
        {
			var dialogAlreadyShown = GetPreferences(FileCreationMode.Private).GetBoolean("dialogShown", false);
			if (!dialogAlreadyShown)
			{
				new Android.App.AlertDialog.Builder(this)
					.SetTitle("Important info")
					.SetMessage(Resources.GetString(Resource.String.donateText))
					.SetPositiveButton("I understand", (s, e) => { return; })
					.SetCancelable(false)
					.Create()
					.Show();

				PutBoolean(true);
			}
        }

		void PutBoolean(bool value)
		{
			var editor = GetPreferences(FileCreationMode.Private).Edit();
			editor.PutBoolean("dialogShown", value);
			editor.Commit();
		}

        public override void OnBackPressed()
		{
			PutBoolean(false);
			StartActivity(new Intent(this, typeof(CategoryActivity)));
			OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
			base.OnBackPressed();
		}
    }
}