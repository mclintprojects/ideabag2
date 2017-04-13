using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using ProgrammingIdeas.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProgrammingIdeas.Activities;
using System;

namespace ProgrammingIdeas
{
    [Activity(Label = "Idea Details", Theme = "@style/AppTheme")]
    public class ItemDetails : BaseActivity
    {
        private List<Category> db;
        private List<Note> notes;
        private List<CategoryItem> itemsList;
        private List<CategoryItem> bookmarkedItems = new List<CategoryItem>();
        private CategoryItem item;
        private string itemTitle, noteText, path, sendingActivity, ideasdb = Path.Combine(Global.APP_PATH, "ideasdb");
        private string notesdb = Path.Combine(Global.APP_PATH, "notesdb");
        private IMenuItem bookmarkIcon;
        private CardView ideaCard;
        private TextView title, itemDescription;
        private OnSwipeListener SwipeListener;

        public override int LayoutResource
        {
            get
            {
                return Resource.Layout.ideadetailsactivity;
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
            path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "bookmarks.json");
            base.OnCreate(savedInstanceState);
            sendingActivity = Intent.GetStringExtra("sender");
        }

		protected override void OnResume()
		{
            item = Global.Categories[Global.CategoryScrollPosition].Items[Global.ItemScrollPosition];
            itemsList = Global.Categories[Global.CategoryScrollPosition].Items;
            title = FindViewById<TextView>(Resource.Id.itemTitle);
            itemDescription = FindViewById<TextView>(Resource.Id.itemDescription);
            ideaCard = FindViewById<CardView>(Resource.Id.ideaCard);

            SwipeListener = new OnSwipeListener(this);
			ideaCard.SetOnTouchListener(SwipeListener);
            SwipeListener.OnSwipeRight += SwipeListener_OnSwipeRight;
            SwipeListener.OnSwipeLeft += SwipeListener_OnSwipeLeft;
            itemDescription.SetOnTouchListener(SwipeListener);

            title.Text = item.Title;
            itemDescription.Text = item.Description;
            db = DBAssist.GetDB(ideasdb);
            notes = JsonConvert.DeserializeObject<List<Note>>(DBAssist.DeserializeDB(notesdb));

            using (BusyHandler.Handle(RemoveBookmarkedItems))
                Task.Run(() =>
                {
                    bookmarkedItems = JsonConvert.DeserializeObject<List<CategoryItem>>(DBAssist.DeserializeDB(path));
                    if (bookmarkedItems == null)
                        bookmarkedItems = new List<CategoryItem>();
                });
			base.OnResume();
		}

        private void SwipeListener_OnSwipeRight(object sender, EventArgs e)
        {
            ChangeItem(Global.ItemScrollPosition - 1);
        }

        private void SwipeListener_OnSwipeLeft(object sender, EventArgs e)
        {
            ChangeItem(Global.ItemScrollPosition + 1);
        }

        private void ChangeItem(int index)
        {
            if (index >= 0 && index <= itemsList.Count - 1)
            {
                Global.ItemScrollPosition = index;
                item = itemsList[index];
                title.Text = item.Title;
                itemDescription.Text = item.Description;
            }
        }

        private void RemoveBookmarkedItems()
        {
            if (bookmarkedItems.Count > 0)
            {
                foreach (CategoryItem item in bookmarkedItems)
                    itemsList.Remove(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.itemdetails_menu, menu);
            bookmarkIcon = menu.FindItem(Resource.Id.bookmarkItem);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    GoAway();
                    return true;

                case Resource.Id.bookmarkItem:
                    return true;

                case Resource.Id.shareItem:
                    var share = new Intent();
                    share.SetAction(Intent.ActionSend);
                    share.SetType("text/plain");
                    string textToShare = $"Can you code this challenge?\r\n\r\n" +
                        $"Title: {this.item.Title}\r\nDifficulty: {this.item.Difficulty}\r\n\r\n{this.item.Description}\r\n\r\n" +
                        $"Want more coding ideas? Get the app here: https://play.google.com/store/apps/details?id=com.alansa.ideabag2";
                    share.PutExtra(Intent.ExtraText, textToShare);
                    StartActivity(Intent.CreateChooser(share, "Share idea via"));
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void writeEntirety()
        {
            DBAssist.SerializeDB(path, bookmarkedItems);
            DBAssist.SerializeDB(ideasdb, db);
            DBAssist.SerializeDB(notesdb, notes);
        }

        public override void OnBackPressed()
        {
            GoAway();
            base.OnBackPressed();
        }

        protected override void OnPause()
        {
            writeEntirety();
            base.OnPause();
        }

        private void GoAway()
        {
            writeEntirety();
            var intent = new Intent(this, sendingActivity == "idealistactivity" ? typeof(ItemActivity) : typeof(BookmarksActivity));
            intent.PutExtra("jsonString", JsonConvert.SerializeObject(itemsList)); //itemlist json for itemactivity header
            intent.PutExtra("title", itemTitle); //title for itemactivity header
            NavigateUpTo(intent);
            OverridePendingTransition(Resource.Animation.push_right_in, Resource.Animation.push_right_out);
        }
    }
}