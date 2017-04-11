using Newtonsoft.Json;
using System.IO;
using Android.Content.Res;
using System.Collections.Generic;

namespace ProgrammingIdeas.Helpers
{
    public static class DBAssist
    {
        public static string DeserializeDB(string path)
        {
            try
            {
				using (StreamReader r = new StreamReader(path))
                {
                    return r.ReadToEnd();
                }
            }
            catch { return ""; }
        }

		public static List<Category> GetDB(AssetManager manager)
		{
			try
			{
				using (StreamReader r = new StreamReader(manager.Open("output.json")))
				{
					var data = r.ReadToEnd();
					var list = JsonConvert.DeserializeObject<List<Category>>(data);
					return list;
				}
			}
			catch { return null; }
		}

		public static List<Category> GetDB(string path)
        {
			using (var r = new StreamReader(new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)))
			{
				var data = r.ReadToEnd();
				return JsonConvert.DeserializeObject<List<Category>>(data);
			}
        }

        public static void SerializeDB(string location, object database)
        {
            try
            {
                if (database != null)
                {
					using (StreamWriter s = new StreamWriter(location, false))
                        s.Write(JsonConvert.SerializeObject(database));
                }
            }
			catch { return; }
        }
    }
}