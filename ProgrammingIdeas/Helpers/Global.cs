using System;
using System.Collections.Generic;
namespace ProgrammingIdeas
{
	public static class Global
	{
		public static bool IsWrittenDB { get; set; } = false;
		public static int CategoryScrollPosition;
		public static int ItemScrollPosition;
        public static int BookmarkScrollPosition;
        public static List<Category> Categories { get; set; }
		public static readonly string APP_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
	}
}
