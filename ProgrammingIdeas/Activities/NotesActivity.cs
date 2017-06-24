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
		private List<Idea> bookmarkedItems;
		private RecyclerView recycler;
		private RecyclerView.LayoutManager manager;
		private NotesAdapter adapter;
		private readonly string notesdb = Path.Combine(Global.APP_PATH, "notesdb");
		private readonly string bookmarksdb = Path.Combine(Global.APP_PATH, "bookmarks.json");
		private View emptyState;

		public override int LayoutResource => Resource.Layout.notesactivity;

		public override bool HomeAsUpEnabled => true;

		protected async override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			notes = await DBAssist.DeserializeDBAsync<List<Note>>(notesdb);
			notes = notes ?? new List<Note>();

			bookmarkedItems = await DBAssist.DeserializeDBAsync<List<Idea>>(bookmarksdb);
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

				recycler.SetAdapter(adapter);
				recycler.SetLayoutManager(manager);
				recycler.SetItemAnimator(new DefaultItemAnimator());
			}
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
				var foundIdea = Global.Categories.FirstOrDefault(x => x.CategoryLbl == note.Category).Items.FirstOrDefault(y => y.Title == note.Title);
				if (foundIdea != null)
					foundIdea.Note = note;

				var foundBookmark = bookmarkedItems.FirstOrDefault(x => x.Title == notes[position].Title);
				if (foundBookmark != null)
					foundBookmark.Note = note;
				adapter.NotifyItemChanged(position);
			};

			dialog.OnError += () => Snackbar.Make(recycler, "Invalid note. Please retry editing.", Snackbar.LengthLong).Show();

			dialog.Show(FragmentManager, "NOTESFRAG");
		}

		protected override void OnStop()
		{
			if (notes != null)
				DBAssist.SerializeDBAsync(notesdb, notes);
			if (bookmarkedItems != null)
				DBAssist.SerializeDBAsync(bookmarksdb, bookmarkedItems);
			base.OnStop();
		}
	}
}