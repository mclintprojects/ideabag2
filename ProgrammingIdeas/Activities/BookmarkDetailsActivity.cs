using Android.App;
using Android.Content;
using Android.Content.PM;
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
    [Activity(Label = "Bookmark details", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class BookmarkDetailsActivity : BaseActivity
    {
        private List<Note> notes;
        private List<Idea> ideasList, bookmarkedItems = new List<Idea>();
        private Idea bookmarkedIdea;
        private IMenuItem bookmarkIcon;
        private TextView ideaTitleLbl, ideaDescriptionLbl, noteContentLbl;
        private LinearLayout detailsView;
        private FloatingActionButton addNoteFab;
        private CardView noteCard;
        private bool isIdeaBookmarked;

        public override int LayoutResource => Resource.Layout.ideadetailsactivity;

        public override bool HomeAsUpEnabled => true;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ideaTitleLbl = FindViewById<TextView>(Resource.Id.itemTitle);
            ideaDescriptionLbl = FindViewById<TextView>(Resource.Id.itemDescription);
            detailsView = FindViewById<LinearLayout>(Resource.Id.detailsView);
            addNoteFab = FindViewById<FloatingActionButton>(Resource.Id.addNotefab);
            var editNoteBtn = FindViewById<Button>(Resource.Id.editNoteBtn);
            noteCard = FindViewById<CardView>(Resource.Id.noteHolder);
            noteContentLbl = FindViewById<TextView>(Resource.Id.noteContent);

            addNoteFab.Click += AddNoteFab_Click;
            var swipeListener = new OnSwipeListener(this);
            swipeListener.OnSwipeRight += SwipeListener_OnSwipeRight;
            swipeListener.OnSwipeLeft += SwipeListener_OnSwipeLeft;
            ideaDescriptionLbl.SetOnTouchListener(swipeListener);
            detailsView.SetOnTouchListener(swipeListener);

            editNoteBtn.Click += delegate
            {
                var dialog = new AddNoteDialog(bookmarkedItems[Global.IdeaScrollPosition].Note);
                dialog.OnError += () => Snackbar.Make(addNoteFab, "Invalid note. Entry fields cannot be empty.", Snackbar.LengthLong).Show();
                dialog.Show(FragmentManager, "ADDNOTEFRAG");
                dialog.OnNoteSave += (Note note) => SaveNote(note);
            };

            bookmarkedItems = await DBSerializer.DeserializeDBAsync<List<Idea>>(Global.BOOKMARKS_PATH);
            bookmarkedItems = bookmarkedItems ?? new List<Idea>();

            bookmarkedIdea = bookmarkedItems[Global.BookmarkScrollPosition];
            ideasList = Global.Categories.FirstOrDefault(x => x.CategoryLbl == bookmarkedIdea.Category).Items;

            notes = await DBSerializer.DeserializeDBAsync<List<Note>>(Global.NOTES_PATH);
            notes = notes ?? new List<Note>();

            SetupUI();
        }

        private void SetupUI()
        {
            ideaTitleLbl.Text = bookmarkedIdea.Title;
            ideaDescriptionLbl.Text = bookmarkedIdea.Description;
            if (bookmarkedIdea.Note != null)
                ShowNote();
            else
                noteCard.Visibility = ViewStates.Gone;
        }

        private void ShowNote()
        {
            noteCard.Visibility = ViewStates.Visible;
            noteContentLbl.Text = bookmarkedIdea.Note.Content;
        }

        private void SaveNote(Note note)
        {
            bookmarkedIdea.Note = note;
            var existingItem = notes.FirstOrDefault(x => x.Title == note.Title);
            if (existingItem == null) // No existing note was found
            {
                notes.Add(note);
                ideasList.FirstOrDefault(x => x.Title == bookmarkedIdea.Title).Note = note;
            }
            else // An existing note was found
            {
                notes[notes.IndexOf(existingItem)] = note;
                ideasList.FirstOrDefault(x => x.Title == bookmarkedIdea.Title).Note = note;
            }

            ShowNote();
            Snackbar.Make(addNoteFab, "Note added.", Snackbar.LengthLong).Show();
        }

        private void AddNoteFab_Click(object sender, EventArgs e)
        {
            if (bookmarkedIdea.Note == null)
            {
                var dialog = new AddNoteDialog(ideasList[Global.IdeaScrollPosition].Category, ideasList[Global.IdeaScrollPosition].Title);
                dialog.OnError += () => { Snackbar.Make(addNoteFab, "Invalid note. Entry fields cannot be empty.", Snackbar.LengthLong).Show(); };
                dialog.OnNoteSave += (Note note) => SaveNote(note);
                dialog.Show(FragmentManager, "ADDNOTEFRAG");
            }
            else
                Snackbar.Make(addNoteFab, "This idea already has a note. Consider editing that instead.", Snackbar.LengthLong).Show();
        }

        private void SwipeListener_OnSwipeRight()
        {
            ChangeItem(--Global.BookmarkScrollPosition, false);
        }

        private void SwipeListener_OnSwipeLeft()
        {
            ChangeItem(++Global.BookmarkScrollPosition, true);
        }

        private void ChangeItem(int index, bool wasLeftSwipe)
        {
            if (index >= 0 && index <= bookmarkedItems.Count - 1)
            {
                Global.BookmarkScrollPosition = index;
                bookmarkedIdea = bookmarkedItems[index];
                FinishIdeaSwipe(wasLeftSwipe);
            }
            else
                ToastDirection(wasLeftSwipe);
        }

        /// <summary>
        /// Show a toast if we've reached the start or end of a category's ideas
        /// </summary>
        /// <param name="wasLeftSwipe">True if the user just swiped left</param>
        private void ToastDirection(bool wasLeftSwipe)
        {
            var direction = !wasLeftSwipe ? "Start" : "End";
            Toast.MakeText(this, $"{direction} of list.", ToastLength.Long).Show();
            Global.BookmarkScrollPosition = !wasLeftSwipe ? 0 : bookmarkedItems.Count - 1;
        }

        /// <summary>
        /// Animates idea swipe and if there's a note shows it.
        /// </summary>
        /// <param name="wasLeftSwipe"></param>
        private void FinishIdeaSwipe(bool wasLeftSwipe)
        {
            float direction = wasLeftSwipe ? 700 : -700;
            AnimHelper.Animate(detailsView, "translationX", 700, new AnticipateOvershootInterpolator(), direction, 0);
            ideaTitleLbl.Text = bookmarkedIdea.Title;
            ideaDescriptionLbl.Text = bookmarkedIdea.Description;
            if (bookmarkedIdea.Note == null)
                noteCard.Visibility = ViewStates.Gone;
            else
                ShowNote();
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
                        $"Title: {bookmarkedIdea.Title}\r\nDifficulty: {bookmarkedIdea.Difficulty}\r\n\r\n{this.bookmarkedIdea.Description}\r\n\r\n" +
                        $"Want more coding ideas? Get the app here: https://play.google.com/store/apps/details?id=com.alansa.ideabag2";
                    share.PutExtra(Intent.ExtraText, textToShare);
                    StartActivity(Intent.CreateChooser(share, "Share idea via"));
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SaveChanges()
        {
            DBSerializer.SerializeDBAsync(Global.BOOKMARKS_PATH, bookmarkedItems);
            DBSerializer.SerializeDBAsync(Global.IDEAS_PATH, Global.Categories);
            DBSerializer.SerializeDBAsync(Global.NOTES_PATH, notes);
        }

        private void BookmarkIdea()
        {
            isIdeaBookmarked = IsIdeaBookmarked(bookmarkedIdea);
            if (bookmarkedItems != null)
            {
                ideasList = Global.Categories.FirstOrDefault(x => x.CategoryLbl == bookmarkedIdea.Category).Items;
                if (isIdeaBookmarked) // if currently open idea is bookmarked
                {
                    bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_border_white_24dp);
                    bookmarkedItems.Remove(bookmarkedItems.FirstOrDefault(x => x.Title == bookmarkedIdea.Title));

                    if (bookmarkedItems.Count == 0)
                        base.NavigateAway();
                    else
                    {
                        Snackbar.Make(addNoteFab, "Idea removed from bookmarks.", Snackbar.LengthLong).Show();
                        isIdeaBookmarked = false;
                        Global.BookmarkScrollPosition -= 1;
                        ChangeItem(++Global.BookmarkScrollPosition, false);
                    }
                }
                else // Currently open idea is not bookmarked
                {
                    bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_white_24dp);
                    bookmarkedItems.Add(bookmarkedIdea);
                    Snackbar.Make(addNoteFab, "Idea added to bookmarks.", Snackbar.LengthLong).Show();
                    ChangeItem(Global.BookmarkScrollPosition + 1, true);
                    isIdeaBookmarked = true;
                }
            }
        }

        /// <summary>
        /// Checks if the currently open idea has been bookmarked and sets the bookmark icon
        /// accordingly
        /// </summary>
        private void CheckAndSetBookmark()
        {
            if (IsIdeaBookmarked(bookmarkedIdea))
            {
                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_white_24dp);
                isIdeaBookmarked = true;
            }
            else
                bookmarkIcon.SetIcon(Resource.Mipmap.ic_bookmark_border_white_24dp);
        }

        /// <summary>
        /// An idea is bookmarked if it is found in the bookmarked ideas list
        /// </summary>
        /// <param name="idea">The idea to check if it is bookmarked</param>
        /// <returns></returns>
        private bool IsIdeaBookmarked(Idea idea)
        {
            if (bookmarkedItems.FirstOrDefault(x => x.Title == idea.Title) != null)
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