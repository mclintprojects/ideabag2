﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using ProgrammingIdeas.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using ProgrammingIdeas.Helpers;
using Newtonsoft.Json;
using Android.Support.V4.Content;

namespace ProgrammingIdeas.Adapters
{
    public class IdeaListAdapter : RecyclerView.Adapter
    {
        private List<Idea> itemsList;
        private List<Idea> bookmarkedItems;
        public Action<int> ItemClick;
        public Action<string> StateClicked;

        public IdeaListAdapter(List<Idea> list)
        {
            itemsList = list;
            Initialize();
        }

        private async void Initialize()
        {
            bookmarkedItems = await DBAssist.DeserializeDBAsync<List<Idea>>(System.IO.Path.Combine(Global.APP_PATH, "bookmarks.json"));
            bookmarkedItems = bookmarkedItems ?? new List<Idea>();
        }

        public override int ItemCount => itemsList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var itemHolder = holder as ItemViewHolder;
            var item = itemsList[position];

            itemHolder.Difficulty.Text = item.Difficulty;
            itemHolder.Title.Text = item.Title;
            itemHolder.Id.Text = item.Id.ToString();
            itemHolder.State.SetBackgroundResource(Resource.Color.undecidedColor);
            itemHolder.Root.SetBackgroundColor(Color.Transparent);
            itemHolder.BookmarkIndicator.Visibility = ViewStates.Gone;
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
            if (position == Global.ItemScrollPosition)
                itemHolder.Root.SetBackgroundResource(Resource.Color.highlight);

            // if idea is bookmarked, show bokmark indicator
            if (bookmarkedItems.FirstOrDefault(x => x.Title == item.Title) != null)
            {
                itemHolder.BookmarkIndicator.Visibility = ViewStates.Visible;
                itemHolder.BookmarkIndicator.SetColorFilter(Color.ParseColor("#FFA000"), PorterDuff.Mode.SrcIn);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.idealistrow, parent, false);
            return new ItemViewHolder(row, (int obj) => { ItemClick?.Invoke(obj); }, StateClicked);
        }
    }

    public class ItemViewHolder : RecyclerView.ViewHolder, View.IOnLongClickListener
    {
        public TextView Title { get; set; }
        public TextView Difficulty { get; set; }
        public TextView Id { get; set; }
        public View State { get; set; }
        public LinearLayout Root { get; set; }
        public ImageView BookmarkIndicator { get; set; }
        private Context ctx;
        private Action<string> StateClicked;

        public ItemViewHolder(View itemView, Action<int> listener, Action<string> StateClicked) : base(itemView)
        {
            this.StateClicked = StateClicked;
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
            var addView = LayoutInflater.From(Application.Context).Inflate(Resource.Layout.ideaprogressfragment, null);
            var inprogress = addView.FindViewById<TextView>(Resource.Id.inprogressText);
            var undecided = addView.FindViewById<TextView>(Resource.Id.undecidedText);
            var done = addView.FindViewById<TextView>(Resource.Id.doneText);

            var builder = new AlertDialog.Builder(App.CurrentActivity);
            builder.SetView(addView);
            builder.SetNegativeButton("Cancel", (sender, e) => { return; });
            var dialog = builder.Create();
            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            inprogress.Click += (sender, e) => { StateClicked?.Invoke($"{AdapterPosition}-{Status.InProgress}"); dialog.Dismiss(); };
            undecided.Click += (sender, e) => { StateClicked?.Invoke($"{AdapterPosition}-{Status.Undecided}"); dialog.Dismiss(); };
            done.Click += (sender, e) => { StateClicked?.Invoke($"{AdapterPosition}-{Status.Done}"); dialog.Dismiss(); };

            dialog.Show();
            return true;
        }
    }
}