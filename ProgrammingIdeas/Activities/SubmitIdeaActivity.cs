using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace ProgrammingIdeas
{
    [Activity(Label = "SubmitIdeaActivity", Theme = "@style/AppTheme")]
    public class SubmitIdeaActivity : Activity
    {
        private string selectedCategory = "";
        private Button submitBtn;
        private EditText author, ideaTitle, description;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.submitideaactivity);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            Title = "Submit an idea";
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
                if (author.Text.Length > 0 && ideaTitle.Text.Length > 0 && description.Text.Length > 0)
                {
                    var submitIntent = new Intent(Intent.ActionSend);
                    submitIntent.SetData(Android.Net.Uri.Parse("mailto:"));
                    submitIntent.PutExtra(Intent.ExtraEmail, new string[] { "alansagh@gmail.com" });
                    submitIntent.PutExtra(Intent.ExtraSubject, $"IdeaBag 2 Submission {DateTime.Now.ToUniversalTime()}");
                    string body = $"Author: {author.Text}\r\nTitle: {ideaTitle.Text}\r\n\r\nDescription\r\n{description.Text}";
                    submitIntent.PutExtra(Intent.ExtraText, body);
                    submitIntent.SetType("message/rfc822");
                    StartActivity(Intent.CreateChooser(submitIntent, "Submit idea to developer via e-mail"));
                }
                else
                    Toast.MakeText(this, "Submission failed. Some entry fields are still empty.", ToastLength.Long).Show();
            };
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender;
            selectedCategory = spinner.GetItemAtPosition(e.Position).ToString();
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
            if (submissionInProgress())
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Cancel submission");
                builder.SetMessage("Are you sure you want to cancel submission?");
                builder.SetPositiveButton("Yes", (sender, e) =>
                {
                    var intent = new Intent(this, typeof(CategoryActivity));
                    NavigateUpTo(intent);
                    OverridePendingTransition(Resource.Animation.push_up_in, Resource.Animation.push_up_out);
                });
                builder.SetNegativeButton("No", (sender, e) => { return; });
                var dialog = builder.Create();
                dialog.SetCancelable(false);
                dialog.Show();
            }
            else
            {
                var intent = new Intent(this, typeof(CategoryActivity));
                NavigateUpTo(intent);
                OverridePendingTransition(Resource.Animation.push_up_in, Resource.Animation.push_up_out);
                base.OnBackPressed();
            }
        }

        public void NavigateAway()
        {
            if (submissionInProgress())
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Cancel submission");
                builder.SetMessage("Are you sure you want to cancel submission?");
                builder.SetPositiveButton("Yes", (sender, e) =>
                {
                   var intent = new Intent(this, typeof(CategoryActivity));
                    NavigateUpTo(intent);
                    OverridePendingTransition(Resource.Animation.push_up_in, Resource.Animation.push_up_out);
                });
                builder.SetNegativeButton("No", (sender, e) => { return; });
                var dialog = builder.Create();
                dialog.SetCancelable(false);
                dialog.Show();
            }
            else
            {
                var intent = new Intent(this, typeof(CategoryActivity));
                NavigateUpTo(intent);
                OverridePendingTransition(Resource.Animation.push_up_in, Resource.Animation.push_up_out);
            }
        }

        public bool submissionInProgress()
        {
            if (author.Text.Length != 0 || ideaTitle.Text.Length != 0 || description.Text.Length != 0)
                return true;
           	return false;
        }
    }
}