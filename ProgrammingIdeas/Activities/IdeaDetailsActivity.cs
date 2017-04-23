using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Newtonsoft.Json;
using ProgrammingIdeas.Animation;
using ProgrammingIdeas.Fragments;
using ProgrammingIdeas.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProgrammingIdeas.Activities
{
    [Activity(Label = "Idea details", Theme = "@style/AppTheme")]
    public class IdeaDetailsActivity : BaseActivity
    {
        private List<Category> db;
        private List<Note> notes;
        private List<CategoryItem> itemsList;
        private List<CategoryItem> bookmarkedItems = new List<CategoryItem>();
        private CategoryItem item;
        private string path, sendingActivity, ideasdb = Path.Combine(Global.APP_PATH, "ideasdb");
        private string notesdb = Path.Combine(Global.APP_PATH, "notesdb");
        private IMenuItem bookmarkIcon;
        private TextView title, itemDescription, noteContent;
        private OnSwipeListener SwipeListener;
        private LinearLayout detailsView;
        private FloatingActionButton addNoteFab;
        private Button editNoteBtn;
        private CardView noteHolder;
        private bool IsBookmarked, IsInBookmarkMode;

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
            title = FindViewById<TextView>(Resource.Id.itemTitle);
            itemDescription = FindViewById<TextView>(Resource.Id.itemDescription);
            detailsView = FindViewById<LinearLayout>(Resource.Id.detailsView);
            addNoteFab = FindViewById<FloatingActionButton>(Resource.Id.addNotefab);
            editNoteBtn = FindViewById<Button>(Resource.Id.editNoteBtn);
            noteHolder = FindViewById<CardView>(Resource.Id.noteHolder);
            noteContent = FindViewById<TextView>(Resource.Id.noteContent);

            addNoteFab.Click += AddNoteFab_Click;
            SwipeListener = new OnSwipeListener(this);
            SwipeListener.OnSwipeRight += SwipeListener_OnSwipeRight;
            SwipeListener.OnSwipeLeft += SwipeListener_OnSwipeLeft;
            itemDescription.SetOnTouchListener(SwipeListener);
            detailsView.SetOnTouchListener(SwipeListener);

            editNoteBtn.Click += delegate
            {
                var dialog = new AddNoteDialog(itemsList[Global.ItemScrollPosition].Note);
                dialog.Show(FragmentManager, "ADDNOTEFRAG");
                dialog.OnNoteSave += (Note note) => HandleNoteSave(note);
            };

            bookmarkedItems = JsonConvert.DeserializeObject<List<CategoryItem>>(DBAssist.DeserializeDB(path));
            bookmarkedItems = bookmarkedItems ?? new List<CategoryItem>();

            if (sendingActivity == "bmk")
                HandleBookmarkedItems();
            else
                HandleNonbookmarkedItems();
            notes = JsonConvert.DeserializeObject<List<Note>>(DBAssist.DeserializeDB(notesdb));
            notes = notes ?? new List<Note>();
            base.OnResume();
        }

        private void HandleNonbookmarkedItems() //Using the same activity to show Bookmarked items and non-bookmarked items. this handle non-bookmarked
        {
            item = Global.Categories[Global.CategoryScrollPosition].Items[Global.ItemScrollPosition];
            itemsList = Global.Categories[Global.CategoryScrollPosition].Items;
            SetupUI();
        }

        private void HandleBookmarkedItems() //handles ideas that were bookmarked
        {
            IsInBookmarkMode = true;
            Global.ItemScrollPosition = Global.BookmarkScrollPosition; // To enable swipes work correctly
			itemsList = Global.Categories[Global.CategoryScrollPosition].Items;
            item = bookmarkedItems[Global.BookmarkScrollPosition];
            SetupUI();
        }

        private void SetupUI()
        {
            title.Text = item.Title;
            itemDescription.Text = item.Description;
            db = Global.Categories;
            if (item.Note != null)
                ShowNote(item.Note);
            else
                noteHolder.Visibility = ViewStates.Gone;
        }

        private void ShowNote(Note note)
        {
            noteHolder.Visibility = ViewStates.Visible;
            noteContent.Text = item.Note.Content;
        }

        private void HandleNoteSave(Note note)
        {
            var existingItem = notes.FirstOrDefault(x => x.Title == note.Title);
            if (existingItem == null) //No existing note was found
            {
                notes.Add(note);
                itemsList[Global.ItemScrollPosition].Note = note;
            }
            else //Existing note was found
            {
                notes[notes.IndexOf(existingItem)] = note;
                itemsList[Global.ItemScrollPosition].Note = note;
            }

            ShowNote(note);
            Snackbar.Make(addNoteFab, "Note added.", Snackbar.LengthLong).Show();
        }

        private void AddNoteFab_Click(object sender, EventArgs e)
        {
            var dialog = new AddNoteDialog(itemsList[Global.ItemScrollPosition].Category, itemsList[Global.ItemScrollPosition].Title);
            dialog.OnError += () => { Snackbar.Make(addNoteFab, "Invalid note. Entry fields cannot be empty.", Snackbar.LengthLong).Show(); };
            dialog.OnNoteSave += (Note note) => HandleNoteSave(note);
            dialog.Show(FragmentManager, "ADDNOTEFRAG");
        }

        private void SwipeListener_OnSwipeRight()
        {
            ChangeItem(Global.ItemScrollPosition - 1, false);
        }

        private void SwipeListener_OnSwipeLeft()
        {
            ChangeItem(Global.ItemScrollPosition + 1, true);
        }

        private void ChangeItem(int index, bool WasLeftSwipe)
        {
            if (!IsInBookmarkMode)
            {
                if (index >= 0 && index <= itemsList.Count - 1)
                {
                    Global.ItemScrollPosition = index;
                    item = itemsList[index];
                    FinishItemChange(WasLeftSwipe);
                }
                else
					ToastDirection(WasLeftSwipe);
            }
            else
            {
				if (index >= 0 && index <= bookmarkedItems.Count - 1)
				{
					Global.BookmarkScrollPosition = index;
					Global.ItemScrollPosition = index;
					item = bookmarkedItems[index];
					FinishItemChange(WasLeftSwipe);
				}
				else
					ToastDirection(WasLeftSwipe);
            }
        }

		void ToastDirection(bool WasLeftSwipe)
		{
			var direction = !WasLeftSwipe ? "Start" : "End";
			Toast.MakeText(this, $"{direction} of list.", ToastLength.Long).Show();
		}

        private void FinishItemChange(bool WasLeftSwipe)
        {
            float direction = WasLeftSwipe ? 700 : -700;
            AnimHelper.Animate(detailsView, "translationX", 700, new AnticipateOvershootInterpolator(), direction, 0);
            title.Text = item.Title;
            itemDescription.Text = item.Description;
            if (item.Note == null)
                noteHolder.Visibility = ViewStates.Gone;
            else
                ShowNote(item.Note);
            CheckAndSetBookmark();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.itemdetails_menu, menu);
            bookmarkIcon = menu.FindItem(Resource.Id.bookmarkItem);
            CheckAndSetBookmark();
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
                    Bookmark();
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

        private void Bookmark()
        {
            IsBookmarked = CheckIfBookmarked(item);
            if (bookmarkedItems != null)
            {
				itemsList = Global.Categories.FirstOrDefault(x => x.CategoryLbl == item.Category).Items;
                if (IsBookmarked == true) // if bookmarked
                {
                    bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_border_white_24dp);
                    if (item != null)
                        bookmarkedItems.Remove(bookmarkedItems.FirstOrDefault(x => x.Title == item.Title));
					/*var beforeItem = itemsList.FirstOrDefault(x => x.Id == item.Id + 1);
					itemsList.Insert(itemsList.IndexOf(beforeItem), item);*/
                    Snackbar.Make(addNoteFab, "Idea removed from bookmarks.", Snackbar.LengthLong).Show();
                    IsBookmarked = false;
                }
                else // not bookmarked
                {
                    bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_white_24dp);
                    bookmarkedItems.Add(item);
					//itemsList.Remove(itemsList.FirstOrDefault(x => x.Id == item.Id));
                    Snackbar.Make(addNoteFab, "Idea added to bookmarks.", Snackbar.LengthLong).Show();
                    ChangeItem(Global.ItemScrollPosition + 1, true);
                    IsBookmarked = true;
                }
            }
        }

        private void CheckAndSetBookmark()
        {
            if (CheckIfBookmarked(item) == true)
            {
                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_white_24dp);
                IsBookmarked = true;
            }
            else
                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_border_white_24dp);
        }

        private bool CheckIfBookmarked(CategoryItem item)
        {
            if (bookmarkedItems.FirstOrDefault(x => x.Description == item.Description) != null)
                return true;
            else
                return false;
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
            NavigateUpTo(new Intent(this, sendingActivity == "idealistactivity" ? typeof(IdeaListActivity) : typeof(BookmarksActivity)));
            OverridePendingTransition(Resource.Animation.push_right_in, Resource.Animation.push_right_out);
        }
    }
}