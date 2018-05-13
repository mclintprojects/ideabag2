using Android.Widget;
using Helpers;
using Newtonsoft.Json;
using ProgrammingIdeas.Api;
using ProgrammingIdeas.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProgrammingIdeas
{
    public class Global
    {
        /// <summary>
        /// True if we checked the online data and new ideas have been added to it
        /// </summary>
        public static bool IsNewIdeasAvailable;

        /// <summary>
        /// True if we have checked online whether new data was available
        /// </summary>
        public static bool LockRequests;

        public static bool RefreshBookmarks;

        /// <summary>
        /// Usually holds the index of the last viewed idea category.
        /// Allows us to set a background highlight for it in the list of idea category
        /// </summary>
        public static int CategoryScrollPosition;

        /// <summary>
        /// Usually holds the index of the last viewed idea.
        /// Allows us to set a background highlight for it in the list of ideas
        /// </summary>
        public static int IdeaScrollPosition;

        /// <summary>
        /// Usually holds the index of the last viewed bookmarked idea.
        /// Allows us to set a background highlight for it in the list of bookmarked ideas
        /// </summary>
        public static int BookmarkScrollPosition;

        /// <summary>
        /// Holds all the idea categories and their ideas
        /// </summary>
        public static List<Category> Categories { get; set; }

        public static LoginResponseData LoginData { get; set; }

        public static readonly string APP_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static readonly string NOTES_PATH = Path.Combine(APP_PATH, "notesdb");
        public static readonly string IDEAS_PATH = Path.Combine(APP_PATH, "ideasdb");
        public static readonly string BOOKMARKS_PATH = Path.Combine(APP_PATH, "bookmarks.json");
        public static readonly string NEWIDEASTXT_PATH = Path.Combine(APP_PATH, "newideastxt");

        internal static void AuthUser(LoginResponseData payload)
        {
            LoginData = payload;
            IdeaBagApi.Instance.SetAuthToken(payload.Token);
            PreferenceManager.Instance.AddEntry("loginData", JsonConvert.SerializeObject(payload));

            Toast.MakeText(App.CurrentActivity, "Logged in successfully.", ToastLength.Long).Show();
        }

        internal static void Logout()
        {
            PreferenceManager.Instance.AddEntry("loginData", string.Empty);
            PreferenceManager.Instance.AddEntry("expiresIn", string.Empty);
            LoginData = null;

            Toast.MakeText(App.CurrentActivity, "Logged out successfully.", ToastLength.Long).Show();
        }
    }
}