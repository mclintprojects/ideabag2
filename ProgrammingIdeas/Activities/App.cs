using Android.App;
using Android.Runtime;
using Calligraphy;
using System;
using System.IO;
using Android.OS;

namespace ProgrammingIdeas
{
    [Application]
    public class App : Application, Application.IActivityLifecycleCallbacks
	{
		private static Activity currentActivity;
		public static Activity CurrentActivity { get { return currentActivity; } }

        public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
            //.SetDefaultFontPath("fonts/RobotoBlack.ttf")
            .SetFontAttrId(Resource.Attribute.fontPath)
            .Build());
			RegisterActivityLifecycleCallbacks(this);
        }

		public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
		{
			currentActivity = activity;
		}

		public void OnActivityDestroyed(Activity activity)
		{
			
		}

		public void OnActivityPaused(Activity activity)
		{
			
		}

		public void OnActivityResumed(Activity activity)
		{
			currentActivity = activity;
		}

		public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
		{
		}

		public void OnActivityStarted(Activity activity)
		{
			
		}

		public void OnActivityStopped(Activity activity)
		{
			
		}
	}
}