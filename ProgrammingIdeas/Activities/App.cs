using System;
using Android.App;
using Android.Runtime;
using Calligraphy;

namespace ProgrammingIdeas
{
	[Application]
	public class App : Application
	{
		public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public override void OnCreate()
		{
			base.OnCreate();
			CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
			.SetDefaultFontPath("fonts/RubikRegular.ttf")
			.SetFontAttrId(Resource.Attribute.fontPath)
			.Build());
		}	}
}
