using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Util;
using Newtonsoft.Json;
using ProgrammingIdeas.Activities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProgrammingIdeas.Helpers
{
    internal class DBManager
    {
        private static List<Category> newDB;
        private static string newideastxt;
        private static HttpClient client = new HttpClient() { Timeout = TimeSpan.FromMilliseconds(12000) };
        private const string TAG = "ALANSADEBUG";

        public static async Task<Tuple<List<Category>, Exception>> DownloadIdeasFromGoogleDrive()
        {
            try
            {
                var response = await client.GetAsync(AppResources.DbLink);
                if (response.IsSuccessStatusCode)
                {
                    var payload = await response.Content.ReadAsStringAsync();
                    newDB = JsonConvert.DeserializeObject<List<Category>>(payload);

                    var newideasdbResponse = await client.GetAsync(AppResources.NewIdeasDbLink);
                    if (newideasdbResponse.IsSuccessStatusCode)
                    {
                        newideastxt = await newideasdbResponse.Content.ReadAsStringAsync();
                        DBAssist.SerializeDBAsync(Global.IDEAS_PATH, Global.Categories);
                        DBAssist.SerializeDBAsync(Global.NEWIDEASTXT_PATH, newideastxt);
                        return Tuple.Create<List<Category>, Exception>(newDB, null);
                    }
                }

                // To prevent too many requests to server, only invalidate cache if app is opening fresh from launcher
                Global.LockRequests = true;
            }
            catch (TaskCanceledException e)
            {
                return Tuple.Create<List<Category>, Exception>(null, e);
            }
            catch (HttpRequestException e)
            {
                return Tuple.Create<List<Category>, Exception>(null, e);
            }
            catch (Exception e)
            {
                return Tuple.Create<List<Category>, Exception>(null, e);
            }

            return null;
        }

        private static void ShowNewIdeasAvailableNotification(int newIdeasCount)
        {
            var activity = App.CurrentActivity;
            activity.RunOnUiThread(() =>
            {
                string notifContent = newIdeasCount == 1 ? $"1 new idea is available." : $"{newIdeasCount} new ideas are available.";
                var intent = new Intent(activity, typeof(CategoryActivity));
                intent.PutExtra("NewIdeasNotif", true);
                var pendingIntent = PendingIntent.GetActivity(activity, 1960, intent, PendingIntentFlags.UpdateCurrent);

                var notif = new NotificationCompat.Builder(activity)
                                .SetContentTitle("New ideas available.")
                                .SetContentText(notifContent)
                                .SetContentIntent(pendingIntent)
                                .SetSmallIcon(Resource.Mipmap.notif_icon)
                                .SetAutoCancel(true)
                                .Build();

                var notifManager = (NotificationManager)activity.GetSystemService(Context.NotificationService);
                notifManager.Notify(1957, notif);
            });
        }

        private static bool AreNewIdeasAvailable(List<Category> oldIdeas, List<Category> newIdeas)
        {
            for (int i = 0; i < oldIdeas.Count; i++)
            {
                if (newIdeas[i].CategoryCount > oldIdeas[i].CategoryCount)
                    return true;
            }
            return false;
        }

        public static async void StartLowkeyInvalidation()
        {
            try
            {
                if (!Global.LockRequests)
                {
                    Log.Debug(TAG, "Starting lowkey invalidation");
                    //TestNotify(); // Use this method to test if notifications work
                    var response = await client.GetAsync(AppResources.DbLink);
                    if (response.IsSuccessStatusCode)
                    {
                        var payload = await response.Content.ReadAsStringAsync();
                        newDB = JsonConvert.DeserializeObject<List<Category>>(payload);
                        var newideasdbResponse = await client.GetAsync(AppResources.NewIdeasDbLink);
                        if (newideasdbResponse.IsSuccessStatusCode)
                        {
                            newideastxt = await newideasdbResponse.Content.ReadAsStringAsync();

                            if (AreNewIdeasAvailable(Global.Categories, newDB))
                            {
                                Log.Debug(TAG, "New ideas available");
                                var newIdeasContent = newideastxt.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                ShowNewIdeasAvailableNotification(newIdeasContent.Length);
                            }

                            InvalidateOldData();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(TAG, e.Message);
            }
        }

        private static void TestNotify()
        {
            var newIdeasContent = new[] { "2-30", "1, 20" };
            ShowNewIdeasAvailableNotification(newIdeasContent.Length);
        }

        private static async void InvalidateOldData()
        {
            Log.Debug(TAG, "Starting full invalidation");

            var notesPath = Path.Combine(Global.APP_PATH, "notesdb");
            if (!File.Exists(notesPath))
                File.Create(notesPath);

            var notes = await DBAssist.DeserializeDBAsync<List<Note>>(notesPath);
            notes = notes ?? new List<Note>();

            for (int i = 0; i < newDB.Count; i++)
            {
                for (int j = 0; j < newDB[i].Items.Count; j++)
                {
                    var newItem = newDB[i].Items[j];
                    var oldItem = Global.Categories[i].Items.FirstOrDefault(x => x.Id == newItem.Id);
                    Note note = null;
                    if (oldItem != null)
                    {
                        note = notes.FirstOrDefault(x => x.Title == oldItem.Title);
                        if (note != null)
                            Log.Debug(TAG, $"Note *{note.Title}*, found for old idea *{oldItem.Title}* placed at new idea *{newItem.Title}*");
                    }

                    newItem.Note = note;
                    newItem.State = oldItem?.State;
                }
            }

            DBAssist.SerializeDBAsync(Global.IDEAS_PATH, newDB);
            DBAssist.SerializeDBAsync(Global.NEWIDEASTXT_PATH, newideastxt);
            Global.Categories = newDB;
            Global.LockRequests = true;

            Log.Debug(TAG, "Invalidation completed.");
        }
    }
}