using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Helpers;
using ProgrammingIdeas.Adapters;
using ProgrammingIdeas.Fragment;
using ProgrammingIdeas.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProgrammingIdeas.Activities
{
    [Activity(Label = "Idea Bag 2", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class CategoryActivity : BaseActivity
    {
        private RecyclerView recyclerView;
        private CategoryAdapter adapter;
        private LinearLayoutManager manager;
        private FloatingActionButton bookmarksFab;
        private List<Category> categoryList = new List<Category>();
        private ProgressBar loadingCircle;

        public override int LayoutResource => Resource.Layout.categoryactivity;

        public override bool HomeAsUpEnabled => false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            bookmarksFab = FindViewById<FloatingActionButton>(Resource.Id.bookmarkFab);
            loadingCircle = FindViewById<ProgressBar>(Resource.Id.loadingCircle);
            SetupUI();

            if (Intent.GetBooleanExtra("NewIdeasNotif", false)) // New ideas notification started the app
            {
                try
                {
                    categoryList = Global.Categories;
                    ShowNewIdeasDialog();
                    Log.Debug("ALANSADEBUG", $"From new ideas notif? True");
                }
                catch (Exception e)
                {
                    Log.Debug("ALANSADEBUG", $"Exception occured. {e.Message}");
                }
            }
        }

        private async void SetupUI()
        {
            loadingCircle.Visibility = ViewStates.Visible;

            if (File.Exists(Global.IDEAS_PATH))
            {
                var cachedDb = await DBAssist.DeserializeDBAsync<List<Category>>(Global.IDEAS_PATH);
                if (cachedDb != null)
                {
                    Global.Categories = cachedDb;
                    categoryList.AddRange(cachedDb);
                    SetupList();
                    DBManager.StartLowkeyInvalidation();
                }
                else
                {
                    File.Delete(Global.IDEAS_PATH);
                    SetupUI();
                }
            }
            else
            {
                var snack = Snackbar.Make(bookmarksFab, "Getting ideas from server. Please wait.", Snackbar.LengthIndefinite);
                snack.Show();
                var response = await DBManager.DownloadIdeasFromGoogleDrive();
                if (response.Item1 != null)
                {
                    // Item1 is the db from the cloud storage
                    Global.Categories = response.Item1;
                    snack.Dismiss();
                    categoryList = response.Item1;
                    SetupList();
                }
                else
                {
                    loadingCircle.Visibility = ViewStates.Gone;
                    if (response.Item2 is TaskCanceledException)
                        snack.SetText("Couldn't download ideas. Your connection might be too slow.").SetAction("Retry", (v) => SetupUI()).Show();
                    else if (response.Item2 is HttpRequestException)
                        snack.SetText("Couldn't download ideas. Please check your connection and retry.").SetAction("Retry", (v) => SetupUI()).Show();
                }
            }
        }

        private void ShowNewIdeasDialog()
        {
            var newideastxtPath = Path.Combine(Global.APP_PATH, "newideastxt");
            if (File.Exists(newideastxtPath))
            {
                var dialogFrag = new NewIdeaFragment(GetNewIdeas());
                dialogFrag.Show(FragmentManager, "DIALOGFRAG");
            }
            else
                Toast.MakeText(this, "Downloading new ideas has not completed. Please wait.", ToastLength.Long).Show();
        }

        private void SetupList()
        {
            loadingCircle.Visibility = ViewStates.Gone;
            manager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(manager);
            adapter = new CategoryAdapter(categoryList);
            adapter.ItemClick += OnItemClick;
            recyclerView.SetAdapter(adapter);
            manager.ScrollToPosition(Global.CategoryScrollPosition);
            bookmarksFab.Click += BookmarksFab_Click;
        }

        private void OnItemClick(int position)
        {
            Global.CategoryScrollPosition = position;
            StartActivity(new Intent(this, typeof(IdeaListActivity)));
            OverridePendingTransition(Resource.Animation.push_left_in, Resource.Animation.push_left_out);
        }

        protected override void OnPause()
        {
            DBAssist.SerializeDBAsync(Global.IDEAS_PATH, Global.Categories);
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            adapter?.NotifyDataSetChanged(); // Highlights the last clicked idea
        }

        protected override void OnDestroy()
        {
            adapter.ItemClick -= OnItemClick;
            bookmarksFab.Click -= BookmarksFab_Click;
            base.OnDestroy();
        }

        private void BookmarksFab_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(BookmarksActivity)));
            OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.about:
                    StartActivity(new Intent(this, typeof(AboutActivity)));
                    OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
                    return true;

                case Resource.Id.changelog:
                    new AlertDialog.Builder(this)
                        .SetTitle($"Changelog {Resources.GetString(Resource.String.appNumber)}")
                        .SetMessage(Resources.GetString(Resource.String.changelog))
                        .SetPositiveButton("Dismiss", (sender, e) => { return; })
                        .Create()
                        .Show();
                    return true;

                case Resource.Id.submitIdea:
                    StartActivity(new Intent(this, typeof(SubmitIdeaActivity)));
                    OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
                    return true;

                case Resource.Id.donate:
                    StartActivity(new Intent(this, typeof(DonateActivity)));
                    OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
                    return true;

                case Resource.Id.newIdeas:
                    ShowNewIdeasDialog();
                    return true;

                case Resource.Id.notes:
                    StartActivity(new Intent(this, typeof(NotesActivity)));
                    OverridePendingTransition(Resource.Animation.push_down_in, Resource.Animation.push_down_out);
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// To indicate which ideas where new, I created a text file. A new idea was represented by {categoryIndex}-{ideaIndex}.
        /// So, "1-2" means that the idea is in Category 1 ie Numbers and was idea number 2 ie Tax Calculator.
        /// This method reads in the text file and generates the respective idea.
        /// </summary>
        /// <returns>The new ideas.</returns>
        private List<Idea> GetNewIdeas()
        {
            var newideastxtPath = Path.Combine(Global.APP_PATH, "newideastxt");
            var newItems = new List<Idea>();
            var newIdeas = new StreamReader(newideastxtPath).ReadToEnd();
            newIdeas = newIdeas.Replace("\"", string.Empty); // It's downloaded as a quoted string so we need to remove the quotes
            var newIdeasContent = newIdeas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < newIdeasContent.Length; i++)
            {
                var sContents = newIdeasContent[i].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                newItems.Add(categoryList[Convert.ToInt32(sContents[0]) - 1].Items.FirstOrDefault(x => x.Id - 1 == Convert.ToInt32(sContents[1]) - 1));
            }
            return newItems;
        }
    }
}