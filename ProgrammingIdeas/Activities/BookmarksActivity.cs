using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using ProgrammingIdeas.Activities;
using ProgrammingIdeas.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammingIdeas
{
    [Activity(Label = "Bookmarks", Theme = "@style/AppTheme")]
    public class BookmarksActivity : BaseActivity
    {
        private List<Category> allItems;
        private RecyclerView recyclerView;
		private LinearLayoutManager manager;
        private ItemAdapter adapter;
        private List<CategoryItem> bookmarksList = new List<CategoryItem>();
        private string itemTitle, path, ideasdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ideasdb");
        private int scrollPosition = 0;

        public override int LayoutResource
        {
            get
            {
                return Resource.Layout.bookmarksactivity;
            }
        }

        public override bool HomeAsUpEnabled
        {
            get
            {
                return true;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "bookmarks.json");
            recyclerView = FindViewById<RecyclerView>(Resource.Id.bookmarkRecyclerView);
            manager = new LinearLayoutManager(this);
            Task.Run(() => { setupUI(); });
        }

        private void setupUI()
        {
            if (File.Exists(path))
            {
                bookmarksList = JsonConvert.DeserializeObject<List<CategoryItem>>(DBAssist.DeserializeDB(path));
                allItems = DBAssist.GetDB(ideasdb);
                if (bookmarksList != null && bookmarksList.Count > 0)
                {
                    RunOnUiThread(() =>
                    {
                        adapter = new ItemAdapter(bookmarksList, this);
                        adapter.ItemClick += OnItemClick;
                        recyclerView.SetAdapter(adapter);
                        recyclerView.SetLayoutManager(manager);
                        manager.ScrollToPosition(scrollPosition);
                        adapter.StateClicked += StateClicked;
                    });
                }
            }
        }

        private void StateClicked(object sender, string e)
        {
            var contents = e.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int position = Convert.ToInt32(contents[0]);
            string state = contents[1];
            if (bookmarksList != null && bookmarksList.Count != 0)
            {
                bookmarksList[position].State = state;
                adapter.NotifyDataSetChanged();
                allItems.FirstOrDefault(x => x.CategoryLbl == bookmarksList[position].Category).Items[position].State = state;
                DBAssist.SerializeDB(path, bookmarksList);
            }
            Toast.MakeText(this, "Idea progress successfully changed.", ToastLength.Short).Show();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Intent intent = new Intent(this, typeof(CategoryActivity));
                    NavigateUpTo(intent);
                    OverridePendingTransition(Resource.Animation.push_up_in, Resource.Animation.push_up_out);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnPause()
        {
            DBAssist.SerializeDB(ideasdb, allItems);
            base.OnPause();
        }

        private void OnItemClick(object sender, int position)
        {
            Global.ItemScrollPosition = position;
            Intent intent = new Intent(this, typeof(ItemDetails));
            intent.PutExtra("item", JsonConvert.SerializeObject(bookmarksList[position]));
            intent.PutExtra("itemsListJson", JsonConvert.SerializeObject(bookmarksList)); //itemsList to be brought back by details activity
            itemTitle = bookmarksList[position].Title;
            intent.PutExtra("title", bookmarksList[position].Category);
            intent.PutExtra("sender", "bmk");
            intent.PutExtra("itemscrollPos", position); //position to scroll to on activity create
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.push_left_in, Resource.Animation.push_left_out);
        }

        public override void OnBackPressed()
        {
            Intent intent = new Intent(this, typeof(CategoryActivity));
            NavigateUpTo(intent);
            OverridePendingTransition(Resource.Animation.push_up_in, Resource.Animation.push_up_out);
            base.OnBackPressed();
        }
    }
}