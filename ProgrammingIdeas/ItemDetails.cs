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
         List<Category> db;
         List<Note> notes;
         List<CategoryItem> itemsList;
         List<CategoryItem> bookmarkedItems = new List<CategoryItem>();
         CategoryItem item;
         string itemTitle, noteText, path, sender, ideasdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ideasdb");
         string notesdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "notesdb");
         IMenuItem bookmarkIcon;
         bool isBookmarked = true, isDBookmarked, isNoteEditing, wasFirstViewClickedMain, wasFirstViewClickedItem; //bookmarks bool, details bool
         int itemscrollPosition, mainscrollPos;
         CardView noteCard;
         ViewSwitcher notesSwitcher;
         TextView notesLbl, notesTitle, editNote;
         EditText notesInput;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "bookmarks.json");
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ideadetailsactivity);
            noteCard = FindViewById<CardView>(Resource.Id.notesCard);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            item = JsonConvert.DeserializeObject<CategoryItem>(Intent.GetStringExtra("item"));
            itemsList = JsonConvert.DeserializeObject<List<CategoryItem>>(Intent.GetStringExtra("itemsListJson"));
            itemTitle = Intent.GetStringExtra("title");
            sender = Intent.GetStringExtra("sender");
            itemscrollPosition = Intent.GetIntExtra("itemscrollPos", 0);
			mainscrollPos = Intent.GetIntExtra("mainscrollPos", 0);
			wasFirstViewClickedMain = Intent.GetBooleanExtra("isHomeFirst", false);
			wasFirstViewClickedItem = Intent.GetBooleanExtra("isItemFirst", false);
            var title = FindViewById<TextView>(Resource.Id.itemTitle);
            var description = FindViewById<TextView>(Resource.Id.itemDescription);
            notesSwitcher = FindViewById<ViewSwitcher>(Resource.Id.notesSwitcher);
            notesLbl = FindViewById<TextView>(Resource.Id.notesLbl);
            notesTitle = FindViewById<TextView>(Resource.Id.notesTitle);
            notesInput = FindViewById<EditText>(Resource.Id.notesInput);
            editNote = FindViewById<TextView>(Resource.Id.editNote);
            title.Text = item.Title;
            description.Text = item.Description;
            db = (List<Category>)DBAssist.DeserializeDB(ideasdb, db);
            notes = (List<Note>)DBAssist.DeserializeDB(notesdb, notes);
            if (notes != null)
            {
                var foundItem = notes.FirstOrDefault(x => x.Title == item.Title);
                if (foundItem != null)
                    notesLbl.Text = foundItem.Content;
                foundItem = null;
            }
            Task.Run(() => { bookmarkedItems = (List<CategoryItem>)DBAssist.DeserializeDB(path, bookmarkedItems); });
            editNote.Click += (sender, e) =>
            {
                if (isNoteEditing == true) //user wants to save note
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
                    else //existing note was found
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
                { //user wants to edit note
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.itemdetails_menu, menu);
            bookmarkIcon = menu.FindItem(Resource.Id.bookmarkItem);
            if (sender == "bmk")
                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_white_24dp);
            else
                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_border_white_24dp);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    writeEntirety();
                    Intent intent = new Intent(this, sender == "deets" ? typeof(ItemActivity) : typeof(Bookmarks));
                    intent.PutExtra("jsonString", JsonConvert.SerializeObject(itemsList)); //itemlist json for itemactivity header
                    intent.PutExtra("title", itemTitle); //title for itemactivity header
                    intent.PutExtra("itemscrollPos", itemscrollPosition);
					intent.PutExtra("mainscrollPos", mainscrollPos);
					intent.PutExtra("isHomeFirst", wasFirstViewClickedMain);
					intent.PutExtra("isItemFirst", wasFirstViewClickedItem);
                    NavigateUpTo(intent);
                    OverridePendingTransition(Resource.Animation.push_right_in, Resource.Animation.push_right_out);
                    return true;

                case Resource.Id.bookmarkItem:
                    if (bookmarkedItems != null || bookmarkedItems.Count != 0)
                    {
                        if (sender == "bmk")
                        {
                            var sentItem = bookmarkedItems.FirstOrDefault(x => x.Description == this.item.Description);
                            if (isBookmarked == true) //if bookmarked
                            {
                                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_border_white_24dp);
                                if (sentItem != null)
                                    bookmarkedItems.Remove(sentItem);
                                Toast.MakeText(this, "Idea removed from bookmarks", ToastLength.Short).Show();
                                isBookmarked = false;
                            }
                            else //removed from bookmarks and clicked again
                            {
                                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_white_24dp);
                                if (!bookmarkedItems.Contains(this.item))
                                    bookmarkedItems.Add(this.item);
                                Toast.MakeText(this, "Idea added to bookmarks", ToastLength.Short).Show();
                                isBookmarked = true;
                            }
                        }
                        else
                        {
                            if (isDBookmarked == false)//not bookmarked
                            {
                                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_white_24dp);
                                if (!bookmarkedItems.Contains(this.item) && this.item != null)
                                    bookmarkedItems.Add(this.item);
                                Toast.MakeText(this, "Idea bookmarked", ToastLength.Short).Show();
                                isDBookmarked = true;
                            }
                            else
                            {
                                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_border_white_24dp);
                                if (bookmarkedItems.Contains(this.item))
                                    bookmarkedItems.Remove(this.item);
                                Toast.MakeText(this, "Idea removed from bookmarks", ToastLength.Short).Show();
                                isDBookmarked = false;
                            }
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

        void writeEntirety()
        {
            DBAssist.SerializeDB(path, bookmarkedItems);
            DBAssist.SerializeDB(ideasdb, db);
            DBAssist.SerializeDB(notesdb, notes);
        }

        protected override void OnStop()
        {
            writeEntirety();
            base.OnStop();
        }

        public override void OnBackPressed()
        {
            writeEntirety();
            Intent intent = new Intent(this, sender == "idealistactivity" ? typeof(ItemActivity) : typeof(Bookmarks));
            intent.PutExtra("jsonString", JsonConvert.SerializeObject(itemsList)); //itemlist json for itemactivity header
            intent.PutExtra("title", itemTitle); //title for itemactivity header
            intent.PutExtra("itemscrollPos", itemscrollPosition);
			intent.PutExtra("mainscrollPos", mainscrollPos);
			intent.PutExtra("wasFirstViewClickedMain", wasFirstViewClickedMain);
			intent.PutExtra("wasFirstViewClickedItem", wasFirstViewClickedItem);
            NavigateUpTo(intent);
            OverridePendingTransition(Resource.Animation.push_right_in, Resource.Animation.push_right_out);
            base.OnBackPressed();
        }
    }
}