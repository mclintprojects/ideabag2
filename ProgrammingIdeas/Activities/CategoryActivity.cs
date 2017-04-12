using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using ProgrammingIdeas.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using ProgrammingIdeas.Activities;

namespace ProgrammingIdeas
{
    //Main activity.
    [Activity(Label = "Idea Bag 2", Theme = "@style/AppTheme")]
    public class CategoryActivity : BaseActivity
    {
        private RecyclerView recyclerView;
        private mAdapter adapter;
        private LinearLayoutManager manager;
        private List<Category> categoryList = new List<Category>();
        private string notesdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "notesdb");
        private string ideasdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ideasdb");
        private int scrollPosition;

        private int[] icons = {Resource.Mipmap.numbers, Resource.Mipmap.text, Resource.Mipmap.network, Resource.Mipmap.enterprise,
                                       Resource.Mipmap.cpu, Resource.Mipmap.web, Resource.Mipmap.file, Resource.Mipmap.database,
                                       Resource.Mipmap.multimedia, Resource.Mipmap.games};

		public override int LayoutResource
		{
			get
			{
				return Resource.Layout.categoryactivity;
			}
		}

		public override bool HomeAsUpEnabled
		{
			get
			{
				return false;
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
			categoryList = Global.Categories;
            setupUI();
        }

        private void setupUI() //first launch, gets json from packaged assets
        {
            manager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(manager);
            adapter = new mAdapter(categoryList, this, icons, Global.CategoryScrollPosition);
            adapter.ItemClick += OnItemClick;
            recyclerView.SetAdapter(adapter);
			manager.ScrollToPosition(Global.CategoryScrollPosition);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.about:
                    var intentAbout = new Intent(this, typeof(AboutActivity));
                    StartActivity(intentAbout);
                    OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
                    return true;

                case Resource.Id.bookmarks:
                    var intent = new Intent(this, typeof(BookmarksActivity));
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
                    return true;

                case Resource.Id.changelog:
                    var builder = new AlertDialog.Builder(this);
                    builder.SetTitle($"Changelog {Resources.GetString(Resource.String.appNumber)}");
                    builder.SetMessage(Resources.GetString(Resource.String.changelog));
                    builder.SetPositiveButton("DISMISS", (sender, e) => { return; });
                    Dialog dialog = builder.Create();
                    dialog.Show();
                    return true;

                case Resource.Id.submitIdea:
                    var intentSubmit = new Intent(this, typeof(SubmitIdeaActivity));
                    StartActivity(intentSubmit);
                    OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
                    return true;

                case Resource.Id.newIdeas:
                    var fragmentManager = FragmentManager;
                    var transaction = fragmentManager.BeginTransaction();
                    transaction.Add(new NewIdeaFragment(GetNewIdeas()), "NewIdeaFragment");
                    transaction.Commit();
                    return true;

                case Resource.Id.notes:
                    var intentNotes = new Intent(this, typeof(NotesActivity));
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
        private List<CategoryItem> GetNewIdeas()
        {
            var newItems = new List<CategoryItem>();
            var newIdeas = new StreamReader(Assets.Open("newideasdb.txt")).ReadToEnd();
            var newIdeasContent = newIdeas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < newIdeasContent.Length; i++)
            {
                var sContents = newIdeasContent[i].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                newItems.Add(categoryList[Convert.ToInt32(sContents[0]) - 1].Items[Convert.ToInt32(sContents[1]) - 1]);
            }
            return newItems;
        }

        private void OnItemClick(object sender, int position)
        {
            var categoryItem = categoryList[position].Items;
            var title = categoryList[position].CategoryLbl;
            var itemsJson = JsonConvert.SerializeObject(categoryItem);
            Intent intent = new Intent(this, typeof(ItemActivity));
            intent.PutExtra("jsonString", itemsJson); //items in category json for item activity
            intent.PutExtra("title", title); //category title for item activity
            intent.PutExtra("sender", "MainActivity"); //get sender to fix ambiguity in items activity
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.push_left_in, Resource.Animation.push_left_out);
        }
    }
}