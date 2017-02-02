using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace ProgrammingIdeas
{
    [Activity(Label = "About")]
    public class About : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.aboutactivity);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            CardView fuckyouLinker = FindViewById<CardView>(Resource.Id.fuckyouLinker);
            TextView bugsy = FindViewById<TextView>(Resource.Id.bugsy);
            bugsy.Text = "For bug reports, please join the Google Plus community here https://plus.google.com/communities/105361187156121163553\r\n\r\n" +
                "Twitter support: @mclint_\r\nDeveloper's email: clintonmbah44@gmail.com\r\n\r\nCredits\r\n1. The Daily Programmer reddit thread" +
                "\r\n2. The Invent with Python blog\r\n3. Karan Goel\r\n4. Martyr2 thread\r\n5. Gianluca De Roma\r\nThanks alot guys!";
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Intent intent = new Intent(this, typeof(MainActivity));
                    NavigateUpTo(intent);
                    OverridePendingTransition(Resource.Animation.push_up_in, Resource.Animation.push_up_out);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}