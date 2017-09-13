using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using ProgrammingIdeas.Animation;
using ProgrammingIdeas.Fragments;
using ProgrammingIdeas.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgrammingIdeas.Activities
{
    [Activity(Label = "Idea details", Theme = "@style/AppTheme")]
    public class IdeaDetailsActivity : BaseActivity
    {
        private List<Note> notes;
        private List<Idea> ideasList;
        private List<Idea> bookmarkedItems = new List<Idea>();
        private Idea idea;
        private IMenuItem bookmarkIcon;
        private TextView ideaTitleLbl, ideaDescriptionLbl, noteContentLbl;
        private OnSwipeListener SwipeListener;
        private LinearLayout detailsView;
        private FloatingActionButton addNoteFab;
        private Button editNoteBtn;
        private CardView noteHolder;
        private bool isBookmarked;

        public override int LayoutResource => Resource.Layout.ideadetailsactivity;

        public override bool HomeAsUpEnabled => true;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ideaTitleLbl = FindViewById<TextView>(Resource.Id.itemTitle);
            ideaDescriptionLbl = FindViewById<TextView>(Resource.Id.itemDescription);
            detailsView = FindViewById<LinearLayout>(Resource.Id.detailsView);
            addNoteFab = FindViewById<FloatingActionButton>(Resource.Id.addNotefab);
            editNoteBtn = FindViewById<Button>(Resource.Id.editNoteBtn);
            noteHolder = FindViewById<CardView>(Resource.Id.noteHolder);
            noteContentLbl = FindViewById<TextView>(Resource.Id.noteContent);

            addNoteFab.Click += AddNoteFab_Click;
            SwipeListener = new OnSwipeListener(this);
            SwipeListener.OnSwipeRight += SwipeListener_OnSwipeRight;
            SwipeListener.OnSwipeLeft += SwipeListener_OnSwipeLeft;
            ideaDescriptionLbl.SetOnTouchListener(SwipeListener);
            detailsView.SetOnTouchListener(SwipeListener);

            editNoteBtn.Click += delegate
            {
                var dialog = new AddNoteDialog(ideasList[Global.IdeaScrollPosition].Note);
                dialog.Show(FragmentManager, "ADDNOTEFRAG");
                dialog.OnError += () => Snackbar.Make(addNoteFab, "Invalid note. Entry fields cannot be empty.", Snackbar.LengthLong).Show();
                dialog.OnNoteSave += (Note note) => SaveNote(note);
            };

            bookmarkedItems = await DBAssist.DeserializeDBAsync<List<Idea>>(Global.BOOKMARKS_PATH);
            bookmarkedItems = bookmarkedItems ?? new List<Idea>();
            idea = Global.Categories[Global.CategoryScrollPosition].Items[Global.IdeaScrollPosition];
            ideasList = Global.Categories[Global.CategoryScrollPosition].Items;
            notes = await DBAssist.DeserializeDBAsync<List<Note>>(Global.NOTES_PATH);
            notes = notes ?? new List<Note>();

            SetupUI();
        }

        private void SetupUI()
        {
            ideaTitleLbl.Text = idea.Title;
            ideaDescriptionLbl.Text = idea.Description;
            if (idea.Note != null)
                ShowNote(idea.Note);
            else
                noteHolder.Visibility = ViewStates.Gone;
        }

        private void ShowNote(Note note)
        {
            noteHolder.Visibility = ViewStates.Visible;
            noteContentLbl.Text = idea.Note.Content;
        }

        private void SaveNote(Note note)
        {
            idea.Note = note;
            var existingItem = notes.FirstOrDefault(x => x.Title == note.Title);
            if (existingItem == null) //No existing note was found
                notes.Add(note);
            else //Existing note was found
            {
                notes[notes.IndexOf(existingItem)] = note;

                var existingBookmark = bookmarkedItems.FirstOrDefault(x => x.Title == note.Title);
                if (existingBookmark != null)
                    existingBookmark.Note = note;
            }

            ShowNote(note);
            Snackbar.Make(addNoteFab, "Note added.", Snackbar.LengthLong).Show();
        }

        private void AddNoteFab_Click(object sender, EventArgs e)
        {
            if (idea.Note == null)
            {
                var dialog = new AddNoteDialog(ideasList[Global.IdeaScrollPosition].Category, ideasList[Global.IdeaScrollPosition].Title);
                dialog.OnError += () =>
                {
                    Snackbar.Make(addNoteFab, "Invalid note. Entry fields cannot be empty.", Snackbar.LengthLong).Show();
                };
                dialog.OnNoteSave += (Note note) => SaveNote(note);
                dialog.Show(FragmentManager, "ADDNOTEFRAG");
            }
            else
                Snackbar.Make(addNoteFab, "This idea already has a note. Consider editing that instead.", Snackbar.LengthLong).Show();
        }

        private void SwipeListener_OnSwipeRight()
        {
            ChangeItem(--Global.IdeaScrollPosition, false);
        }

        private void SwipeListener_OnSwipeLeft()
        {
            ChangeItem(++Global.IdeaScrollPosition, true);
        }

        private void ChangeItem(int index, bool WasLeftSwipe)
        {
            if (index >= 0 && index <= ideasList.Count - 1)
            {
                idea = ideasList[index];
                FinishItemChange(WasLeftSwipe);
            }
            else
                ToastDirection(WasLeftSwipe);
        }

        private void ToastDirection(bool WasLeftSwipe)
        {
            var direction = !WasLeftSwipe ? "Start" : "End";
            Toast.MakeText(this, $"{direction} of list.", ToastLength.Long).Show();
            Global.IdeaScrollPosition = !WasLeftSwipe ? 0 : ideasList.Count - 1;
        }

        private void FinishItemChange(bool WasLeftSwipe)
        {
            float direction = WasLeftSwipe ? 700 : -700;
            AnimHelper.Animate(detailsView, "translationX", 700, new AnticipateOvershootInterpolator(), direction, 0);
            ideaTitleLbl.Text = idea.Title;
            ideaDescriptionLbl.Text = idea.Description;
            if (idea.Note == null)
                noteHolder.Visibility = ViewStates.Gone;
            else
                ShowNote(idea.Note);
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
                case Resource.Id.bookmarkItem:
                    BookmarkIdea();
                    return true;

                case Resource.Id.shareItem:
                    var share = new Intent();
                    share.SetAction(Intent.ActionSend);
                    share.SetType("text/plain");
                    string textToShare = $"Can you code this challenge?\r\n\r\n" +
                        $"Title: {this.idea.Title}\r\nDifficulty: {this.idea.Difficulty}\r\n\r\n{this.idea.Description}\r\n\r\n" +
                        $"Want more coding ideas? Get the app here: https://play.google.com/store/apps/details?id=com.alansa.ideabag2";
                    share.PutExtra(Intent.ExtraText, textToShare);
                    StartActivity(Intent.CreateChooser(share, "Share idea via"));
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SaveChanges()
        {
            DBAssist.SerializeDBAsync(Global.BOOKMARKS_PATH, bookmarkedItems);
            DBAssist.SerializeDBAsync(Global.IDEAS_PATH, Global.Categories);
            DBAssist.SerializeDBAsync(Global.NOTES_PATH, notes);
        }

        private void BookmarkIdea()
        {
            Global.RefreshBookmarks = true;
            isBookmarked = CheckIfBookmarked(idea);
            if (bookmarkedItems != null)
            {
                if (isBookmarked) // if bookmarked
                {
                    bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_border_white_24dp);
                    if (idea != null)
                        bookmarkedItems.Remove(bookmarkedItems.FirstOrDefault(x => x.Title == idea.Title));

                    Snackbar.Make(addNoteFab, "Idea removed from bookmarks.", Snackbar.LengthLong).Show();
                    isBookmarked = false;
                    ChangeItem(++Global.IdeaScrollPosition, false);
                }
                else // not bookmarked
                {
                    bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_white_24dp);
                    bookmarkedItems.Add(idea);
                    Snackbar.Make(addNoteFab, "Idea added to bookmarks.", Snackbar.LengthLong).Show();
                    ChangeItem(++Global.IdeaScrollPosition, true);
                    isBookmarked = true;
                }
            }
        }

        private void CheckAndSetBookmark()
        {
            if (CheckIfBookmarked(idea) == true)
            {
                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_white_24dp);
                isBookmarked = true;
            }
            else
                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_border_white_24dp);
        }

        private bool CheckIfBookmarked(Idea item)
        {
            if (bookmarkedItems.FirstOrDefault(x => x.Title == item.Title) != null)
                return true;
            else
                return false;
        }

        protected override void OnPause()
        {
            SaveChanges();
            base.OnPause();
        }
    }
}