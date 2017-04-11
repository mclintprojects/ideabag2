using System;
using System.IO;
namespace ProgrammingIdeas
{
	public static class Global
	{
		public static bool IsWrittenDB { get; set; } = false;
		public static string APP_PATH { get { return Environment.GetFolderPath(Environment.SpecialFolder.Personal); }}
		public static int CategoryScrollPosition;
		public static int ItemScrollPosition;
	}
}
