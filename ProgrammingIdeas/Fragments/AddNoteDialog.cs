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
        private bool isEditingNote;

        public AddNoteDialog()
        {
        }

        public AddNoteDialog(string category, string title)
        {
            this.category = category;
            this.title = title;
        }

        public AddNoteDialog(Note note)
        {
            isEditingNote = true;
            this.note = note;
            title = note.Title;
            category = note.Category;
        }

        public override Dialog OnCreateDialog(Android.OS.Bundle savedInstanceState)
        {
            var inflater = (LayoutInflater)Activity.GetSystemService(Activity.LayoutInflaterService);
            view = inflater.Inflate(Resource.Layout.add_note_layout, null);
            nameTitle = view.FindViewById<EditText>(Resource.Id.noteTitle);
            noteContentTb = view.FindViewById<EditText>(Resource.Id.noteTb);
            clearNote = view.FindViewById<Button>(Resource.Id.clearNoteBtn);

            clearNote.Click += delegate
            {
                nameTitle.Text = string.Empty;
                noteContentTb.Text = string.Empty;
            };

            if (isEditingNote)
            {
                nameTitle.Text = note.Name;
                noteContentTb.Text = note.Content;
            }

            var dialog = new AlertDialog.Builder(Activity)
                .SetTitle(!isEditingNote ? "Add a note" : "Edit this note")
                .SetView(view)
                .SetPositiveButton("Save", (s, e) => SaveNote())
                .SetNegativeButton("Cancel", (s, e) => { return; })
                .Create();
            return dialog;
        }

        private void SaveNote()
        {
            using (var validator = new Validator())
            {
                validator.ValidateIsNotEmpty(nameTitle, true);
                validator.ValidateIsNotEmpty(noteContentTb, true);
                if (validator.PassedValidation)
                {
                    note = new Note()
                    {
                        Name = nameTitle.Text,
                        Category = category,
                        Content = noteContentTb.Text,
                        Title = title
                    };

                    OnNoteSave?.Invoke(note);
                }
                else
                    OnError?.Invoke();
            }
        }
    }
}