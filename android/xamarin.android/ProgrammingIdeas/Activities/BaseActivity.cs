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

        // Bool that is used to know if the current toolbar should have an elevation or not.
        protected virtual bool ToolbarNoElevation => false;

        // Some activities won't have a toolbar and will set this to false to let us know not to inflate a toolbar.
        protected virtual bool SetupToolbar => true;

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(LayoutResource);
            if (SetupToolbar)
            {
                toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolbarNoElevation && Build.VERSION.SdkInt > BuildVersionCodes.Kitkat)
                    toolbar.Elevation = 0;
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(HomeAsUpEnabled);
            }
        }

        /// <summary>
        /// A method that is called when the back arrow in an activity's toolbar is pressed
        /// </summary>
        public virtual void OnBackArrowPressed() => NavigateAway();

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
        public virtual void NavigateAway()
        {
            Finish();
            OverridePendingTransition(Resource.Animation.push_right_in, Resource.Animation.push_right_out);
        }

        /// <summary>
        /// When the back button is pressed
        /// </summary>
        public override void OnBackPressed() => NavigateAway();
    }
}