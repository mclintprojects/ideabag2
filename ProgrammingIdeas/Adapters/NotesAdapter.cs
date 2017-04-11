using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace ProgrammingIdeas
{
    public class NotesAdapter : RecyclerView.Adapter
    {
        private List<Note> notes = new List<Note>();
		public event EventHandler<int> EditClicked;

        public NotesAdapter(List<Note> notes)
        {
            this.notes = notes;
        }

        public override int ItemCount
        {
            get
            {
                return notes.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var note = notes[position];
            var view = holder as NoteViewHolder;
            view.Title.Text = $"{note.Category} >> {note.Title}";
            view.Content.Text = note.Content;
            view.EditNote.Click += (sender, e) =>
            {
				EditClicked?.Invoke(sender, position);
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.notesrow, parent, false);
            return new NoteViewHolder(row);
        }
    }

    public class NoteViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public TextView EditNote { get; set; }
        public TextView Content { get; set; }
        public EditText NoteInput { get; set; }
        public ViewSwitcher Switcher { get; set; }

        public NoteViewHolder(View itemView) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.notesTitle);
			EditNote = itemView.FindViewById<TextView>(Resource.Id.notesEditBtn);
            Content = itemView.FindViewById<TextView>(Resource.Id.notesContent);
            NoteInput = itemView.FindViewById<EditText>(Resource.Id.notesEdit);
            Switcher = itemView.FindViewById<ViewSwitcher>(Resource.Id.notesActivitySwitcher);
        }
    }
}