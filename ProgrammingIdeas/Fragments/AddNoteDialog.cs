using Android.App;
using Android.Views;
using Android.Widget;
using ProgrammingIdeas.Helpers;
using System;

namespace ProgrammingIdeas.Fragments
{
    public class AddNoteDialog : DialogFragment
    {
        public Action<Note> OnNoteSave;
        public Action OnError;
        private Note note;
        private string category, title;
        private EditText nameTitle, noteContentTb;
        private Button clearNote;
        private View view;
        private Validator validator;
        private bool IsEditing;

        public AddNoteDialog() : base()
        {
        }

        public AddNoteDialog(string category, string title)
        {
            this.category = category;
            this.title = title;
        }

        public AddNoteDialog(Note item)
        {
            IsEditing = true;
            this.note = item;
            this.title = item.Title;
            this.category = item.Category;
        }

        public override Dialog OnCreateDialog(Android.OS.Bundle savedInstanceState)
        {
            validator = new Validator(Activity);
            var inflater = (LayoutInflater)Activity.GetSystemService(Activity.LayoutInflaterService);
            view = inflater.Inflate(Resource.Layout.add_note_layout, null);
            nameTitle = view.FindViewById<EditText>(Resource.Id.noteTitle);
            noteContentTb = view.FindViewById<EditText>(Resource.Id.noteTb);
            clearNote = view.FindViewById<Button>(Resource.Id.clearNoteBtn);

            clearNote.Click += delegate { nameTitle.Text = ""; noteContentTb.Text = ""; };
            if (IsEditing)
            {
                nameTitle.Text = note.Name;
                noteContentTb.Text = note.Content;
            }

            var dialog = new AlertDialog.Builder(Activity)
                .SetTitle(!IsEditing ? "Add a note" : "Editing this note")
                .SetView(view)
                .SetPositiveButton("Save", (s, e) => SaveNote())
                .SetNegativeButton("Cancel", (s, e) => { return; })
                .Create();
            return dialog;
        }

        /*public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
		{
            validator = new Validator(Activity);
            view = inflater.Inflate(Resource.Layout.add_note_layout, container, false);
			nameTitle = view.FindViewById<EditText>(Resource.Id.noteTitle);
			noteContentTb = view.FindViewById<EditText>(Resource.Id.noteTb);
            clearNote = view.FindViewById<Button>(Resource.Id.clearNoteBtn);
            saveNote = view.FindViewById<Button>(Resource.Id.addNoteBtn);

            clearNote.Click += delegate { nameTitle.Text = ""; noteContentTb.Text = ""; };
            saveNote.Click += delegate { SaveNote(); };

            if (IsEditing)
            {
                Dialog.SetTitle("Editing this note");
                nameTitle.Text = note.Title;
                noteContentTb.Text = note.Content;
            }
            else
                Dialog.SetTitle("Add a note");
            return view;
		}*/

        private void SaveNote()
        {
            validator.CheckIfEmpty(nameTitle, "Note title");
            validator.CheckIfEmpty(noteContentTb, "Note description");
            if (validator.Result)
            {
                note = new Note() { Name = nameTitle.Text, Category = category, Content = noteContentTb.Text, Title = title };
                OnNoteSave?.Invoke(note);
            }
            else
                OnError?.Invoke();
        }
    }
}