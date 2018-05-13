using Adapters;
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

namespace ProgrammingIdeas.Activities
{
    [Activity(Label = "Bookmarks", Theme = "@style/AppTheme")]
    public class BookmarksActivity : BaseActivity
    {
        private RecyclerView recyclerView;
        private BookmarkListAdapter adapter;
        private List<Idea> bookmarksList = new List<Idea>(); private View emptyState;
        private ProgressBar progressBar;

        public override int LayoutResource => Resource.Layout.bookmarksactivity;

        public override bool HomeAsUpEnabled => true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.bookmarkRecyclerView);
            emptyState = FindViewById(Resource.Id.empty);
            progressBar = FindViewById<ProgressBar>(Resource.Id.completedIdeasBar);
            SetupUI();
        }

        private async void SetupUI()
        {
            if (File.Exists(Global.BOOKMARKS_PATH)) // If bookmarks exist
            {
                bookmarksList = await DBSerializer.DeserializeDBAsync<List<Idea>>(Global.BOOKMARKS_PATH);
                bookmarksList = bookmarksList ?? new List<Idea>();
                if (bookmarksList.Count > 0)
                {
                    adapter = new BookmarkListAdapter(bookmarksList);
                    adapter.ItemClick += OnItemClick;
                    var manager = new LinearLayoutManager(this);
                    recyclerView.SetLayoutManager(manager);
                    recyclerView.SetAdapter(adapter);
                    recyclerView.SetItemAnimator(new DefaultItemAnimator());
                    manager.ScrollToPosition(Global.BookmarkScrollPosition);
                    adapter.StateClicked += StateClicked;
                    ShowProgress();
                }
                else
                    ShowEmptyState();
            }
            else
                ShowEmptyState();
        }

        protected override void OnResume()
        {
            base.OnResume();
            adapter?.NotifyDataSetChanged(); // Highlights the last clicked/viewed idea
        }

        private void OnItemClick(int position)
        {
            Global.BookmarkScrollPosition = position;
            StartActivity(new Intent(this, typeof(BookmarkDetailsActivity)));
            OverridePendingTransition(Resource.Animation.push_left_in, Resource.Animation.push_left_out);
        }

        /// <summary>
        /// Shows a progress bar indicating how many ideas the user has marked as completed
        /// </summary>
        private void ShowProgress()
        {
            var completedIdeasCount = bookmarksList.FindAll(x => x.State == Status.Done).Count;
            progressBar.Max = bookmarksList.Count;
            progressBar.Progress = 0;
            progressBar.IncrementProgressBy(completedIdeasCount);
        }

        private void ShowEmptyState()
        {
            recyclerView.Visibility = ViewStates.Gone;
            emptyState.Visibility = ViewStates.Visible;
            emptyState.FindViewById<TextView>(Resource.Id.infoText).Text = "You have no bookmarks yet.";
        }

        /// <summary>
        /// The user just selected a new state [done, inprogress, undecided] for an idea
        /// </summary>
        /// <param name="title">The name of the idea whose state changed</param>
        /// <param name="state">The state that was selected</param>
        /// <param name="adapterPos">The index of the idea in its category</param>
        private void StateClicked(string title, string state, int adapterPos)
        {
            if (bookmarksList != null && bookmarksList.Count != 0)
            {
                bookmarksList[adapterPos].State = state;
                adapter.NotifyItemChanged(adapterPos);
                Global.Categories.FirstOrDefault(x => x.CategoryLbl == bookmarksList[adapterPos].Category).Items.FirstOrDefault(x => x.Title == title).State = state;
                ShowProgress();
                DBSerializer.SerializeDBAsync(Global.BOOKMARKS_PATH, bookmarksList);
            }
        }

        protected override void OnPause()
        {
            DBSerializer.SerializeDBAsync(Global.IDEAS_PATH, Global.Categories);
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Resets position back to zero so the bookmark highlight shows at the top of the list if the user re-enters
            Global.BookmarkScrollPosition = 0;
        }
    }
}