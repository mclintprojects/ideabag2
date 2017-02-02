using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ProgrammingIdeas.Helpers;
using System.Collections.Generic;
using System.IO;

namespace ProgrammingIdeas
{
    [Activity(Label = "Notes")]
    public class NotesActivity : Activity
    {
        List<Note> notes;
        RecyclerView recycler;
        RecyclerView.LayoutManager manager;
        NotesAdapter adapter;
        ViewSwitcher switcher, notesActivitySwitcher;
        string notesdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "notesdb");

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.notesactivity);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            notes = (List<Note>)DBAssist.DeserializeDB(notesdb, notes);
            if (notes == null)
                notes = new List<Note>();
            recycler = FindViewById<RecyclerView>(Resource.Id.notesRecyclerView);
            switcher = FindViewById<ViewSwitcher>(Resource.Id.notesSwitcher);
            notesActivitySwitcher = FindViewById<ViewSwitcher>(Resource.Id.notesActivitySwitcher);
            manager = new LinearLayoutManager(this);
            adapter = new NotesAdapter(notes);
            adapter.OnAdapterEmpty += (sender, e) => { switcher.ShowNext(); };
            recycler.SetAdapter(adapter);
            recycler.SetLayoutManager(manager);
            recycler.SetItemAnimator(new DefaultItemAnimator());
            if (notes.Count == 0)
                switcher.ShowNext();
            base.OnCreate(savedInstanceState);
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

        void NavigateAway()
        {
            Intent intent = new Intent(this, typeof(MainActivity));
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