using Newtonsoft.Json;
using System.IO;
using Android.Content.Res;

namespace ProgrammingIdeas.Helpers
{
    public static class DBAssist
    {
        public static object DeserializeDB<T>(string path, T type)
        {
            try
            {
				using (StreamReader r = new StreamReader(new FileStream(path, FileMode.OpenOrCreate)))
                {
                    string data = r.ReadToEnd();
                    return JsonConvert.DeserializeAnonymousType(data, type);
                }
            }
            catch { return ""; }
        }

		public static object GetDB<T>(AssetManager manager, T type)
		{
			try
			{
				using (StreamReader r = new StreamReader(manager.Open("output.json")))
				{
					string data = r.ReadToEnd();
					return JsonConvert.DeserializeAnonymousType(data, type);
				}
			}
			catch { return ""; }
		}

        public static void SerializeDB(string location, object database)
        {
            try
            {
                if (database != null)
                {
					using (StreamWriter s = new StreamWriter(new FileStream(location, FileMode.OpenOrCreate)))
                        s.Write(JsonConvert.SerializeObject(database));
                }
            }
			catch { return; }
        }
    }
}