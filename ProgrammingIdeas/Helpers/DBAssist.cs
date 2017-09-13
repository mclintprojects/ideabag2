using Android.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProgrammingIdeas.Helpers
{
    public static class DBAssist
    {
        public async static Task<T> DeserializeDBAsync<T>(string path) where T : new()
        {
            try
            {
                using (StreamReader r = new StreamReader(path))
                {
                    var jsonString = await r.ReadToEndAsync();
                    return JsonConvert.DeserializeAnonymousType(jsonString, new T());
                }
            }
            catch { return default(T); }
        }

        public static T DeserializeDB<T>(string path) where T : new()
        {
            try
            {
                using (StreamReader r = new StreamReader(path))
                {
                    var jsonString = r.ReadToEnd();
                    return JsonConvert.DeserializeAnonymousType(jsonString, new T());
                }
            }
            catch { return default(T); }
        }

        public async static Task<List<Category>> GetDBAsync(string path)
        {
            using (var r = new StreamReader(new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)))
            {
                var data = await r.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<Category>>(data);
            }
        }

        public static void SerializeDBAsync(string location, object data)
        {
            Task.Run(() =>
            {
                try
                {
                    if (data != null)
                    {
                        using (StreamWriter s = new StreamWriter(location, false))
                            s.Write(JsonConvert.SerializeObject(data));
                    }
                }
                catch (Exception e)
                {
                    Log.Error("IDEABAG-DB", e.Message);
                }
            });
        }
    }
}