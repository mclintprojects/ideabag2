using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ProgrammingIdeas.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ProgrammingIdeas
{
	[Activity(Label = "Notes")]
	public class NotesActivity : Activity
	{
		private List<Note> notes;
		private RecyclerView recycler;
		private RecyclerView.LayoutManager manager;
		private NotesAdapter adapter;
		private ViewSwitcher switcher, notesActivitySwitcher;
		private string notesdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "notesdb");
		string noteText;
		bool isNoteEditing = false;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			SetContentView(Resource.Layout.notesactivity);
			ActionBar.SetHomeButtonEnabled(true);
			ActionBar.SetDisplayHomeAsUpEnabled(true);
			notes = JsonConvert.DeserializeObject<List<Note>>(DBAssist.DeserializeDB(notesdb));
			if (notes == null)
				notes = new List<Note>();
			recycler = FindViewById<RecyclerView>(Resource.Id.notesRecyclerView);
			switcher = FindViewById<ViewSwitcher>(Resource.Id.notesSwitcher);
			notesActivitySwitcher = FindViewById<ViewSwitcher>(Resource.Id.notesActivitySwitcher);
			manager = new LinearLayoutManager(this);
			adapter = new NotesAdapter(notes);
			adapter.EditClicked += Adapter_EditClicked;
			recycler.SetAdapter(adapter);
			recycler.SetLayoutManager(manager);
			recycler.SetItemAnimator(new DefaultItemAnimator());
			if (notes.Count == 0)
				switcher.ShowNext();
			base.OnCreate(savedInstanceState);
		}

		void Adapter_OnAdapterEmpty(object sender, System.EventArgs e)
		{
			switcher.ShowNext();
		}

		void Adapter_EditClicked(object sender, int e)
		{
			var view = recycler.GetChildAt(e);
			var input = view.FindViewById<TextView>(Resource.Id.notesEdit);
			var switcher = view.FindViewById<ViewSwitcher>(Resource.Id.notesActivitySwitcher);
			var editNote = view.FindViewById<TextView>(Resource.Id.notesEditBtn);
			var content = view.FindViewById<TextView>(Resource.Id.notesContent);

			if (isNoteEditing == true) //user wants to save note
			{
				noteText = input.Text;
				switcher.ShowPrevious();
				editNote.Text = noteText;
				isNoteEditing = false;
				editNote.Text = "Edit this note";
				var note = notes[e];
				var newNote = new Note() { Category = note.Category, Content = noteText.Length == 0 ? null : noteText, Title = note.Title };
				var foundNote = notes.FirstOrDefault(x => x.Title == note.Title);
				if (foundNote == null) // existing note wasn't found
				{
					if (newNote.Content != null)
						notes.Add(newNote);
					else
						editNote.Text = "You have no notes for this idea. Tap the button below to add one.";
				}
				else //existing note was found
				{
					if (newNote.Content == null)
					{
						editNote.Text = "You have no notes for this idea. Tap the button below to add one.";
						notes.Remove(foundNote);
						adapter.NotifyItemRemoved(e);
					}
					else
					{
						notes.Remove(foundNote);
						notes.Add(newNote);
					}
				}
				content.Text = noteText;
			}
			else
			{ //user wants to edit note
				if (content.Text.Contains("You have no notes for this idea"))
					input.Text = "";
				else
					input.Text = content.Text;
				input.RequestFocus();
				isNoteEditing = true;
				switcher.ShowNext();
				editNote.Text = "Save this note";
			}
			if (notes.Count == 0)
				Adapter_OnAdapterEmpty(sender, new System.EventArgs());
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