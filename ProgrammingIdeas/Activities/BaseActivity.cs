using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
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
    }
}