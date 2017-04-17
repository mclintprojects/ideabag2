using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace ProgrammingIdeas.Activities
{
    [Activity(Label = "About", Theme = "@style/AppTheme")]
    public class AboutActivity : BaseActivity
    {
        public override int LayoutResource
        {
            get
            {
                return Resource.Layout.aboutactivity;
            }
        }

        public override bool HomeAsUpEnabled
        {
            get
            {
                return true;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var fuckyouLinker = FindViewById<CardView>(Resource.Id.fuckyouLinker);
            var bugsy = FindViewById<TextView>(Resource.Id.bugsy);
            bugsy.Text = "For bug reports, please join the Google Plus community here https://plus.google.com/communities/105361187156121163553\r\n\r\n" +
                "Twitter support: @mclint_\r\nDeveloper's email: clintonmbah44@gmail.com\r\n\r\nCredits\r\n1. The Daily Programmer reddit thread" +
                "\r\n2. The Invent with Python blog\r\n3. Karan Goel\r\n4. Martyr2 thread\r\n5. Gianluca De Roma\r\n6. www.flaticon.com\r\n\r\nThanks alot guys!";
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    NavigateAway();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void NavigateAway()
        {
            NavigateUpTo(new Intent(this, typeof(CategoryActivity)));
            OverridePendingTransition(Resource.Animation.push_up_in, Resource.Animation.push_up_out);
        }

        public override void OnBackPressed()
        {
            NavigateAway();
            base.OnBackPressed();
        }
    }
}