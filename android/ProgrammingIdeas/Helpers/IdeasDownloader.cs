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
    internal static class IdeasDownloader
    {
        private static List<Category> onlineIdeas;
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
                    onlineIdeas = JsonConvert.DeserializeObject<List<Category>>(payload);

                    var newideasdbResponse = await client.GetAsync(AppResources.NewIdeasDbLink);
                    if (newideasdbResponse.IsSuccessStatusCode)
                    {
                        newideastxt = await newideasdbResponse.Content.ReadAsStringAsync();
                        DBSerializer.SerializeDBAsync(Global.IDEAS_PATH, Global.Categories);
                        DBSerializer.SerializeDBAsync(Global.NEWIDEASTXT_PATH, newideastxt);
                        return Tuple.Create<List<Category>, Exception>(onlineIdeas, null);
                    }
                }

                // To prevent too many requests to server, only invalidate cache once when app is opening fresh from launcher
                Global.LockRequests = true;
            }
            catch (Exception e)
            {
                return Tuple.Create<List<Category>, Exception>(null, e);
            }

            return null;
        }

        /// <summary>
        /// If new ideas are available. Popup a notification showing how many new ideas were added.
        /// </summary>
        /// <param name="newIdeasCount"></param>
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

        /// <summary>
        /// Checks the cache and the online data idea count to see if new ideas have been added online
        /// </summary>
        /// <param name="oldIdeas">The offline cache of the ideas</param>
        /// <param name="newIdeas">The online repo of the ideas</param>
        /// <returns></returns>
        private static bool AreNewIdeasAvailable(List<Category> oldIdeas, List<Category> newIdeas)
        {
            for (int i = 0; i < oldIdeas.Count; i++)
            {
                if (newIdeas[i].CategoryCount > oldIdeas[i].CategoryCount)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Invalidates the offline ideas cache whether or not new ideas have been added
        /// </summary>
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
                        onlineIdeas = JsonConvert.DeserializeObject<List<Category>>(payload);
                        var newideasdbResponse = await client.GetAsync(AppResources.NewIdeasDbLink);
                        if (newideasdbResponse.IsSuccessStatusCode)
                        {
                            newideastxt = await newideasdbResponse.Content.ReadAsStringAsync();

                            if (AreNewIdeasAvailable(Global.Categories, onlineIdeas))
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

        // During app launch, we check the online data with the cached data. If anything new has been added online we invalidate the cache.
        private static async Task InvalidateOldData()
        {
            Log.Debug(TAG, "Starting full invalidation");

            var notesPath = Global.NOTES_PATH;
            if (!File.Exists(notesPath))
                File.Create(notesPath);

            var notes = await DBSerializer.DeserializeDBAsync<List<Note>>(notesPath);
            notes = notes ?? new List<Note>();

            // This is the code that does the actual invalidation
            for (int i = 0; i < onlineIdeas.Count; i++) // Looping through categories
            {
                for (int j = 0; j < onlineIdeas[i].Items.Count; j++) // Looping through ideas in each category
                {
                    var newItem = onlineIdeas[i].Items[j];
                    var oldItem = Global.Categories[i].Items.FirstOrDefault(x => x.Id == newItem.Id);

                    // We don't want to clear the user's notes for a particular idea during invalidation
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

            DBSerializer.SerializeDBAsync(Global.IDEAS_PATH, onlineIdeas);
            DBSerializer.SerializeDBAsync(Global.NEWIDEASTXT_PATH, newideastxt);
            Global.Categories = onlineIdeas;
            Global.LockRequests = true;

            Log.Debug(TAG, "Invalidation completed.");
        }
    }
}