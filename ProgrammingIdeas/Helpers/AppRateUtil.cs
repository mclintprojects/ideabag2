using Android.App;
using Android.Content;
using Android.Widget;
using Helpers;

namespace ProgrammingIdeas.Helpers
{
    /// <summary>
    /// Pops up a rate activity dialog when the app usage threshold is met
    /// </summary>
    internal static class AppRateUtil
    {
        public static void OnAppStarted()
        {
            var threshold = PreferenceManager.Instance.GetEntry("rateAppThreshold", 5);
            var currentThreshold = PreferenceManager.Instance.GetEntry("currentRateAppThreshold", 0);

            if (currentThreshold >= threshold)
            {
                ShowAppRateDialog();
                PreferenceManager.Instance.AddEntry("currentRateAppThreshold", 0); // Reset current threshold
            }
            else
                PreferenceManager.Instance.AddEntry("currentRateAppThreshold", currentThreshold + 1); // Increment the current threshold
        }

        private static void ShowAppRateDialog()
        {
            new AlertDialog.Builder(App.CurrentActivity)
                .SetTitle("Rate Programming Ideas 2")
                .SetMessage("If you enjoy using Programming Ideas 2, please help us by rating it. It only takes a few seconds!")
                .SetPositiveButton("Rate", (s, e) => Attempted())
                .SetNegativeButton("Never", (s, e) => DismissedNever())
                .SetNeutralButton("Later", (s, e) => DismissedLater())
                .Create().Show();
        }

        private static void DismissedLater()
        {
            var threshold = PreferenceManager.Instance.GetEntry("rateAppThreshold", 5);
            PreferenceManager.Instance.AddEntry("rateAppThreshold", threshold * 2);
        }

        private static void DismissedNever()
        {
            var threshold = PreferenceManager.Instance.GetEntry("rateAppThreshold", 5);
            PreferenceManager.Instance.AddEntry("rateAppThreshold", threshold * 8);
        }

        private static void Attempted()
        {
            try
            {
                App.CurrentActivity.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse($"market://details?id={App.CurrentActivity.PackageName}")));
            }
            catch
            {
                Toast.MakeText(App.CurrentActivity, "You do not have the Google Play Store installed.", ToastLength.Long).Show();
            }
            var threshold = PreferenceManager.Instance.GetEntry("rateAppThreshold", 5);
            PreferenceManager.Instance.AddEntry("rateAppThreshold", threshold * 4);
        }
    }
}