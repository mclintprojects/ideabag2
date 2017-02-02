using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.Widget;
using Android.Views;
using Newtonsoft.Json;
using ProgrammingIdeas.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProgrammingIdeas
{
	//Main activity.
    [Activity(Label = "Idea Bag 2", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        RecyclerView recyclerView;
        mAdapter adapter;
        LinearLayoutManager manager;
        List<Category> categoryList = new List<Category>();
        string notesdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "notesdb");
        string jsonString, ideasdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ideasdb");
        int scrollPosition = 0;
		bool wasFirstViewClickedMain;

         int[] icons = {Resource.Mipmap.numbers, Resource.Mipmap.text, Resource.Mipmap.network, Resource.Mipmap.enterprise,
                                       Resource.Mipmap.cpu, Resource.Mipmap.web, Resource.Mipmap.file, Resource.Mipmap.database,
                                       Resource.Mipmap.multimedia, Resource.Mipmap.games};

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.categoryactivity);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            scrollPosition = Intent.GetIntExtra("mainscrollPos", 0);
			wasFirstViewClickedMain = Intent.GetBooleanExtra("isHomeFirst", false);
            setupUI();
        }

        void setupUI() //first launch, gets json from packaged assets
        {
            if (!File.Exists(notesdb))
            {
                using (StreamWriter w = new StreamWriter(notesdb))
                    w.Write("");
            }
            AssetManager assets = Assets;
            using (StreamReader reader = new StreamReader(assets.Open("output.json")))
            {
                var prefManager = PreferenceManager.GetDefaultSharedPreferences(this);

                #region Code that copies database from Assets to internal storage

                /*var teditor = prefManager.Edit();
				teditor.PutBoolean("opened", false);
				teditor.Commit();*/

                #endregion Code that copies database from Assets to internal storage

                if (prefManager.GetBoolean("opened", false) == false) //first launch, copies ideas from app assets to internal storage
                {
                    jsonString = reader.ReadToEnd();
                    var editor = prefManager.Edit();
                    editor.PutBoolean("opened", true);
                    editor.Commit();
                    using (StreamWriter writer = new StreamWriter(ideasdb))
                        writer.Write(jsonString);
					categoryList = JsonConvert.DeserializeObject<List<Category>>(jsonString);
                }
                else //assets already copied to internal storage, proceeds to deserialize
                    categoryList = (List<Category>)DBAssist.DeserializeDB(ideasdb, categoryList);
                //jsonString = reader.ReadToEnd();
            }
            manager = new LinearLayoutManager(this);
            RunOnUiThread(() =>
            {
                recyclerView.SetLayoutManager(manager);
                adapter = new mAdapter(categoryList, this, icons, scrollPosition, wasFirstViewClickedMain);
                adapter.ItemClick += OnItemClick;
                recyclerView.SetAdapter(adapter);
                manager.ScrollToPosition(scrollPosition);
            });
        }

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.about:
                    Intent intentAbout = new Intent(this, typeof(About));
                    StartActivity(intentAbout);
                    OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
                    return true;

                case Resource.Id.bookmarks:
                    Intent intent = new Intent(this, typeof(Bookmarks));
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
                    return true;

                case Resource.Id.changelog:
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
					builder.SetTitle($"Changelog {Resources.GetString(Resource.String.appNumber)}");
                    builder.SetMessage("1. Added 4 new ideas.");
                    builder.SetPositiveButton("DISMISS", (sender, e) => { return; });
                    Dialog dialog = builder.Create();
                    dialog.Show();
                    return true;

                case Resource.Id.submitIdea:
                    Intent intentSubmit = new Intent(this, typeof(SubmitIdeaActivity));
                    StartActivity(intentSubmit);
                    OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
                    return true;

                case Resource.Id.newIdeas:
                    var manager = FragmentManager;
                    var transaction = manager.BeginTransaction();
                    transaction.Add(new NewIdeaFragment(GetNewIdeas()), "NewIdeaFragment");
                    transaction.Commit();
                    return true;

                case Resource.Id.notes:
                    Intent intentNotes = new Intent(this, typeof(NotesActivity));
                    StartActivity(intentNotes);
                    OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

		/// <summary>
		/// To indicate which ideas where new, I created a text file. A new idea was represented by {categoryIndex}-{ideaIndex}.
		/// So, "1-2" means that the idea is in Category 1 ie Numbers and was idea number 2 ie Tax Calculator.
		/// This method reads in the text file and generates the respective idea.
		/// </summary>
		/// <returns>The new ideas.</returns>
        List<CategoryItem> GetNewIdeas()
        {
            List<CategoryItem> newItems = new List<CategoryItem>();
            string newIdeas = new StreamReader(Assets.Open("newideasdb.txt")).ReadToEnd();
            var newIdeasContent = newIdeas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < newIdeasContent.Length; i++)
            {
                var sContents = newIdeasContent[i].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                newItems.Add(categoryList[Convert.ToInt32(sContents[0]) - 1].Items[Convert.ToInt32(sContents[1]) - 1]);
            }
            return newItems;
        }

        void OnItemClick(object sender, int position)
        {
			wasFirstViewClickedMain = true;
            var categoryItem = categoryList[position].Items;
            var title = categoryList[position].CategoryLbl;
            var itemsJson = JsonConvert.SerializeObject(categoryItem);
            Intent intent = new Intent(this, typeof(ItemActivity));
            intent.PutExtra("jsonString", itemsJson); //items in category json for item activity
            intent.PutExtra("title", title); //category title for item activity
            intent.PutExtra("sender", "MainActivity"); //get sender to fix ambiguity in items activity
            intent.PutExtra("mainscrollPos", position); //position to scroll to onresume
			intent.PutExtra("wasFirstViewClickedMain", wasFirstViewClickedMain); //if first view was clicked to handle double view highlight
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.push_left_in, Resource.Animation.push_left_out);
        }
    }
}