using Android.Content;
using ProgrammingIdeas;

namespace Helpers
{
    public class PreferenceManager
    {
        private readonly ISharedPreferencesEditor editor;
        private readonly ISharedPreferences pref;

        private static PreferenceManager singleton;

        public static PreferenceManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new PreferenceManager();
                    return singleton;
                }
                return singleton;
            }
        }

        public PreferenceManager()
        {
            pref = App.CurrentActivity.GetPreferences(FileCreationMode.Private);
            editor = pref.Edit();
        }

        public void AddEntry(string tag, bool value)
        {
            editor.PutBoolean(tag, value);
            editor.Commit();
        }

        public void AddEntry(string tag, int value)
        {
            editor.PutInt(tag, value);
            editor.Commit();
        }

        public bool GetEntry(string tag, bool defaultValue) => pref.GetBoolean(tag, defaultValue);

        public int GetEntry(string tag, int defaultValue) => pref.GetInt(tag, defaultValue);
    }
}