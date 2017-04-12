using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.IO;
using ProgrammingIdeas.Helpers;
using Newtonsoft.Json;

namespace ProgrammingIdeas.Activities
{
	[Activity(Label = "Programming Ideas 2", MainLauncher = true, Theme = "@style/AppFullscreen", Icon = "@mipmap/icon", NoHistory = true)]
	public class Splashscreen : AppCompatActivity
	{
		private string ideasdb = Path.Combine(Global.APP_PATH, "ideasdb");

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.splashscreen);
			Global.Categories = DBAssist.GetDB(Assets);
		}

		protected override void OnResume()
		{
			if (System.IO.File.Exists(ideasdb))
			{
				var ideas = DBAssist.GetDB(ideasdb);

				if (NewIdeasAvailable(ideas, Global.Categories))
				{
					//copy new ideas to saved ideas db. let the notes remain. use the new ideas txt file to add the new ideas
					string newIdeas = new StreamReader(Assets.Open("newideasdb.txt")).ReadToEnd();
					var newIdeasContent = newIdeas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < newIdeasContent.Length; i++)
					{
						var sContents = newIdeasContent[i].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
						var categoryIndex = Convert.ToInt32(sContents[0]) - 1;
						var itemIndex = Convert.ToInt32(sContents[1]) - 1;
						ideas[categoryIndex].Items.Add((Global.Categories[categoryIndex].Items[itemIndex]));
						ideas[categoryIndex].CategoryCount++;
					}
					DBAssist.SerializeDB(ideasdb, ideas);
					Global.Categories = ideas;
				}

				else
					Global.Categories = ideas;
			}
			else
			{
				//file does not exist, copy and save file
				DBAssist.SerializeDB(ideasdb, Global.Categories);
			}

            StartActivity(new Intent(this, typeof(CategoryActivity)));
			OverridePendingTransition(Resource.Animation.design_bottom_sheet_slide_in, Resource.Animation.design_bottom_sheet_slide_out);
			base.OnResume();
		}

		private bool NewIdeasAvailable(List<Category> oldIdeas, List<Category> newIdeas)
		{
			for (int i = 0; i < 10; i++)
			{
				if (newIdeas[i].CategoryCount > oldIdeas[i].CategoryCount)
					return true;
			}
			return false;
		}
	}
}