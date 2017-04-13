using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Support.V4.Content;

namespace ProgrammingIdeas
{
    public class ItemAdapter : RecyclerView.Adapter
    {
        private List<CategoryItem> itemsList;

        public event EventHandler<int> ItemClick;

        public event EventHandler<string> StateClicked;

        private Context ctx;

        public ItemAdapter(List<CategoryItem> list, Context ctx)
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
            var itemHolder = holder as itemViewHolder;
            var item = itemsList[position];

            itemHolder.difficulty.Text = item.Difficulty;
            itemHolder.title.Text = item.Title;
            itemHolder.id.Text = item.Id.ToString();
            itemHolder.State.Background.SetTint(ContextCompat.GetColor(Application.Context, Resource.Color.undecidedColor));
			itemHolder.Root.Background?.SetTint(ContextCompat.GetColor(Application.Context, Resource.Color.white));
            switch (item.State)
            {
                case "undecided":
                    itemHolder.State.Background.SetTint(ContextCompat.GetColor(Application.Context, Resource.Color.undecidedColor));
                    break;

                case "inprogress":
                    itemHolder.State.Background.SetTint(ContextCompat.GetColor(Application.Context, Resource.Color.inProgressColor));
                    break;

                case "done":
                    itemHolder.State.Background.SetTint(ContextCompat.GetColor(Application.Context, Resource.Color.doneColor));
                    break;
            }
            if (position == Global.ItemScrollPosition)
                itemHolder.Root.Background?.SetTint(Color.ParseColor("#ffff7b"));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.idealistrow, parent, false);
            return new itemViewHolder(row, OnClick, ctx, ref StateClicked);
        }

        private void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick?.Invoke(this, position);
        }
    }

    public class itemViewHolder : RecyclerView.ViewHolder, View.IOnLongClickListener
    {
        public TextView title { get; set; }
        public TextView difficulty { get; set; }
        public TextView id { get; set; }
        public View State { get; set; }
        public LinearLayout Root { get; set; }
        private Context ctx;
        private ImageView inprogress, done, undecided;

        private event EventHandler<string> stateClick;

        public itemViewHolder(View itemView, Action<int> listener, Context ctx, ref EventHandler<string> stateClick) : base(itemView)
        {
            this.stateClick = stateClick;
            this.ctx = ctx;
            View row = itemView;
            row.Click += (sender, e) => listener(AdapterPosition);
            row.SetOnLongClickListener(this);
            title = itemView.FindViewById<TextView>(Resource.Id.title);
            difficulty = itemView.FindViewById<TextView>(Resource.Id.difficulty);
            id = itemView.FindViewById<TextView>(Resource.Id.itemId);
            State = itemView.FindViewById<View>(Resource.Id.progressState);
            Root = itemView.FindViewById<LinearLayout>(Resource.Id.layoutRoot);
        }

        public bool OnLongClick(View v)
        {
            var addView = LayoutInflater.From(Application.Context).Inflate(Resource.Layout.ideaprogressfragment, null);
            inprogress = addView.FindViewById<ImageView>(Resource.Id.inProgress);
            undecided = addView.FindViewById<ImageView>(Resource.Id.undecided);
            done = addView.FindViewById<ImageView>(Resource.Id.done);
            var builder = new AlertDialog.Builder(ctx);
            builder.SetView(addView);
            builder.SetNegativeButton("Cancel", (sender, e) => { return; });
            var dialog = builder.Create();
            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dialog.Show();
            inprogress.Click += (sender, e) => { stateClick?.Invoke(this, $"{AdapterPosition}-inprogress"); dialog.Dismiss(); };
            undecided.Click += (sender, e) => { stateClick?.Invoke(this, $"{AdapterPosition}-undecided"); dialog.Dismiss(); };
            done.Click += (sender, e) => { stateClick?.Invoke(this, $"{AdapterPosition}-done"); dialog.Dismiss(); };
            return true;
        }
    }
}