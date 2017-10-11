using Android.App;
using Android.OS;
using Android.Runtime;
using Calligraphy;
using System;

namespace ProgrammingIdeas
{
    [Application]
    public class App : Application, Application.IActivityLifecycleCallbacks
    {
        private static Activity _currentActivity;
        public static Activity CurrentActivity => _currentActivity;

        public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
            .SetFontAttrId(Resource.Attribute.fontPath)
            .Build());
            RegisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            _currentActivity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            _currentActivity = activity;
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

        public static void Post(Action action) => _currentActivity.RunOnUiThread(action.Invoke);
    }
}