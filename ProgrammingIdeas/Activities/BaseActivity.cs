using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Calligraphy;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace ProgrammingIdeas.Activities
{
    public abstract class BaseActivity : AppCompatActivity
    {
        public abstract int LayoutResource { get; }
        public abstract bool HomeAsUpEnabled { get; }
        private Toolbar toolbar;

        public Toolbar Toolbar { get { return toolbar; } }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(LayoutResource);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(HomeAsUpEnabled);
            base.OnCreate(savedInstanceState);
        }

        /// <summary>
        /// A method that is called when the back arrow in an activity's toolbar is pressed
        /// </summary>
        public virtual void OnBackArrowPressed()
        {
            NavigateAway();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackArrowPressed();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// Finishes the current activity and animates the transition to the previous activity.
        /// </summary>
        public void NavigateAway()
        {
            Finish();
            OverridePendingTransition(Resource.Animation.push_right_in, Resource.Animation.push_right_out);
        }

        public override void OnBackPressed()
        {
            NavigateAway();
            base.OnBackPressed();
        }
    }
}