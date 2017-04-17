using Android.App;
using Android.Content.Res;
using Android.Runtime;
using Calligraphy;
using ProgrammingIdeas.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProgrammingIdeas
{
    [Application]
    public class App : Application
    {
        private static string ideasdb = Path.Combine(Global.APP_PATH, "ideasdb");

        public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
            .SetDefaultFontPath("fonts/RalewayRegular.ttf")
            .SetFontAttrId(Resource.Attribute.fontPath)
            .Build());
        }        public static void SetupDB(AssetManager manager)
        {
            if (System.IO.File.Exists(ideasdb))
            {
                var ideas = DBAssist.GetDB(ideasdb);
                Global.Categories = DBAssist.GetDB(ideasdb);
                if (NewIdeasAvailable(ideas, Global.Categories))
                {
                    //copy new ideas to saved ideas db. let the notes remain. use the new ideas txt file to add the new ideas
                    string newIdeas = new StreamReader(manager.Open("newideasdb.txt")).ReadToEnd();
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
        }        private static bool NewIdeasAvailable(List<Category> oldIdeas, List<Category> newIdeas)
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