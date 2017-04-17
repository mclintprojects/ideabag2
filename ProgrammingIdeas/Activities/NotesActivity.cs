using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using ProgrammingIdeas.Adapters;
using ProgrammingIdeas.Fragments;
using ProgrammingIdeas.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProgrammingIdeas.Activities
{
    [Activity(Label = "Notes", Theme = "@style/AppTheme")]
    public class NotesActivity : BaseActivity
    {
        private List<Note> notes;
        private RecyclerView recycler;
        private RecyclerView.LayoutManager manager;
        private NotesAdapter adapter;
        private readonly string notesdb = Path.Combine(Global.APP_PATH, "notesdb");
        private View emptyState;

        public override int LayoutResource
        {
            get
            {
                return Resource.Layout.notesactivity;
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
        }

        protected override void OnResume()
        {
            notes = JsonConvert.DeserializeObject<List<Note>>(DBAssist.DeserializeDB(notesdb));
            notes = notes ?? new List<Note>();
            recycler = FindViewById<RecyclerView>(Resource.Id.notesRecyclerView);
            emptyState = FindViewById(Resource.Id.empty);
            if (notes.Count == 0)
                ShowEmptyState();
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
                Global.Categories.FirstOrDefault(x => x.CategoryLbl == notes[position].Category).Items.FirstOrDefault(y => y.Title == notes[position].Title).Note = null;
                notes.RemoveAt(position);
                adapter.NotifyItemRemoved(position);
                if (notes.Count == 0)
                    ShowEmptyState();
            };
            recycler.SetAdapter(adapter);
            recycler.SetLayoutManager(manager);
            recycler.SetItemAnimator(new DefaultItemAnimator());
            base.OnResume();
        }

        private void ShowEmptyState()
        {
            recycler.Visibility = ViewStates.Gone;
            emptyState.Visibility = ViewStates.Visible;
            emptyState.FindViewById<TextView>(Resource.Id.infoText).Text += " notes.";
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
                Global.Categories.FirstOrDefault(x => x.CategoryLbl == note.Category).Items.FirstOrDefault(y => y.Title == note.Title).Note = note;
                adapter.NotifyItemChanged(position);
            };

            dialog.OnError += () =>
            {
                Snackbar.Make(recycler, "Invalid note. Please retry editing.", Snackbar.LengthLong).Show();
            };
            dialog.Show(FragmentManager, "NOTESFRAG");
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

        public override void OnBackPressed()
        {
            NavigateAway();
            base.OnBackPressed();
        }

        private void NavigateAway()
        {
            var intent = new Intent(this, typeof(CategoryActivity));
            NavigateUpTo(intent);
            OverridePendingTransition(Resource.Animation.push_up_in, Resource.Animation.push_up_out);
        }

        protected override void OnStop()
        {
            DBAssist.SerializeDB(notesdb, notes);
            base.OnStop();
        }
    }
}