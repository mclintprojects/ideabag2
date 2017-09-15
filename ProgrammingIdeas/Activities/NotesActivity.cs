using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ProgrammingIdeas.Adapters;
using ProgrammingIdeas.Fragments;
using ProgrammingIdeas.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace ProgrammingIdeas.Activities
{
    [Activity(Label = "Notes", Theme = "@style/AppTheme")]
    public class NotesActivity : BaseActivity
    {
        private List<Note> notes;
        private List<Idea> bookmarkedItems;
        private RecyclerView recycler;
        private RecyclerView.LayoutManager manager;
        private NotesAdapter adapter;
        private View emptyState;

        public override int LayoutResource => Resource.Layout.notesactivity;

        public override bool HomeAsUpEnabled => true;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            notes = await DBAssist.DeserializeDBAsync<List<Note>>(Global.NOTES_PATH);
            notes = notes ?? new List<Note>();

            bookmarkedItems = await DBAssist.DeserializeDBAsync<List<Idea>>(Global.BOOKMARKS_PATH);
            bookmarkedItems = bookmarkedItems ?? new List<Idea>();

            recycler = FindViewById<RecyclerView>(Resource.Id.notesRecyclerView);
            emptyState = FindViewById(Resource.Id.empty);

            if (notes.Count == 0)
                ShowEmptyState();
            else
            {
                manager = new LinearLayoutManager(this);
                adapter = new NotesAdapter(notes);
                adapter.EditClicked += Adapter_EditClicked;
                adapter.ViewNoteClicked += (position) =>
                {
                    new AlertDialog.Builder(this)
                                    .SetTitle(notes[position].Name)
                                    .SetMessage(notes[position].Content)
                                    .SetPositiveButton("Great", (sender, e) => { return; })
                                    .Create().Show();
                };

                adapter.DeleteClicked += (position) =>
                {
                    var foundIdea = Global.Categories.FirstOrDefault(x => x.CategoryLbl == notes[position].Category).Items.FirstOrDefault(y => y.Title == notes[position].Title);
                    if (foundIdea != null)
                        foundIdea.Note = null;

                    var foundBookmark = bookmarkedItems.FirstOrDefault(x => x.Title == notes[position].Title);
                    if (foundBookmark != null)
                        foundBookmark.Note = null;

                    notes.RemoveAt(position);
                    adapter.NotifyItemRemoved(position);
                    if (notes.Count == 0)
                        ShowEmptyState();
                };

                recycler.SetLayoutManager(manager);
                recycler.SetAdapter(adapter);
                recycler.SetItemAnimator(new DefaultItemAnimator());
            }
        }

        private void ShowEmptyState()
        {
            recycler.Visibility = ViewStates.Gone;
            emptyState.Visibility = ViewStates.Visible;
            emptyState.FindViewById<TextView>(Resource.Id.infoText).Text = "You have no notes yet.";
        }

        private void Adapter_EditClicked(int position)
        {
            var view = recycler.GetChildAt(position);
            var vieNote = view.FindViewById<TextView>(Resource.Id.viewNote);
            var editNote = view.FindViewById<TextView>(Resource.Id.noteEdit);
            var content = view.FindViewById<TextView>(Resource.Id.notesContent);

            var dialog = new AddNoteDialog(notes[position]);
            dialog.OnNoteSave += (Note note) =>
            {
                notes[position] = note;
                var foundIdea = Global.Categories.FirstOrDefault(x => x.CategoryLbl == note.Category).Items.FirstOrDefault(y => y.Title == note.Title);
                if (foundIdea != null)
                    foundIdea.Note = note;

                var foundBookmark = bookmarkedItems.FirstOrDefault(x => x.Title == notes[position].Title);
                if (foundBookmark != null)
                    foundBookmark.Note = note;
                adapter.NotifyItemChanged(position);
            };

            dialog.OnError += () => Snackbar.Make(recycler, "Invalid note. Entry fields cannot be empty.", Snackbar.LengthLong).Show();

            dialog.Show(FragmentManager, "NOTESFRAG");
        }

        protected override void OnPause()
        {
            base.OnPause();
            DBAssist.SerializeDBAsync(Global.NOTES_PATH, notes);
            DBAssist.SerializeDBAsync(Global.BOOKMARKS_PATH, bookmarkedItems);
        }
    }
}