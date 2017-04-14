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
        private ItemAdapter adapter;
        private List<Category> allItems = new List<Category>();
        private List<CategoryItem> itemsList;
        private List<CategoryItem> bookmarkedList;
        private ProgressBar progressBar;
        private string itemTitle, path, ideasdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ideasdb");
        private string progressingListPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "progressdb");

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
            progressBar = FindViewById<ProgressBar>(Resource.Id.completedIdeasBar);
            allItems = Global.Categories;
			progressBar.Max = allItems[Global.ItemScrollPosition].Items.Count;
            ShowProgress();
            bookmarkedList = JsonConvert.DeserializeObject<List<CategoryItem>>(DBAssist.DeserializeDB(path));
            setupMainIntent();
        }

        private void ShowProgress()
        {
            var completedCount = allItems[Global.ItemScrollPosition].Items.FindAll(x => x.State == "done").Count;
			progressBar.Progress = 0;
			progressBar.IncrementProgressBy(completedCount);
        }

        private void setupMainIntent()
        {
			string title = Global.Categories[Global.CategoryScrollPosition].CategoryLbl;
            itemTitle = title;
            itemsList = Global.Categories[Global.CategoryScrollPosition].Items;
            if (bookmarkedList != null && bookmarkedList.Count != 0)
            {
                foreach (CategoryItem item in bookmarkedList)
                    itemsList.Remove(itemsList.FirstOrDefault(x => x.Title == item.Title));
            }

            RunOnUiThread(() =>
            {
                Title = title;
                recyclerView.SetLayoutManager(manager);
                adapter = new ItemAdapter(itemsList, this);
                adapter.ItemClick += OnItemClick;
                recyclerView.SetAdapter(adapter);
                manager.ScrollToPosition(Global.ItemScrollPosition);
                adapter.StateClicked += (sender, e) =>
                {
                    var contents = e.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    int position = Convert.ToInt32(contents[0]);
                    string state = contents[1];
                    if (itemsList != null && itemsList.Count != 0)
                    {
                        itemsList[position].State = state;
                        adapter.NotifyItemChanged(position);
						ShowProgress();
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
					NavigateAway();
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
            Intent intent = new Intent(this, typeof(IdeaDetailsActivity));
            intent.PutExtra("title", Title); //item title
            intent.PutExtra("sender", "idealistactivity"); //one recycler view for bookmark activity and idea list activity so i need to know the sender
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.push_left_in, Resource.Animation.push_left_out);
        }

        public override void OnBackPressed()
        {
			NavigateAway();
            base.OnBackPressed();
        }

		void NavigateAway()
		{
            NavigateUpTo(new Intent(this, typeof(CategoryActivity)));
			OverridePendingTransition(Resource.Animation.push_right_in, Resource.Animation.push_right_out);
		}
    }
}