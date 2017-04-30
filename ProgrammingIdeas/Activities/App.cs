using Android.App;
using Android.Runtime;
using Calligraphy;
using System;
using System.IO;

namespace ProgrammingIdeas
{
    [Application]
    public class App : Application
    {
        private static string ideasdb = Path.Combine(Global.APP_PATH, "ideasdb");

        public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
            .SetDefaultFontPath("fonts/RalewayRegular.ttf")
            .SetFontAttrId(Resource.Attribute.fontPath)
            .Build());
        }
    }
}