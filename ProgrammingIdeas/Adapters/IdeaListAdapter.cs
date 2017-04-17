using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using ProgrammingIdeas.Animation;
using System;
using System.Collections.Generic;

namespace ProgrammingIdeas.Adapters
{
    public class IdeaListAdapter : RecyclerView.Adapter
    {
        private List<CategoryItem> itemsList;
        public Action<int> ItemClick;
        public Action<string> StateClicked;
        private Context ctx;
		private int index = 1;

        public IdeaListAdapter(List<CategoryItem> list, Context ctx)
        {
            this.ctx = ctx;
            itemsList = list;
        }

        public override int ItemCount
        {
            get
            {
                return itemsList.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var itemHolder = holder as ItemViewHolder;
            var item = itemsList[position];

            itemHolder.Difficulty.Text = item.Difficulty;
            itemHolder.Title.Text = item.Title;
			itemHolder.Id.Text = (position + 1).ToString();
            itemHolder.State.SetBackgroundResource(Resource.Color.undecidedColor);
            itemHolder.Root.SetBackgroundColor(Color.Transparent);
            switch (item.State)
            {
                case "undecided":
                    itemHolder.State.SetBackgroundResource(Resource.Color.undecidedColor);
                    break;

                case "inprogress":
                    itemHolder.State.SetBackgroundResource(Resource.Color.inProgressColor);
                    break;

                case "done":
                    itemHolder.State.SetBackgroundResource(Resource.Color.doneColor);
                    break;
            }
            if (position == Global.ItemScrollPosition)
                itemHolder.Root.SetBackgroundResource(Resource.Color.highlight);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.idealistrow, parent, false);
            return new ItemViewHolder(row, (int obj) => { ItemClick?.Invoke(obj); }, ctx, StateClicked);
        }
    }

    public class ItemViewHolder : RecyclerView.ViewHolder, View.IOnLongClickListener
    {
        public TextView Title { get; set; }
        public TextView Difficulty { get; set; }
        public TextView Id { get; set; }
        public View State { get; set; }
        public LinearLayout Root { get; set; }
        private Context ctx;
        private TextView inprogress, done, undecided;

        private Action<string> StateClicked;

        public ItemViewHolder(View itemView, Action<int> listener, Context ctx, Action<string> StateClicked) : base(itemView)
        {
            this.StateClicked = StateClicked;
            this.ctx = ctx;
            itemView.Click += (sender, e) => listener(AdapterPosition);
            itemView.SetOnLongClickListener(this);
            Title = itemView.FindViewById<TextView>(Resource.Id.title);
            Difficulty = itemView.FindViewById<TextView>(Resource.Id.difficulty);
            Id = itemView.FindViewById<TextView>(Resource.Id.itemId);
            State = itemView.FindViewById<View>(Resource.Id.progressState);
            Root = itemView.FindViewById<LinearLayout>(Resource.Id.layoutRoot);
        }

        public bool OnLongClick(View v)
        {
            var addView = LayoutInflater.From(Application.Context).Inflate(Resource.Layout.ideaprogressfragment, null);
            inprogress = addView.FindViewById<TextView>(Resource.Id.inprogressText);
            undecided = addView.FindViewById<TextView>(Resource.Id.undecidedText);
            done = addView.FindViewById<TextView>(Resource.Id.doneText);
            var builder = new AlertDialog.Builder(ctx);
            builder.SetView(addView);
            builder.SetNegativeButton("Cancel", (sender, e) => { return; });
            var dialog = builder.Create();
            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dialog.Show();
            inprogress.Click += (sender, e) => { StateClicked?.Invoke($"{AdapterPosition}-inprogress"); AnimateStateChange(inprogress); dialog.Dismiss(); };
            undecided.Click += (sender, e) => { StateClicked?.Invoke($"{AdapterPosition}-undecided"); AnimateStateChange(undecided); dialog.Dismiss(); };
            done.Click += (sender, e) => { StateClicked?.Invoke($"{AdapterPosition}-done"); AnimateStateChange(done); dialog.Dismiss(); };
            return true;
        }

        private void AnimateStateChange(View v)
        {
            AnimHelper.Animate(v, "rotationY", 500, new AnticipateOvershootInterpolator(), 0, 360);
        }
    }
}