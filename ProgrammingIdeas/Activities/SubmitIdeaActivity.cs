using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using ProgrammingIdeas.Helpers;
using System;

namespace ProgrammingIdeas.Activities
{
    [Activity(Label = "Submit an idea", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class SubmitIdeaActivity : BaseActivity
    {
        private Button submitBtn;
        private EditText author, ideaTitle, description;
        private string selectedCategory = string.Empty;

        public override int LayoutResource => Resource.Layout.submitideaactivity;

        public override bool HomeAsUpEnabled => true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var spinner = FindViewById<Spinner>(Resource.Id.spinner);
            author = FindViewById<EditText>(Resource.Id.authorTb);
            ideaTitle = FindViewById<EditText>(Resource.Id.submitTitle);
            description = FindViewById<EditText>(Resource.Id.ideaDescription);

            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.categories, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
            spinner.ItemSelected += Spinner_ItemSelected;
            submitBtn = FindViewById<Button>(Resource.Id.submitBtn);
            submitBtn.Click += (sender, e) =>
            {
                using (var validator = new Validator())
                {
                    validator.ValidateIsNotEmpty(author, true);
                    validator.ValidateIsNotEmpty(ideaTitle, true);
                    validator.ValidateIsNotEmpty(description, true);
                    if (validator.PassedValidation)
                    {
                        var submitIntent = new Intent(Intent.ActionSend);
                        submitIntent.SetData(Android.Net.Uri.Parse("mailto:"));
                        submitIntent.PutExtra(Intent.ExtraEmail, new string[] { "alansagh@gmail.com" });
                        submitIntent.PutExtra(Intent.ExtraSubject, $"IdeaBag 2 Submission {DateTime.Now.ToUniversalTime()}");
                        string body = $"Author: {author.Text}\r\nCategory: {selectedCategory}\r\nTitle: {ideaTitle.Text}\r\n\r\nDescription\r\n{description.Text}";
                        submitIntent.PutExtra(Intent.ExtraText, body);
                        submitIntent.SetType("message/rfc822");
                        StartActivity(Intent.CreateChooser(submitIntent, "Submit idea to developer via e-mail"));
                    }
                }
            };
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender;
            selectedCategory = spinner.GetItemAtPosition(e.Position).ToString();
        }

        public override void NavigateAway()
        {
            if (SubmissionInProgress())
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Cancel submission");
                builder.SetMessage("Are you sure you want to cancel submission?");
                builder.SetPositiveButton("Yes", (sender, e) => base.NavigateAway());
                builder.SetNegativeButton("No", (sender, e) => { return; });
                var dialog = builder.Create();
                dialog.SetCancelable(false);
                dialog.Show();
            }
            else
                base.NavigateAway();
        }

        public bool SubmissionInProgress()
        {
            if (author.Text.Length != 0 || ideaTitle.Text.Length != 0 || description.Text.Length != 0)
                return true;
            return false;
        }
    }
}