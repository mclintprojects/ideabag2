using Android.App;
using Android.Support.Design.Widget;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace ProgrammingIdeas.Helpers
{
    internal class CloudDB
    {
        private static string oldDBPath = Path.Combine(Global.APP_PATH, "ideasdb");
        private static string newideastxtPath = Path.Combine(Global.APP_PATH, "newideastxt");
        private static List<Category> newDB;
        private static string newideastxt;
        private static HttpClient client = new HttpClient();

        public static async Task Startup(Action retryMethod, Snackbar snack)
        {
			try
			{
				var source = new CancellationTokenSource();
				source.CancelAfter(12000);
				if (!File.Exists(oldDBPath)) // First launch, as in just installed app and launched it
				{
					var response = await client.GetAsync(AppResources.DbLink, source.Token);
					if (response.IsSuccessStatusCode)
					{
						var payload = await response.Content.ReadAsStringAsync();
						newDB = JsonConvert.DeserializeObject<List<Category>>(payload);
						var newideasdbResponse = await client.GetAsync(AppResources.NewIdeasDbLink);
						if (newideasdbResponse.IsSuccessStatusCode)
						{
							newideastxt = await newideasdbResponse.Content.ReadAsStringAsync();
							Global.Categories = newDB;
							DBAssist.SerializeDB(oldDBPath, Global.Categories);
							DBAssist.SerializeDB(newideastxtPath, newideastxt);
						}
					}
				}
				else // Not first launch, checking if new ideas available and invalidating old ideas if true
					StartLowkeyInvalidation();

				// To prevent too many requests to server, only invalidate cache if app is opening fresh from launcher
				Global.LockRequests = true;
			}
			catch (TaskCanceledException)
			{
				snack.SetText("Can't load ideas. Your connection might be too slow.").SetAction("Retry", (v) => retryMethod?.Invoke()).Show();
			}
            catch (HttpRequestException)
            {
                snack.SetText("Can't load ideas. Check your connection.").SetAction("Retry", (v) => retryMethod?.Invoke()).Show();
            }
            catch (Exception e)
            {
                snack.SetText($"Oops! {e.Message}.").SetAction("Retry", (v) => retryMethod?.Invoke()).Show();
            }
        }

        private static bool NewIdeasAvailable(List<Category> oldIdeas, List<Category> newIdeas)
        {
            for (int i = 0; i < oldIdeas.Count; i++)
            {
                if (newIdeas[i].CategoryCount > oldIdeas[i].CategoryCount)
                    return true;
            }
            return false;
        }

        private static async void StartLowkeyInvalidation()
        {
            try
            {
                if (!Global.LockRequests)
                {
#if DEBUG
                    Toast.MakeText(Application.Context, "Starting lowkey invalidation", ToastLength.Long).Show();
#endif
                    Global.Categories = DBAssist.GetDB(oldDBPath);
                    var response = await client.GetAsync(AppResources.DbLink);
                    if (response.IsSuccessStatusCode)
                    {
                        var payload = await response.Content.ReadAsStringAsync();
                        newDB = JsonConvert.DeserializeObject<List<Category>>(payload);
                        var newideasdbResponse = await client.GetAsync(AppResources.NewIdeasDbLink);
                        if (newideasdbResponse.IsSuccessStatusCode)
                        {
                            newideastxt = await newideasdbResponse.Content.ReadAsStringAsync();
                            if (NewIdeasAvailable(Global.Categories, newDB))
                                InvalidateOldDB();
                        }
                    }
                }
            }
            catch { }
        }

        private static void InvalidateOldDB()
        {
            var newIdeasContent = newideastxt.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < newIdeasContent.Length; i++)
            {
                var sContents = newIdeasContent[i].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var categoryIndex = Convert.ToInt32(sContents[0]) - 1;
                var itemIndex = Convert.ToInt32(sContents[1]) - 1;
                Global.Categories[categoryIndex].Items.Add((newDB[categoryIndex].Items[itemIndex]));
                Global.Categories[categoryIndex].CategoryCount++;
            }
            DBAssist.SerializeDB(oldDBPath, Global.Categories);
            DBAssist.SerializeDB(newideastxtPath, newideastxt);
			Global.IsNewIdeasAvailable = true;
        }
    }
}