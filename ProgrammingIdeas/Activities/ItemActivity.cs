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
using ProgrammingIdeas.Activities;

namespace ProgrammingIdeas
{
    [Activity(Label = "ItemActivity", Theme = "@style/AppTheme")]
    public class ItemActivity : BaseActivity
    {
        private RecyclerView recyclerView;
        private RecyclerView.LayoutManager manager;
        private itemAdapter adapter;
        private List<Category> allItems = new List<Category>();
        private List<CategoryItem> itemsList;
        private List<CategoryItem> bookmarkedList;
        private string itemTitle, path, ideasdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ideasdb");
        private string progressingListPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "progressdb");
        private int count = 0, mainscrollPosition = 0, itemscrollPosition = 0;

		public override int LayoutResource
		{
			get
			{
				return Resource.Layout.idealistactivity;
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
            Global.IsWrittenDB = true;
            path = Path.Combine(Global.APP_PATH, "bookmarks.json");
            manager = new LinearLayoutManager(this);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.itemRecyclerView);
            //allItems = (List<Category>)DBAssist.GetDB(Assets, allItems);
            allItems = DBAssist.GetDB(ideasdb);
            bookmarkedList = JsonConvert.DeserializeObject<List<CategoryItem>>(DBAssist.DeserializeDB(path));
            setupMainIntent();
        }

        private void setupMainIntent()
        {
            string jsonString = Intent.GetStringExtra("jsonString");
            string title = Intent.GetStringExtra("title");
            itemTitle = title;
			itemsList = JsonConvert.DeserializeObject<List<CategoryItem>>(jsonString);
            count = itemsList.Count;
            if (bookmarkedList != null && bookmarkedList.Count != 0)
            {
                foreach (CategoryItem item in bookmarkedList)
                    itemsList.Remove(itemsList.FirstOrDefault(x => x.Title == item.Title));
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
                    Intent intent = new Intent(this, typeof(CategoryActivity));
                    intent.PutExtra("mainscrollPos", mainscrollPosition);
                    NavigateUpTo(intent);
                    OverridePendingTransition(Resource.Animation.push_right_in, Resource.Animation.push_right_out);
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
            Intent intent = new Intent(this, typeof(CategoryActivity));
            intent.PutExtra("mainscrollPos", mainscrollPosition);
            NavigateUpTo(intent);
            OverridePendingTransition(Resource.Animation.push_right_in, Resource.Animation.push_right_out);
            base.OnBackPressed();
        }
    }
}