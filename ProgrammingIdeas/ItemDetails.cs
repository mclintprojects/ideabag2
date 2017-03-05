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

namespace ProgrammingIdeas
{
    [Activity(Label = "Idea Details")]
    public class ItemDetails : Activity
    {
        private List<Category> db;
        private List<Note> notes;
        private List<CategoryItem> itemsList;
        private List<CategoryItem> bookmarkedItems = new List<CategoryItem>();
        private CategoryItem item;
        private string itemTitle, noteText, path, sendingActivity, ideasdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ideasdb");
        private string notesdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "notesdb");
        private IMenuItem bookmarkIcon;
        private bool isBookmarked = false, isNoteEditing, wasFirstViewClickedMain, wasFirstViewClickedItem; //bookmarks bool, details bool
        private int itemscrollPosition, mainscrollPos;
        private CardView noteCard, ideaCard;
        private ViewSwitcher notesSwitcher;
        private TextView notesLbl, notesTitle, editNote, title, itemDescription;
        private EditText notesInput;
        private OnSwipeListener SwipeListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "bookmarks.json");
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ideadetailsactivity);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            itemTitle = Intent.GetStringExtra("title");
            sendingActivity = Intent.GetStringExtra("sender");
            itemscrollPosition = Intent.GetIntExtra("itemscrollPos", 0);
            mainscrollPos = Intent.GetIntExtra("mainscrollPos", 0);
            wasFirstViewClickedMain = Intent.GetBooleanExtra("isHomeFirst", false);
            wasFirstViewClickedItem = Intent.GetBooleanExtra("isItemFirst", false);

            noteCard = FindViewById<CardView>(Resource.Id.notesCard);
            item = JsonConvert.DeserializeObject<CategoryItem>(Intent.GetStringExtra("item"));
            itemsList = JsonConvert.DeserializeObject<List<CategoryItem>>(Intent.GetStringExtra("itemsListJson"));
            title = FindViewById<TextView>(Resource.Id.itemTitle);
            itemDescription = FindViewById<TextView>(Resource.Id.itemDescription);
            notesSwitcher = FindViewById<ViewSwitcher>(Resource.Id.notesSwitcher);
            notesLbl = FindViewById<TextView>(Resource.Id.notesLbl);
            notesTitle = FindViewById<TextView>(Resource.Id.notesTitle);
            notesInput = FindViewById<EditText>(Resource.Id.notesInput);
            editNote = FindViewById<TextView>(Resource.Id.editNote);
            ideaCard = FindViewById<CardView>(Resource.Id.ideaCard);

            SwipeListener = new OnSwipeListener(this);
            ideaCard.SetOnTouchListener(SwipeListener);
            SwipeListener.OnSwipeRight += SwipeListener_OnSwipeRight;
            SwipeListener.OnSwipeLeft += SwipeListener_OnSwipeLeft;
            itemDescription.SetOnTouchListener(SwipeListener);

            title.Text = item.Title;
            itemDescription.Text = item.Description;
            db = (List<Category>)DBAssist.GetDB(Assets, db);
			notes = JsonConvert.DeserializeObject<List<Note>>(DBAssist.DeserializeDB(notesdb));

            FindNotes();

            using (BusyHandler.Handle(RemoveBookmarkedItems))
                Task.Run(() => { 
				bookmarkedItems = JsonConvert.DeserializeObject<List<CategoryItem>>(DBAssist.DeserializeDB(path));
					if (bookmarkedItems == null)
						bookmarkedItems = new List<CategoryItem>();
			});

            editNote.Click += (sender, e) =>
            {
                // User wants to save note.
                if (isNoteEditing == true)
                {
                    noteText = notesInput.Text;
                    notesSwitcher.ShowPrevious();
                    notesLbl.Text = noteText;
                    isNoteEditing = false;
                    editNote.Text = "Edit this note";
                    var newNote = new Note() { Category = item.Category, Content = noteText.Length == 0 ? null : noteText, Title = item.Title };
                    itemsList.FirstOrDefault(x => x.Description == item.Description).Note = newNote;
                    db.FirstOrDefault(x => x.CategoryLbl == itemTitle).Items.FirstOrDefault(y => y.Description == item.Description).Note = newNote;
                    if (notes == null)
                        notes = new List<Note>();
                    var foundNote = notes.FirstOrDefault(x => x.Title == newNote.Title);
                    if (foundNote == null) // existing note wasn't found
                    {
                        if (newNote.Content != null)
                            notes.Add(newNote);
                        else
                            notesLbl.Text = "You have no notes for this idea. Tap the button below to add one.";
                    }

                    // Existing note was found.
                    else
                    {
                        if (newNote.Content == null)
                        {
                            notesLbl.Text = "You have no notes for this idea. Tap the button below to add one.";
                            notes.Remove(foundNote);
                            db.FirstOrDefault(x => x.CategoryLbl == itemTitle).Items.FirstOrDefault(y => y.Description == item.Description).Note = null;
                        }
                        else
                        {
                            notes.Remove(foundNote);
                            notes.Add(newNote);
                        }
                    }
                }
                else
                {
                    // User wants to edit note.
                    if (notesLbl.Text.Contains("You have no notes for this idea"))
                        notesInput.Text = "";
                    else
                        notesInput.Text = notesLbl.Text;
                    notesInput.RequestFocus();
                    isNoteEditing = true;
                    notesSwitcher.ShowNext();
                    editNote.Text = "Save this note";
                }
            };
        }

        private void SwipeListener_OnSwipeRight(object sender, System.EventArgs e)
        {
            ChangeItem(itemscrollPosition - 1);
        }

        private void SwipeListener_OnSwipeLeft(object sender, System.EventArgs e)
        {
            ChangeItem(itemscrollPosition + 1);
        }

        private void ChangeItem(int index)
        {
            if (index >= 0 && index <= itemsList.Count - 1)
            {
                itemscrollPosition = index;
                item = itemsList[index];
                CheckAndSetBookmark();
				isBookmarked = CheckIfBookmarked(item);
                title.Text = item.Title;
                itemDescription.Text = item.Description;
                FindNotes();
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

        private void FindNotes()
        {
            if (notes != null)
            {
                var foundItem = notes.FirstOrDefault(x => x.Title == item.Title);
				if (foundItem != null)
					notesLbl.Text = foundItem.Content;
				else
					notesLbl.Text = Resources.GetString(Resource.String.notes_sample_content);
                foundItem = null;
            }
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
                    if (bookmarkedItems != null || bookmarkedItems.Count != 0)
                    {
                        if (isBookmarked == true) // if bookmarked
                        {
                            bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_border_white_24dp);
                            if (this.item != null)
								bookmarkedItems.Remove(bookmarkedItems.FirstOrDefault(x => x.Description == this.item.Description));
                            Toast.MakeText(this, "Idea removed from bookmarks", ToastLength.Short).Show();
                            isBookmarked = false;
                        }
                        else // not bookmarked
                        {
							if (sendingActivity == "details")
                            	itemsList.RemoveAt(itemscrollPosition);
                            bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_white_24dp);
							if (bookmarkedItems.FirstOrDefault(x => x.Description == this.item.Description) == null)
								bookmarkedItems.Add(this.item);
                            Toast.MakeText(this, "Idea added to bookmarks", ToastLength.Short).Show();
                            isBookmarked = true;
                        }
                    }
                    writeEntirety();
                    return true;

                case Resource.Id.shareItem:
                    Intent share = new Intent();
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

        private void CheckAndSetBookmark()
        {
            if (CheckIfBookmarked(item) == true)
            {
                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_white_24dp);
                isBookmarked = true;
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

        private void writeEntirety()
        {
            Task.Run(() =>
            {
                DBAssist.SerializeDB(path, bookmarkedItems);
                DBAssist.SerializeDB(ideasdb, db);
                DBAssist.SerializeDB(notesdb, notes);
            });
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

		void GoAway()
		{
			writeEntirety();
			Intent intent = new Intent(this, sendingActivity == "idealistactivity" ? typeof(ItemActivity) : typeof(Bookmarks));
			intent.PutExtra("jsonString", JsonConvert.SerializeObject(itemsList)); //itemlist json for itemactivity header
			intent.PutExtra("title", itemTitle); //title for itemactivity header
			intent.PutExtra("itemscrollPos", itemscrollPosition);
			intent.PutExtra("mainscrollPos", mainscrollPos);
			intent.PutExtra("wasFirstViewClickedMain", wasFirstViewClickedMain);
			intent.PutExtra("wasFirstViewClickedItem", wasFirstViewClickedItem);
			NavigateUpTo(intent);
			OverridePendingTransition(Resource.Animation.push_right_in, Resource.Animation.push_right_out);
		}
    }
}