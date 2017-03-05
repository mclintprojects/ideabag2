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

namespace ProgrammingIdeas
{
    [Activity(Label = "ItemActivity", Theme = "@style/MyTheme")]
    public class ItemActivity : Activity
    {
        private RecyclerView recyclerView;
        private RecyclerView.LayoutManager manager;
        private itemAdapter adapter;
        private List<Category> allItems = new List<Category>();
		private List<CategoryItem> itemsList;
		private List<CategoryItem> bookmarkedList;
        private string itemTitle, path, ideasdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ideasdb");
        private int count = 0, mainscrollPosition = 0, itemscrollPosition = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.idealistactivity);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            itemscrollPosition = Intent.GetIntExtra("itemscrollPos", 0);
            mainscrollPosition = Intent.GetIntExtra("mainscrollPos", 0);
            path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "bookmarks.json");
            manager = new LinearLayoutManager(this);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.itemRecyclerView);
			allItems = (List<Category>)DBAssist.GetDB(Assets, allItems);
			bookmarkedList = (List<CategoryItem>)DBAssist.DeserializeDB(path, bookmarkedList);
            setupMainIntent();
        }

        private void setupMainIntent()
        {
            string jsonString = Intent.GetStringExtra("jsonString");
            string title = Intent.GetStringExtra("title");
            itemTitle = title;
			itemsList = allItems[mainscrollPosition].Items;
            count = itemsList.Count;
            if (bookmarkedList != null && bookmarkedList.Count != 0)
            {
                foreach (CategoryItem item in bookmarkedList)
                {
                    var bmkItem = itemsList.FirstOrDefault(x => x.Title == item.Title);
                    itemsList.Remove(bmkItem);
                }
            }

            RunOnUiThread(() =>
            {
                Title = title;
                recyclerView.SetLayoutManager(manager);
                adapter = new itemAdapter(itemsList, this, itemscrollPosition);
                adapter.ItemClick += OnItemClick;
                recyclerView.SetAdapter(adapter);
                manager.ScrollToPosition(itemscrollPosition);
                adapter.StateClicked += (sender, e) =>
                {
                    var contents = e.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    int position = Convert.ToInt32(contents[0]);
                    string state = contents[1];
                    if (itemsList != null && itemsList.Count != 0)
                    {
                        itemsList[position].State = state;
                        adapter.NotifyDataSetChanged();
                        allItems.FirstOrDefault(x => x.CategoryLbl == title).Items.FirstOrDefault(y => y.Description == itemsList[position].Description).State = state;
                        DBAssist.SerializeDB(ideasdb, allItems);
                    }
                    Toast.MakeText(this, $"Idea progress successfully changed.", ToastLength.Short).Show();
                };
            });
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Intent intent = new Intent(this, typeof(MainActivity));
                    intent.PutExtra("mainscrollPos", mainscrollPosition);
                    NavigateUpTo(intent);
                    OverridePendingTransition(Resource.Animation.push_right_in, Resource.Animation.push_right_out);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void OnItemClick(object sender, int position)
        {
            Intent intent = new Intent(this, typeof(ItemDetails));
            intent.PutExtra("item", JsonConvert.SerializeObject(itemsList[position]));
            intent.PutExtra("itemsListJson", JsonConvert.SerializeObject(itemsList)); //itemsList to be brought back by details activity
            intent.PutExtra("title", Title); //item title
            intent.PutExtra("sender", "idealistactivity"); //one recycler view for bookmark activity and idealistactivity so i need to know the sender
            intent.PutExtra("mainscrollPos", mainscrollPosition);
            intent.PutExtra("itemscrollPos", position);
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.push_left_in, Resource.Animation.push_left_out);
        }

        public override void OnBackPressed()
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("mainscrollPos", mainscrollPosition);
            NavigateUpTo(intent);
            OverridePendingTransition(Resource.Animation.push_right_in, Resource.Animation.push_right_out);
            base.OnBackPressed();
        }
    }
}