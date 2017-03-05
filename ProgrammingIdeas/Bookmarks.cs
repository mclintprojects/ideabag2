using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using ProgrammingIdeas.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammingIdeas
{
    [Activity(Label = "Bookmarks")]
    public class Bookmarks : Activity
    {
        private List<Category> allItems = new List<Category>();
        private RecyclerView recyclerView;
        private RecyclerView.LayoutManager manager;
        private itemAdapter adapter;
        private ViewSwitcher parent;
        private List<CategoryItem> itemsList = new List<CategoryItem>();
        private string itemTitle, path, ideasdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ideasdb");
        private int scrollPosition = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.bookmarksactivity);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            scrollPosition = Intent.GetIntExtra("itemscrollPos", 0);
            path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "bookmarks.json");
            recyclerView = FindViewById<RecyclerView>(Resource.Id.bookmarkRecyclerView);
            parent = FindViewById<ViewSwitcher>(Resource.Id.parent);
            manager = new LinearLayoutManager(this);
            Task.Run(() => { setupUI(); });
        }

        private void setupUI()
        {
            if (File.Exists(path))
            {
				itemsList = JsonConvert.DeserializeObject<List<CategoryItem>>(DBAssist.DeserializeDB(path));
                allItems = (List<Category>)DBAssist.GetDB(Assets, allItems);
                if (itemsList.Count > 0)
                {
                    RunOnUiThread(() =>
                    {
                        adapter = new itemAdapter(itemsList, this, scrollPosition);
                        adapter.ItemClick += OnItemClick;
                        recyclerView.SetAdapter(adapter);
                        recyclerView.SetLayoutManager(manager);
                        manager.ScrollToPosition(scrollPosition);
                        adapter.StateClicked += StateClicked;
                    });
                }
                else
                    RunOnUiThread(() => { parent.ShowNext(); });
            }
            else
                ShowEmptyState();
        }

        private void ShowEmptyState()
        {
            RunOnUiThread(() =>
            {
                parent.ShowNext();
            });
        }

        private void StateClicked(object sender, string e)
        {
            var contents = e.Split(new char[] { '-' }, System.StringSplitOptions.RemoveEmptyEntries);
            int position = Convert.ToInt32(contents[0]);
            string state = contents[1];
            if (itemsList != null && itemsList.Count != 0)
            {
                itemsList[position].State = state;
                adapter.NotifyDataSetChanged();
                allItems.FirstOrDefault(x => x.CategoryLbl == itemsList[position].Category).Items[position].State = state;
                DBAssist.SerializeDB(ideasdb, allItems);
                DBAssist.SerializeDB(path, itemsList);
            }
            Toast.MakeText(this, "Idea progress successfully changed.", ToastLength.Short).Show();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Intent intent = new Intent(this, typeof(MainActivity));
                    NavigateUpTo(intent);
                    OverridePendingTransition(Resource.Animation.push_up_in, Resource.Animation.push_up_out);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void OnItemClick(object sender, int position)
        {
            Intent intent = new Intent(this, typeof(ItemDetails));
            intent.PutExtra("item", JsonConvert.SerializeObject(itemsList[position]));
            intent.PutExtra("itemsListJson", JsonConvert.SerializeObject(itemsList)); //itemsList to be brought back by details activity
            itemTitle = itemsList[position].Title;
            intent.PutExtra("title", itemsList[position].Category);
            intent.PutExtra("sender", "bmk");
            intent.PutExtra("itemscrollPos", position); //position to scroll to on activity create
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.push_left_in, Resource.Animation.push_left_out);
        }

        public override void OnBackPressed()
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            NavigateUpTo(intent);
            OverridePendingTransition(Resource.Animation.push_up_in, Resource.Animation.push_up_out);
            base.OnBackPressed();
        }
    }
}