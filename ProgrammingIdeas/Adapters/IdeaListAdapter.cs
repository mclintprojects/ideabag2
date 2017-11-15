using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ProgrammingIdeas.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgrammingIdeas.Adapters
{
    public class IdeaListAdapter : RecyclerView.Adapter
    {
        private readonly List<Idea> itemsList;
        private List<Idea> bookmarkedItems;
        public Action<int> ItemClick;
        public Action<string, string, int> StateClicked;

        public IdeaListAdapter(List<Idea> list)
        {
            itemsList = list;
            GetBookmarks();
        }

        /// <summary>
        /// Checks and sets a bookmarked icon for ideas in the list which items have been bookmarked
        /// </summary>
        public void RefreshBookmarks()
        {
            Global.RefreshBookmarks = false;
            GetBookmarks();
            NotifyDataSetChanged();
        }

        private void GetBookmarks()
        {
            bookmarkedItems = DBSerializer.DeserializeDB<List<Idea>>(Global.BOOKMARKS_PATH);
            bookmarkedItems = bookmarkedItems ?? new List<Idea>();
        }

        public override int ItemCount => itemsList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var itemHolder = holder as IdeaViewHolder;
            var item = itemsList[position];

            itemHolder.Difficulty.Text = item.Difficulty;
            itemHolder.Title.Text = item.Title;
            itemHolder.Id.Text = item.Id.ToString();
            itemHolder.State.SetBackgroundResource(Resource.Color.undecidedColor);
            itemHolder.Root.SetBackgroundColor(Color.Transparent);
            switch (item.State)
            {
                case Status.Undecided:
                    itemHolder.State.SetBackgroundResource(Resource.Color.undecidedColor);
                    break;

                case Status.InProgress:
                    itemHolder.State.SetBackgroundResource(Resource.Color.inProgressColor);
                    break;

                case Status.Done:
                    itemHolder.State.SetBackgroundResource(Resource.Color.doneColor);
                    break;
            }

            if (position == Global.IdeaScrollPosition)
                itemHolder.Root.SetBackgroundResource(Resource.Color.highlight);

            // if idea is bookmarked, show bookmarked icon
            if (bookmarkedItems?.FirstOrDefault(x => x.Title == item.Title) != null)
            {
                itemHolder.BookmarkIndicator.Visibility = ViewStates.Visible;
                itemHolder.BookmarkIndicator.SetColorFilter(Color.ParseColor("#FFA000"), PorterDuff.Mode.SrcIn);
            }
            else
                itemHolder.BookmarkIndicator.Visibility = ViewStates.Gone;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.idealistrow, parent, false);
            return new IdeaViewHolder(row, itemsList, ItemClick, StateClicked);
        }
    }

    public class IdeaViewHolder : RecyclerView.ViewHolder, View.IOnLongClickListener
    {
        public TextView Title { get; set; }
        public TextView Difficulty { get; set; }
        public TextView Id { get; set; }
        public View State { get; set; }
        public LinearLayout Root { get; set; }
        public ImageView BookmarkIndicator { get; set; }

        private readonly Action<string, string, int> StateClicked;
        private readonly List<Idea> ideasList;

        public IdeaViewHolder(View itemView, List<Idea> ideasList, Action<int> listener, Action<string, string, int> StateClicked) : base(itemView)
        {
            this.StateClicked = StateClicked;
            this.ideasList = ideasList;
            itemView.Click += (sender, e) => listener(AdapterPosition);
            itemView.SetOnLongClickListener(this);
            Title = itemView.FindViewById<TextView>(Resource.Id.title);
            Difficulty = itemView.FindViewById<TextView>(Resource.Id.difficulty);
            Id = itemView.FindViewById<TextView>(Resource.Id.itemId);
            State = itemView.FindViewById<View>(Resource.Id.progressState);
            Root = itemView.FindViewById<LinearLayout>(Resource.Id.layoutRoot);
            BookmarkIndicator = itemView.FindViewById<ImageView>(Resource.Id.bookmarkIndicator);
        }

        public bool OnLongClick(View v)
        {
            var view = LayoutInflater.From(App.CurrentActivity).Inflate(Resource.Layout.ideaprogressfragment, null);
            var inprogress = view.FindViewById<TextView>(Resource.Id.inprogressText);
            var undecided = view.FindViewById<TextView>(Resource.Id.undecidedText);
            var done = view.FindViewById<TextView>(Resource.Id.doneText);

            var builder = new AlertDialog.Builder(App.CurrentActivity);
            builder.SetView(view);
            builder.SetNegativeButton("Cancel", (sender, e) => { });
            var dialog = builder.Create();
            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);

            // User changed idea progress state to InProgress
            inprogress.Click += (sender, e) =>
            {
                StateClicked?.Invoke($"{ideasList[AdapterPosition].Title}", Status.InProgress, AdapterPosition); dialog.Dismiss();
            };

            // User changed idea progress state to Undecided
            undecided.Click += (sender, e) =>
            {
                StateClicked?.Invoke($"{ideasList[AdapterPosition].Title}", Status.Undecided, AdapterPosition); dialog.Dismiss();
            };

            // User changed idea progress state to Done
            done.Click += (sender, e) =>
            {
                StateClicked?.Invoke($"{ideasList[AdapterPosition].Title}", Status.Done, AdapterPosition); dialog.Dismiss();
            };

            dialog.Show();
            return true;
        }
    }
}