using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgrammingIdeas
{
    public class NotesAdapter : RecyclerView.Adapter
    {
        List<Note> notes = new List<Note>();

        public event EventHandler OnAdapterEmpty;

        bool isNoteEditing;
        string noteText;

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
                if (isNoteEditing == true) //user wants to save note
                {
                    noteText = view.NoteInput.Text;
                    view.Switcher.ShowPrevious();
                    view.EditNote.Text = noteText;
                    isNoteEditing = false;
                    view.EditNote.Text = "Edit this note";
                    var newNote = new Note() { Category = note.Category, Content = noteText.Length == 0 ? null : noteText, Title = note.Title };
                    var foundNote = notes.FirstOrDefault(x => x.Title == note.Title);
                    if (foundNote == null) // existing note wasn't found
                    {
                        if (newNote.Content != null)
                            notes.Add(newNote);
						else
							view.EditNote.Text = "You have no notes for this idea. Tap the button below to add one.";
                    }
                    else //existing note was found
                    {
                        if (newNote.Content == null)
                        {
                            view.EditNote.Text = "You have no notes for this idea. Tap the button below to add one.";
                            notes.Remove(foundNote);
                        }
                        else
                        {
                            notes.Remove(foundNote);
                            notes.Add(newNote);
                        }
                    }
                    view.Content.Text = noteText;
                }
                else
                { //user wants to edit note
                    if (view.Content.Text.Contains("You have no notes for this idea"))
                        view.NoteInput.Text = "";
                    else
                        view.NoteInput.Text = view.Content.Text;
                    view.NoteInput.RequestFocus();
                    isNoteEditing = true;
                    view.Switcher.ShowNext();
                    view.EditNote.Text = "Save this note";
                }
                if (notes.Count == 0)
                    OnAdapterEmpty?.Invoke(this, e);
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
            EditNote = itemView.FindViewById<TextView>(Resource.Id.notesCategory);
            Content = itemView.FindViewById<TextView>(Resource.Id.notesContent);
            NoteInput = itemView.FindViewById<EditText>(Resource.Id.notesEdit);
            Switcher = itemView.FindViewById<ViewSwitcher>(Resource.Id.notesActivitySwitcher);
        }
    }
}