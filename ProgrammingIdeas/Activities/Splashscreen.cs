using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Helpers;
using ProgrammingIdeas.Activities;

namespace Activities
{
    [Activity(Label = "Idea Bag 2", Icon = "@mipmap/icon", MainLauncher = true, Theme = "@style/AppFullscreen", NoHistory = true)]
    public class Splashscreen : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            PreferenceHelper.Init(this);
            StartActivity(new Intent(this, typeof(CategoryActivity)));
        }
    }
}