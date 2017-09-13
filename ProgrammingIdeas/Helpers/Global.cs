using System;
using System.Collections.Generic;
using System.IO;

namespace ProgrammingIdeas
{
    public class Global
    {
        public static bool IsNewIdeasAvailable;
        public static bool LockRequests;
        public static bool RefreshBookmarks;
        public static int CategoryScrollPosition;
        public static int IdeaScrollPosition;
        public static int BookmarkScrollPosition;
        public static List<Category> Categories { get; set; }

        public static readonly string APP_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static readonly string NOTES_PATH = Path.Combine(APP_PATH, "notesdb");
        public static readonly string IDEAS_PATH = Path.Combine(APP_PATH, "ideasdb");
        public static readonly string BOOKMARKS_PATH = Path.Combine(APP_PATH, "bookmarks.json");
        public static readonly string NEWIDEASTXT_PATH = Path.Combine(APP_PATH, "newideastxt");
    }
}