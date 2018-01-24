using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace ProgrammingIdeas.Adapters
{
    public class NotesAdapter : RecyclerView.Adapter
    {
        private readonly List<Note> notes;
        public Action<int> EditClicked, ViewNoteClicked, DeleteClicked;

        public NotesAdapter(List<Note> notes)
        {
            this.notes = notes;
        }

        public override int ItemCount => notes.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var note = notes[position];
            var noteViewHolder = holder as NoteViewHolder;
            noteViewHolder.Title.Text = $"{note.Category} • {note.Title}";
            noteViewHolder.Content.Text = note.Content;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.notesrow, parent, false);
            return new NoteViewHolder(row, EditClicked, ViewNoteClicked, DeleteClicked);
        }
    }

    public class NoteViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public TextView EditNote { get; set; }
        public TextView Content { get; set; }
        public TextView ViewNote { get; set; }
        public TextView DeleteNote { get; set; }

        public NoteViewHolder(View itemView, Action<int> EditClicked, Action<int> ViewNoteClicked, Action<int> DeleteClicked) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.notesTitle);
            EditNote = itemView.FindViewById<TextView>(Resource.Id.noteEdit);
            Content = itemView.FindViewById<TextView>(Resource.Id.notesContent);
            ViewNote = itemView.FindViewById<TextView>(Resource.Id.viewNote);
            DeleteNote = itemView.FindViewById<TextView>(Resource.Id.deleteNote);

            EditNote.Click += delegate
            {
                EditClicked?.Invoke(AdapterPosition);
            };

            ViewNote.Click += delegate
            {
                ViewNoteClicked?.Invoke(AdapterPosition);
            };

            DeleteNote.Click += delegate
            {
                DeleteClicked?.Invoke(AdapterPosition);
            };
        }
    }
}