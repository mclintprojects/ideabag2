using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views.Animations;
using Android.Widget;
using Helpers;
using ProgrammingIdeas.Animation;

namespace ProgrammingIdeas.Activities
{
    [Activity(Label = "Donate", Theme = "@style/AppTheme")]
    public class DonateActivity : AppCompatActivity
    {
        private TextView amountLbl;
        private Button nextAmountBtn, donateAmountBtn;
        private int currentIndex;
        private string[] amounts = new string[] { "$1", "$2", "$5", "$10", "Your choice" };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.donateactivity);
        }

        protected override void OnResume()
        {
            base.OnResume();
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
                var url = amountLbl.Text != "Your choice" ? $"{AppResources.PaypalLink}{amountLbl.Text.Substring(1, amountLbl.Text.Length - 1)}" : AppResources.PaypalLink;
                intent.SetData(Android.Net.Uri.Parse(url));
                StartActivity(Intent.CreateChooser(intent, "Thank you for your donation! Please select any browser here."));
            };
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

                PreferenceManager.Instance.AddEntry("dialogShown", true);
            }
        }

        public override void OnBackPressed()
        {
            PreferenceManager.Instance.AddEntry("dialogShown", false);
            Finish();
            OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
        }
    }
}