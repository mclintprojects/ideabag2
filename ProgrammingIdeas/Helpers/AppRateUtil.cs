using Android.App;
using Helpers;

namespace ProgrammingIdeas.Helpers
{
    internal static class AppRateUtil
    {
        public static void OnAppStarted()
        {
            var threshold = PreferenceManager.Instance.GetEntry("rateAppThreshold", 10);
            var currentThreshold = PreferenceManager.Instance.GetEntry("currentRateAppThreshold", 0);

            if (currentThreshold >= threshold)
            {
                ShowAppRateDialog();
                PreferenceManager.Instance.AddEntry("currentRateAppThreshold", 0); // Reset current threshold
            }
        }

        private static void ShowAppRateDialog()
        {
            new AlertDialog.Builder(App.CurrentActivity)
                .SetTitle("Rate Programming Ideas 2")
                .SetMessage("If you enjoy using Programming Ideas 2, please help us by rating it. It only takes a fe seconds!")
                .SetPositiveButton("Rate", (s, e) => Attempted())
                .Create().Show();
        }

        private static void Attempted()
        {
        }
    }
}