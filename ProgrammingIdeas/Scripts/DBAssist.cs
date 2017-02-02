using Newtonsoft.Json;
using System.IO;

namespace ProgrammingIdeas.Helpers
{
    public static class DBAssist
    {
        public static object DeserializeDB<T>(string path, T type)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string data = r.ReadToEnd();
                return JsonConvert.DeserializeAnonymousType(data, type);
            }
        }

        public static void SerializeDB(string location, object database)
        {
            if (database != null)
            {
                using (StreamWriter s = new StreamWriter(location))
                    s.Write(JsonConvert.SerializeObject(database));
            }
        }
    }
}