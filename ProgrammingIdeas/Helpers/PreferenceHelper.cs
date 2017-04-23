using System;
using Android.App;
using Android.Content;

namespace Helpers
{
	public class PreferenceHelper
	{
		private static Activity context;
		private static ISharedPreferencesEditor editor;
		private static ISharedPreferences pref;

		public static void Init(Activity ctx)
		{
			context = ctx;
			pref = context.GetPreferences(FileCreationMode.Private);
			editor = pref.Edit();
		}

		public static void PutBoolean(string tag, bool value)
		{
			editor.PutBoolean(tag, value);
            editor.Commit();
		}

		public static bool GetBoolean(string tag, bool defaultValue)
		{
			return pref.GetBoolean(tag, defaultValue);
		}
	}
}
